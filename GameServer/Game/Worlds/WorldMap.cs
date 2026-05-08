using System.Buffers;
using System.Runtime.InteropServices;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
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

    public RefEnumerator<Entity> GetEntitiesWithin(WorldPosData pos, float radiusSqr) {
        return GetEntitiesWithin(pos.X, pos.Y, radiusSqr);
    }

    public RefEnumerator<Entity> GetEntitiesWithin(float x, float y, float radiusSqr) {
        var chunkX = (int)x / Chunk.CHUNK_SIZE;
        var chunkY = (int)y / Chunk.CHUNK_SIZE;
        if (chunkX < 0 || chunkX >= _chunkMap.Width || chunkY < 0 || chunkY >= _chunkMap.Height)
            return RefEnumerator<Entity>.Empty;

        var selected = ArrayPool<int>.Shared.Rent(10);
        var count = 0;
        for (var cY = chunkY - 1; cY <= chunkY + 1; cY++)
            for (var cX = chunkX - 1; cX <= chunkX + 1; cX++) {
                if (cX < 0 || cX >= _chunkMap.Width || cY < 0 || cY >= _chunkMap.Height)
                    continue;
                
                var chunk = _chunkMap.Chunks[cX, cY];
                foreach (var enId in chunk.Entities) {
                    ref var stats = ref _world.EntityStats.Get(enId);
                    if (stats.DistSqr(x, y) <= radiusSqr) {
                        selected[count++] = enId;
                        if (count >= selected.Length) {
                            var newSelected = ArrayPool<int>.Shared.Rent(count * 2);
                            selected.AsSpan().CopyTo(newSelected);
                            ArrayPool<int>.Shared.Return(selected);
                            selected = newSelected;
                        }
                    }
                }
            }

        if (count == 0) {
            ArrayPool<int>.Shared.Return(selected);
            return RefEnumerator<Entity>.Empty;
        }

        return new RefEnumerator<Entity>(_world.Entities.Set, selected, count);
    }
}