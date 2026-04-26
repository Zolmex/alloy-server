#region

using Common;
using Common.Network;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities;
using System.Collections.Generic;
using System.IO;
using GameServer.Game.Entities.Types;

#endregion

namespace GameServer.Game.Worlds;

public class WorldTile
{
    public readonly MapTileData Data;

    private ushort _groundType;

    public WorldTile(MapTileData tile, int x, int y, MapChunk chunk)
    {
        TileDesc = XmlLibrary.TileDescs[tile.GroundType];

        Data = tile;
        X = x;
        Y = y;
        Chunk = chunk;
        GroundType = tile.GroundType;
        ObjectType = tile.ObjectType;
        Region = tile.Region;
    }

    public TileDesc TileDesc { get; private set; }
    public int X { get; }
    public int Y { get; }
    public ushort ObjectType { get; set; }
    public TileRegion Region { get; set; }
    public Entity Object { get; private set; }
    public bool BlocksSight { get; set; }
    public MapChunk Chunk { get; set; }
    public Portal Portal { get; set; }

    public ushort GroundType
    {
        get => _groundType;
        set
        {
            _groundType = value;
            TileDesc = XmlLibrary.TileDescs[value];
        }
    }
    
    public bool FullOccupy { get; private set; }
    public bool EnemyOccupySquare { get; private set; }
    public bool OccupySquare { get; private set; }

    public void SetObject(Entity en)
    {
        ObjectType = en?.Desc.ObjectType ?? 0;
        Object = en;
        FullOccupy = en?.Desc.FullOccupy ?? false;
        EnemyOccupySquare = en?.Desc.EnemyOccupySquare ?? false;
        OccupySquare = en?.Desc.OccupySquare ?? false;
    }

    public void Update(MapTileData newTile)
    {
        GroundType = newTile.GroundType;
        ObjectType = newTile.ObjectType;
        Region = newTile.Region;
    }

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write((short)X);
        wtr.Write((short)Y);
        wtr.Write(GroundType);
    }
}

public class WorldMap
{
    private readonly World _world;

    // https://github.com/dhojka7/realm-server/blob/456166cbd3c43ade24df8f7904db1f7863e4ebde/Game/World.cs#L80
    public WorldMap(World world, MapData map)
    {
        _world = world;

        Width = map.Width;
        Height = map.Height;
        Entities = new List<Entity>();
        Tiles = new WorldTile[Width, Height];
        Chunks = new ChunkMap(world, Width, Height);
        Regions = new Dictionary<TileRegion, List<WorldPosData>>();
        Terrains = new Dictionary<TerrainType, List<WorldPosData>>();
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var cX = (int)(x / ChunkMap.CHUNK_SIZE);
                var cY = (int)(y / ChunkMap.CHUNK_SIZE);
                var chunk = Chunks[cX, cY];
                var js = map.Tiles[x, y];
                var tile = Tiles[x, y] = new WorldTile(js, x, y, chunk);
                if (js.ObjectType != 0xff && js.ObjectType != 0)
                {
                    var entity = Entity.Resolve(js.ObjectType);
                    if (entity.Desc.Static)
                    {
                        if (entity.Desc.BlocksSight)
                            tile.BlocksSight = true;
                        tile.SetObject(entity);
                    }

                    entity.Move(x + 0.5f, y + 0.5f);
                    Entities.Add(entity);
                }

                var pos = new WorldPosData { X = x, Y = y };
                if (tile.Region != TileRegion.None)
                {
                    Regions.TryAdd(tile.Region, []);
                    Regions[tile.Region].Add(pos);
                }

                if (tile.Data.Terrain != TerrainType.None)
                {
                    Terrains.TryAdd(tile.Data.Terrain, []);
                    Terrains[tile.Data.Terrain].Add(pos);
                }
            }
    }

    public int Width { get; }
    public int Height { get; }
    public Dictionary<TileRegion, List<WorldPosData>> Regions { get; }
    public Dictionary<TerrainType, List<WorldPosData>> Terrains { get; }
    public List<Entity> Entities { get; }
    public WorldTile[,] Tiles { get; }
    public ChunkMap Chunks { get; set; }

    public WorldTile this[int x, int y]
    {
        get => Tiles[x, y];
        set => Tiles[x, y] = value;
    }

    public bool Contains(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= Width || y >= Height);
    }
}