using Arch.Core;
using Arch.System;
using Common.Game;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Worlds;

public partial class ChunkMap {
    public readonly int Width;
    public readonly int Height;
    public readonly Chunk[,] Chunks;

    private readonly World _world;
    
    public ChunkMap(World world, int mapWidth, int mapHeight) {
        _world = world;
        
        Width = (int)MathF.Ceiling((float)mapWidth / Chunk.CHUNK_SIZE);
        Height = (int)MathF.Ceiling((float)mapHeight / Chunk.CHUNK_SIZE);
        Chunks = new Chunk[Width, Height];
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++) {
                Chunks[x, y] = new Chunk();
            }
    }

    public void Rebuild() {
        foreach (var chunk in Chunks)
            chunk.Clear();

        RebuildChunkQuery(_world, this); 
    }

    [Query]
    private static void RebuildChunk([Data] ChunkMap grid, Entity en, ref Position pos) {
        var chunkX = (int)pos.Pos.X / Chunk.CHUNK_SIZE;
        var chunkY = (int)pos.Pos.Y / Chunk.CHUNK_SIZE;

        if ((uint)chunkX < grid.Width && (uint)chunkY < grid.Height)
            grid.Chunks[chunkX, chunkY].Entities.Add(en);
    }
}

public class Chunk {
    public const int CHUNK_SIZE = 16;

    public readonly List<Entity> Entities = [];

    public void Clear() {
        Entities.Clear();
    }
}