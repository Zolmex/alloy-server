using System.Buffers;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;
using GameServer.Utilities;

namespace GameServer.Game.Worlds;

public class WorldMap {
    private static readonly Logger _log = new Logger(typeof(WorldMap));

    public MapTileData this[int x, int y] {
        get {
            if (x < 0 || x >= Data.Width || y < 0 || y >= Data.Height)
                return null;
            return _tiles[x, y];
        }
    }
    public Dictionary<TileRegion, HashSet<IntPoint>> Regions = [];

    public readonly MapData Data;

    private readonly World _world;
    private readonly ChunkMap _chunkMap;
    private readonly MapTileData[,] _tiles;
    private readonly SpatialQueryCache _queryCache = new();

    public WorldMap(World world, MapData data) {
        _world = world;
        _chunkMap = new ChunkMap(world, data.Width, data.Height);
        _tiles = new MapTileData[data.Width, data.Height];
        for (var y = 0; y < data.Height; y++)
            for (var x = 0; x < data.Width; x++) {
                var tile = data.Tiles[x, y];
                _tiles[x, y] = tile.Clone();
                if (tile.Region == TileRegion.None)
                    continue;
                
                if (!Regions.TryGetValue(tile.Region, out var regions))
                    regions = new HashSet<IntPoint>();
                regions.Add(new IntPoint(x, y));
            }

        Data = data;
    }

    public void Tick(ref RealmTime time) {
        _chunkMap.Rebuild();
        _queryCache.Invalidate();
    }

    public bool IsPassable(int x, int y, bool spawning = false, bool bypassNoWalk = false) {
        if (x < 0 || x >= Data.Width || y < 0 || y >= Data.Height)
            return false;

        var tile = this[x, y];
        if (tile.Desc.NoWalk && !bypassNoWalk)
            return false;

        if (tile.ObjectType == 0)
            return true;

        return !tile.FullOccupy && !tile.EnemyOccupySquare && (spawning || !tile.OccupySquare);
    }

    public void SpawnSetPiece(string spName, int spawnX, int spawnY, int mapIndex = -1, bool center = false) {
        if (spawnX < 0 || spawnY < 0 || spawnX > Data.Width || spawnY > Data.Height)
            return;

        if (!WorldLibrary.MapDatas.TryGetValue(spName, out var setpiece)) {
            _log.Error($"Invalid setpiece: {spName}");
            return;
        }

        var map = mapIndex == -1 ? setpiece.RandomElement() : setpiece[mapIndex];
        if (center) {
            spawnX -= map.Width / 2;
            spawnY -= map.Height / 2;
        }

        for (var spY = 0; spY < map.Height; spY++)
            for (var spX = 0; spX < map.Width; spX++) {
                var x = spawnX + spX;
                var y = spawnY + spY;
                if (x < 0 || y < 0 || x > Data.Width || y > Data.Height)
                    continue;

                var tile = this[x, y]; // Clone because we'll be making changes to this tile
                var spTile = map.Tiles[spX, spY];
                if (spTile.GroundType != 255) {
                    tile.GroundType = spTile.GroundType;
                }

                if (spTile.ObjectType != 0xff && spTile.ObjectType != 0) {
                    var entity = new Entity(spTile.ObjectType);
                    if (entity.Desc.Static) {
                        _world.LeaveWorld(tile.ObjectId);
                        tile.SetObject(entity.Desc);
                        tile.ObjectId = entity.Id;
                    }

                    _world.EnterWorld(ref entity);
                    ref var enStats = ref _world.EntityStats.Get(entity.Id);
                    enStats.Move(x + 0.5f, y + 0.5f);
                }

                var pos = new IntPoint { X = x, Y = y };
                if (spTile.Region == TileRegion.None)
                    Regions[tile.Region].Remove(pos);
                else
                    Regions[spTile.Region].Add(pos);

                _world.PlayerSights.TileUpdate(pos);
            }
    }

    public EntityId[] GetEntityIdsWithin(float x, float y, float radiusSqr, out int count)
    {
        return _queryCache.GetOrComputeWithCount(x, y, radiusSqr,
            compute: () => ComputeEntityIdsWithin(x, y, radiusSqr),
            out count);
    }

    // Raw chunk-map traversal — only called on cache miss
    private (EntityId[] ids, int count) ComputeEntityIdsWithin(float x, float y, float radiusSqr)
    {
        var chunkX = (int)x / Chunk.CHUNK_SIZE;
        var chunkY = (int)y / Chunk.CHUNK_SIZE;
        if (chunkX < 0 || chunkX >= _chunkMap.Width || chunkY < 0 || chunkY >= _chunkMap.Height)
            return ([], 0);

        var selected = ArrayPool<EntityId>.Shared.Rent(10);
        var count = 0;

        for (var cY = chunkY - 1; cY <= chunkY + 1; cY++)
            for (var cX = chunkX - 1; cX <= chunkX + 1; cX++)
            {
                if (cX < 0 || cX >= _chunkMap.Width || cY < 0 || cY >= _chunkMap.Height)
                    continue;

                var chunk = _chunkMap.Chunks[cX, cY];
                foreach (var enId in chunk.Entities)
                {
                    ref var stats = ref _world.EntityStats.Get(enId);
                    if (stats.Id == EntityId.Null)
                        continue;
                    if (stats.DistSqr(x, y) > radiusSqr)
                        continue;

                    selected[count++] = enId;
                    if (count >= selected.Length)
                    {
                        var grown = ArrayPool<EntityId>.Shared.Rent(count * 2);
                        selected.AsSpan().CopyTo(grown);
                        ArrayPool<EntityId>.Shared.Return(selected);
                        selected = grown;
                    }
                }
            }

        // Return the ArrayPool array + count; cache will copy and return it.
        if (count == 0)
        {
            ArrayPool<EntityId>.Shared.Return(selected);
            return ([], 0);
        }
        return (selected, count);
    }

