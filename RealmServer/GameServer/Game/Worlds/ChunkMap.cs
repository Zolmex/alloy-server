#region

using System;

#endregion

namespace GameServer.Game.Worlds
{
    public class ChunkMap
    {
        public const float CHUNK_SIZE = 16;

        public MapChunk this[int x, int y]
        {
            get
            {
                if (x >= 0 && y >= 0 && x < Width && y < Height)
                    return _chunks[x, y];
                return null;
            }
        }

        private readonly World _world;
        private readonly MapChunk[,] _chunks;
        public readonly int Width;
        public readonly int Height;

        public ChunkMap(World world, int width, int height)
        {
            _world = world;
            Width = (int)Math.Ceiling(width / CHUNK_SIZE);
            Height = (int)Math.Ceiling(height / CHUNK_SIZE);
            _chunks = new MapChunk[Width, Height];
            for (var cY = 0; cY < Height; cY++)
                for (var cX = 0; cX < Width; cX++)
                {
                    var chunk = new MapChunk(world, cX, cY);
                    _chunks[cX, cY] = chunk;
                }
        }
    }
}