using System.Buffers;
using System.Runtime.InteropServices;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;
using GameServer.Utilities;

namespace GameServer.Game.Worlds;

public class WorldMap {
    public MapTileData this[int x, int y] => Data.Tiles[x, y];
    public readonly MapData Data;

    private readonly World _world;
    private readonly ChunkMap _chunkMap;

    public WorldMap(World world, MapData data) {
        _world = world;
        _chunkMap = new ChunkMap(world, data.Width, data.Height);

        Data = data;
    }

    public void Tick(ref RealmTime time) {
        _chunkMap.Rebuild();
    }
    
    public int[] GetEntityIdsWithin(float x, float y, float radiusSqr, out int count) { // Array is from ArrayPool, must be returned if count != 0
        count = 0;
        
        var chunkX = (int)x / Chunk.CHUNK_SIZE;
        var chunkY = (int)y / Chunk.CHUNK_SIZE;
        if (chunkX < 0 || chunkX >= _chunkMap.Width || chunkY < 0 || chunkY >= _chunkMap.Height)
            return [];

        var selected = ArrayPool<int>.Shared.Rent(10);
        for (var cY = chunkY - 1; cY <= chunkY + 1; cY++)
            for (var cX = chunkX - 1; cX <= chunkX + 1; cX++) {
                if (cX < 0 || cX >= _chunkMap.Width || cY < 0 || cY >= _chunkMap.Height)
                    continue;

                var chunk = _chunkMap.Chunks[cX, cY];
                foreach (var enId in chunk.Entities) {
                    ref var stats = ref _world.EntityStats.Get(enId);
                    if (stats.DistSqr(x, y) > radiusSqr)
                        continue;
                    
                    selected[count++] = enId;
                    if (count >= selected.Length) {
                        var newSelected = ArrayPool<int>.Shared.Rent(count * 2);
                        selected.AsSpan().CopyTo(newSelected);
                        ArrayPool<int>.Shared.Return(selected);
                        selected = newSelected;
                    }
                }
            }

        if (count != 0)
            return selected;
        
        ArrayPool<int>.Shared.Return(selected);
        return [];
    }

    public RefEnumerator<Entity> GetEntitiesWithin(WorldPosData pos, float radiusSqr)
        => GetEntitiesWithin(pos.X, pos.Y, radiusSqr);

    public RefEnumerator<Entity> GetEntitiesWithin(float x, float y, float radiusSqr) {
        var selected = GetEntityIdsWithin(x, y, radiusSqr, out var count);
        if (count == 0)
            return RefEnumerator<Entity>.Empty;
        
        return new RefEnumerator<Entity>(_world.Entities.Set, selected, count);
    }
    
    public RefEnumerator<EntityStats> GetEntityStatsWithin(WorldPosData pos, float radiusSqr) // Methods like this one can be recycled for any entity component
        => GetEntityStatsWithin(pos.X, pos.Y, radiusSqr);

    public RefEnumerator<EntityStats> GetEntityStatsWithin(float x, float y, float radiusSqr) {
        var selected = GetEntityIdsWithin(x, y, radiusSqr, out var count);
        if (count == 0)
            return RefEnumerator<EntityStats>.Empty;
        
        return new RefEnumerator<EntityStats>(_world.EntityStats.Set, selected, count);
    }
    
    public RefEnumerator<EntityBehavior> GetEntityBehaviorWithin(WorldPosData pos, float radiusSqr)
        => GetEntityBehaviorWithin(pos.X, pos.Y, radiusSqr);

    public RefEnumerator<EntityBehavior> GetEntityBehaviorWithin(float x, float y, float radiusSqr) {
        var selected = GetEntityIdsWithin(x, y, radiusSqr, out var count);
        if (count == 0)
            return RefEnumerator<EntityBehavior>.Empty;
        
        return new RefEnumerator<EntityBehavior>(_world.EntityBehaviors.Set, selected, count);
    }

    public int GetNearestPlayer(WorldPosData pos, float radiusSqr)
        => GetNearestPlayer(pos.X, pos.Y, radiusSqr);
    
    public int GetNearestPlayer(float x, float y, float radiusSqr) {
        var min = float.MaxValue;
        var ret = 0;
        foreach (var id in _world.PlayerToUser.Keys) {
            ref var stats = ref _world.EntityStats.Get(id);
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr && dist < min) {
                min = dist;
                ret = stats.Id;
            }
        }

        return ret;
    }

    public IEnumerable<int> GetPlayersWithin(WorldPosData pos, float radiusSqr)
        => GetPlayersWithin(pos.X, pos.Y, radiusSqr);

    public IEnumerable<int> GetPlayersWithin(float x, float y, float radiusSqr) {
        foreach (var id in _world.PlayerToUser.Keys) {
            ref var stats = ref _world.EntityStats.Get(id);
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr)
                yield return stats.Id;
        }
    }

    public int GetNearestEntityByName(string name, WorldPosData pos, float radiusSqr)
        => GetNearestEntityByName(name, pos.X, pos.Y, radiusSqr);
    
    public int GetNearestEntityByName(string name, float x, float y, float radiusSqr) {
        var min = float.MaxValue;
        var ret = 0;
        foreach (var id in GetEntityIdsWithin(x, y, radiusSqr, out _)) {
            ref var stats = ref _world.EntityStats.Get(id);
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

    public int GetFarthestPlayer(WorldPosData pos, float radiusSqr)
        => GetFarthestPlayer(pos.X, pos.Y, radiusSqr);
    
    public int GetFarthestPlayer(float x, float y, float radiusSqr) {
        var max = 0f;
        var ret = 0;
        foreach (var id in _world.PlayerToUser.Keys) {
            ref var stats = ref _world.EntityStats.Get(id);
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
            var dist = stats.DistSqr(x, y);
            if (dist <= radiusSqr)
                act(user);
        }
    }
}