    public RefEnumerator<Entity> GetEntitiesWithin(WorldPosData pos, float radiusSqr)
        => GetEntitiesWithin(pos.X, pos.Y, radiusSqr);

    public RefEnumerator<Entity> GetEntitiesWithin(float x, float y, float radiusSqr) {
        var ids = GetEntityIdsWithin(x, y, radiusSqr, out var count);
        if (count == 0)
            return RefEnumerator<Entity>.Empty;
        
        var pooled = ArrayPool<EntityId>.Shared.Rent(count);
        ids.AsSpan(0, count).CopyTo(pooled);
        return new RefEnumerator<Entity>(_world.Entities.Set, pooled, count);
    }
    
    public EntityId GetNearestPlayer(WorldPosData pos, float radiusSqr)
        => GetNearestPlayer(pos.X, pos.Y, radiusSqr);
    
    public EntityId GetNearestPlayer(float x, float y, float radiusSqr) {
        var min = float.MaxValue;
        var ret = EntityId.Null;
        foreach (var (id, _) in _world.PlayerToUser) {
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr && dist < min) {
                min = dist;
                ret = stats.Id;
            }
        }

        return ret;
    }

    public IEnumerable<EntityId> GetPlayersWithin(WorldPosData pos, float radiusSqr)
        => GetPlayersWithin(pos.X, pos.Y, radiusSqr);

    public IEnumerable<EntityId> GetPlayersWithin(float x, float y, float radiusSqr) {
        foreach (var (id, _) in _world.PlayerToUser) {
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return stats.Id;
        }
    }
    
    public IEnumerable<User> GetUsersWithin(WorldPosData pos, float radiusSqr)
        => GetUsersWithin(pos.X, pos.Y, radiusSqr);

    public IEnumerable<User> GetUsersWithin(float x, float y, float radiusSqr) {
        foreach (var (id, user) in _world.PlayerToUser) {
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return user;
        }
    }

    public EntityId GetNearestEntityByName(string name, WorldPosData pos, float radiusSqr)
        => GetNearestEntityByName(name, pos.X, pos.Y, radiusSqr);
    
    public EntityId GetNearestEntityByName(string name, float x, float y, float radiusSqr) {
        var min = float.MaxValue;
        var ret = EntityId.Null;
        foreach (var id in GetEntityIdsWithin(x, y, radiusSqr, out _)) {
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            if (stats.GetString(StatType.Name) != name)
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr && dist < min) {
                min = dist;
                ret = stats.Id;
            }
        }

        return ret;
    }
    
    public EntityId GetNearestOtherEntityByName(WorldPosData pos, EntityId entityId, string name, float radiusSqr)
        => GetNearestOtherEntityByName(pos.X, pos.Y, entityId, name, radiusSqr);
    
    public EntityId GetNearestOtherEntityByName(float x, float y, EntityId entityId, string name, float radiusSqr) {
        var min = float.MaxValue;
        var ret = EntityId.Null;
        foreach (var id in GetEntityIdsWithin(x, y, radiusSqr, out _)) {
            if (id == entityId)
                continue;
            
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            if (name != null && stats.GetString(StatType.Name) != name)
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr && dist < min) {
                min = dist;
                ret = stats.Id;
            }
        }

        return ret;
    }
    
    public IEnumerable<EntityId> GetEntitiesByName(WorldPosData pos, string name, float radiusSqr)
        => GetEntitiesByName(pos.X, pos.Y, name, radiusSqr);
    
    public IEnumerable<EntityId> GetEntitiesByName(float x, float y, string name, float radiusSqr) {
        foreach (var id in GetEntityIdsWithin(x, y, radiusSqr, out _)) {
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            if (stats.GetString(StatType.Name) != name)
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return id;
        }
    }
    
    public IEnumerable<EntityId> GetEntitiesByName(WorldPosData pos, string[] names, float radiusSqr)
        => GetEntitiesByName(pos.X, pos.Y, names, radiusSqr);
    
    public IEnumerable<EntityId> GetEntitiesByName(float x, float y, string[] names, float radiusSqr) {
        foreach (var id in GetEntityIdsWithin(x, y, radiusSqr, out _)) {
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            if (!names.Contains(stats.GetString(StatType.Name)))
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return id;
        }
    }

    public EntityId GetFarthestPlayer(WorldPosData pos, float radiusSqr)
        => GetFarthestPlayer(pos.X, pos.Y, radiusSqr);
    
    public EntityId GetFarthestPlayer(float x, float y, float radiusSqr) {
        var max = 0f;
        var ret = EntityId.Null;
        foreach (var id in _world.PlayerToUser.Keys) {
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr && dist > max) {
                max = dist;
                ret = stats.Id;
            }
        }

        return ret;
    }

    public void BroadcastNearby(WorldPosData pos, float radiusSqr, Action<User> act)
        => BroadcastNearby(pos.X, pos.Y, radiusSqr, act);

    public void BroadcastNearby(float x, float y, float radiusSqr, Action<User> act) {
        foreach (var (id, user) in _world.PlayerToUser) {
            ref var stats = ref _world.EntityStats.Get(id);
            if (stats.Id == EntityId.Null)
                continue;
            
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr)
                act(user);
        }
    }
}