using Common.Structs;
using Common.Utilities.Collections;

namespace GameServer.Game.Worlds;

public class ChunkMap {

    public readonly int Width;
    public readonly int Height;
    public readonly Chunk[,] Chunks;

    private readonly World _world;
    
    public ChunkMap(World world, int mapWidth, int mapHeight) {
        _world = world;
        
        Width = mapWidth / Chunk.CHUNK_SIZE;
        Height = mapHeight / Chunk.CHUNK_SIZE;
        Chunks = new Chunk[Width, Height];
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++) {
                Chunks[x, y] = new Chunk();
            }
    }

    public void Rebuild() {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++) {
                Chunks[x, y].Clear();
            }

        foreach (ref var stats in _world.EntityStats) {
            var chunkX = (int)stats.Pos.X / Chunk.CHUNK_SIZE;
            var chunkY = (int)stats.Pos.Y / Chunk.CHUNK_SIZE;
            if (chunkX < 0 || chunkX >= Width || chunkY < 0 || chunkY >= Height)
                continue;
            
            Chunks[chunkX, chunkY].Entities.Add(stats.Id);
        }
    }
}

public class Chunk {
    public const int CHUNK_SIZE = 16;

    public readonly List<int> Entities = [];

    public void Clear() {
        Entities.Clear();
    }
}