using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Arch.Core;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
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
    public readonly Dictionary<TileRegion, HashSet<IntPoint>> Regions = [];

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
                var tile = _tiles[x, y] = data.Tiles[x, y].Clone();
                if (tile.Region == TileRegion.None)
                    continue;
                
                if (!Regions.TryGetValue(tile.Region, out var regions))
                    regions = Regions[tile.Region] = new HashSet<IntPoint>();
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
                    var desc = XmlLibrary.ObjectDescs[spTile.ObjectType];
                    var entity =_world.EnterWorld(desc);
                    if (desc.Static) {
                        _world.LeaveWorld(tile.Object);
                        tile.SetObject(desc);
                        tile.Object = entity;
                    }
                    
                    ref var enPos = ref _world.Ecs.Get<Position>(entity);
                    enPos.Move(x + 0.5f, y + 0.5f);
                }

                var pos = new IntPoint { X = x, Y = y };
                if (spTile.Region == TileRegion.None)
                    Regions[tile.Region].Remove(pos);
                else
                    Regions[spTile.Region].Add(pos);

                _world.PlayerSightSystem.TileUpdate(pos);
            }
    }

    public IEnumerable<Entity> GetEntitiesWithin(WorldPosData pos, float radiusSqr)
        => GetEntitiesWithin(pos.X, pos.Y, radiusSqr);

    public Entity[] GetEntitiesWithin(float x, float y, float radiusSqr)
    {
        return _queryCache.GetOrComputeWithCount(x, y, radiusSqr,
            compute: () => ComputeEntitiesWithin(x, y, radiusSqr), out _);
    }

    // Raw chunk-map traversal — only called on cache miss
    private (Entity[] entities, int count) ComputeEntitiesWithin(float x, float y, float radiusSqr)
    {
        var chunkX = (int)x / Chunk.CHUNK_SIZE;
        var chunkY = (int)y / Chunk.CHUNK_SIZE;
        if (chunkX < 0 || chunkX >= _chunkMap.Width || chunkY < 0 || chunkY >= _chunkMap.Height)
            return ([], 0);

        var selected = ArrayPool<Entity>.Shared.Rent(10);
        var count = 0;

        for (var cY = chunkY - 1; cY <= chunkY + 1; cY++)
            for (var cX = chunkX - 1; cX <= chunkX + 1; cX++)
            {
                if (cX < 0 || cX >= _chunkMap.Width || cY < 0 || cY >= _chunkMap.Height)
                    continue;

                var chunk = _chunkMap.Chunks[cX, cY];
                foreach (var en in chunk.Entities)
                {
                    ref var pos = ref _world.Ecs.Get<Position>(en);
                    if (pos.DistSqr(x, y) > radiusSqr)
                        continue;

                    selected[count++] = en;
                    if (count >= selected.Length)
                    {
                        var grown = ArrayPool<Entity>.Shared.Rent(count * 2);
                        selected.AsSpan().CopyTo(grown);
                        ArrayPool<Entity>.Shared.Return(selected);
                        selected = grown;
                    }
                }
            }

        // Return the ArrayPool array + count; cache will copy and return it.
        if (count == 0)
        {
            ArrayPool<Entity>.Shared.Return(selected);
            return ([], 0);
        }
        return (selected, count);
    }

    public Entity GetNearestPlayer(WorldPosData pos, float radiusSqr)
        => GetNearestPlayer(pos.X, pos.Y, radiusSqr);
    
    public Entity GetNearestPlayer(float x, float y, float radiusSqr) {
        var min = float.MaxValue;
        var ret = Entity.Null;
        foreach (var (en, _) in _world.Users) {
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr && dist < min) {
                min = dist;
                ret = en;
            }
        }

        return ret;
    }

    public IEnumerable<Entity> GetPlayersWithin(WorldPosData pos, float radiusSqr)
        => GetPlayersWithin(pos.X, pos.Y, radiusSqr);

    public IEnumerable<Entity> GetPlayersWithin(float x, float y, float radiusSqr) {
        foreach (var (en, _) in _world.Users) {
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return en;
        }
    }
    
    public IEnumerable<User> GetUsersWithin(WorldPosData pos, float radiusSqr)
        => GetUsersWithin(pos.X, pos.Y, radiusSqr);

    public IEnumerable<User> GetUsersWithin(float x, float y, float radiusSqr) {
        foreach (var (en, user) in _world.Users) {
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return user;
        }
    }

    public Entity GetNearestEntityByName(string name, WorldPosData pos, float radiusSqr)
        => GetNearestEntityByName(name, pos.X, pos.Y, radiusSqr);
    
    public Entity GetNearestEntityByName(string name, float x, float y, float radiusSqr) {
        var min = float.MaxValue;
        var ret = Entity.Null;
        foreach (var en in GetEntitiesWithin(x, y, radiusSqr)) {
            ref var stats = ref _world.Ecs.Get<Stats>(en);
            if (stats.GetString(StatType.Name) != name)
                continue;
            
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr && dist < min) {
                min = dist;
                ret = en;
            }
        }

        return ret;
    }
    
    public Entity GetNearestOtherEntityByName(WorldPosData pos, Entity entity, string name, float radiusSqr)
        => GetNearestOtherEntityByName(pos.X, pos.Y, entity, name, radiusSqr);
    
    public Entity GetNearestOtherEntityByName(float x, float y, Entity entity, string name, float radiusSqr) {
        var min = float.MaxValue;
        var ret = Entity.Null;
        foreach (var en in GetEntitiesWithin(x, y, radiusSqr)) {
            if (en == entity)
                continue;
            
            ref var stats = ref _world.Ecs.Get<Stats>(en);
            if (name != null && stats.GetString(StatType.Name) != name)
                continue;
            
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr && dist < min) {
                min = dist;
                ret = en;
            }
        }

        return ret;
    }
    
    public IEnumerable<Entity> GetEntitiesByName(WorldPosData pos, string name, float radiusSqr)
        => GetEntitiesByName(pos.X, pos.Y, name, radiusSqr);
    
    public IEnumerable<Entity> GetEntitiesByName(float x, float y, string name, float radiusSqr) {
        foreach (var en in GetEntitiesWithin(x, y, radiusSqr)) {
            ref var stats = ref _world.Ecs.Get<Stats>(en);
            if (stats.GetString(StatType.Name) != name)
                continue;
            
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return en;
        }
    }
    
    public IEnumerable<Entity> GetEntitiesByName(WorldPosData pos, string[] names, float radiusSqr)
        => GetEntitiesByName(pos.X, pos.Y, names, radiusSqr);
    
    public IEnumerable<Entity> GetEntitiesByName(float x, float y, string[] names, float radiusSqr) {
        foreach (var en in GetEntitiesWithin(x, y, radiusSqr)) {
            ref var stats = ref _world.Ecs.Get<Stats>(en);
            if (!names.Contains(stats.GetString(StatType.Name)))
                continue;
            
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return en;
        }
    }

    public Entity GetFarthestPlayer(WorldPosData pos, float radiusSqr)
        => GetFarthestPlayer(pos.X, pos.Y, radiusSqr);
    
    public Entity GetFarthestPlayer(float x, float y, float radiusSqr) {
        var max = 0f;
        var ret = Entity.Null;
        foreach (var en in _world.Users.Keys) {
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr && dist > max) {
                max = dist;
                ret = en;
            }
        }

        return ret;
    }

    public void BroadcastNearby(WorldPosData pos, float radiusSqr, Action<User> act)
        => BroadcastNearby(pos.X, pos.Y, radiusSqr, act);

    public void BroadcastNearby(float x, float y, float radiusSqr, Action<User> act) {
        foreach (var (en, user) in _world.Users) {
            ref var pos = ref _world.Ecs.Get<Position>(en);
            var dist = pos.DistSqr(x, y);
            if (dist <= radiusSqr)
                act(user);
        }
    }
}