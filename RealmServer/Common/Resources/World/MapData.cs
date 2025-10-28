#region

using Common.Resources.Xml;
using Common.Utilities;
using Common.Utilities.Net;
using Ionic.Zlib;
using Newtonsoft.Json;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Common.Resources.World
{
    public enum TileRegion
    {
        None,
        Spawn = 0x01,
        Realm_Portals = 0x02,
        Store_1 = 0x03,
        Store_2 = 0x04,
        Store_3 = 0x05,
        Store_4 = 0x06,
        Store_5 = 0x07,
        Store_6 = 0x08,
        Store_7 = 0x11,
        Store_8 = 0x12,
        Store_9 = 0x13,
        Store_10 = 0x15,
        Store_11 = 0x16,
        Store_12 = 0x17,
        Store_13 = 0x18,
        Store_14 = 0x1a,
        Store_15 = 0x1b,
        Store_16 = 0x1c,
        Store_17 = 0x1d,
        Store_18 = 0x1e,
        Store_19 = 0x1f,
        Store_20 = 0x20,
        Store_21 = 0x21,
        Store_22 = 0x22,
        Store_23 = 0x23,
        Store_24 = 0x24,
        Store_25 = 0x2a,
        Store_26 = 0x2b,
        Store_27 = 0x2c,
        Store_28 = 0x2d,
        Store_29 = 0x2e,
        Store_30 = 0x2f,
        Store_31 = 0x30,
        Store_32 = 0x31,
        Store_33 = 0x32,
        Store_34 = 0x33,
        Store_35 = 0x34,
        Store_36 = 0x35,
        Store_37 = 0x36,
        Store_38 = 0x37,
        Store_39 = 0x38,
        Store_40 = 0x39,
        Vault = 0x09,
        Loot = 0x0a,
        Defender = 0x0b,
        Hallway = 0x0c,
        Enemy = 0x0d,
        Hallway_1 = 0x0e,
        Hallway_2 = 0x0f,
        Hallway_3 = 0x10,
        Gifting_Chest = 0x14,
        PetRegion = 0x25,
        Outside_Arena = 0x26,
        Item_Spawn_Point = 0x27,
        Arena_Central_Spawn = 0x28,
        Arena_Edge_Spawn = 0x29,
        Quest_Monster_Region = 0x30,
        Quest_Monster_Region_2 = 0x3a
    }

    public struct json_dat
    {
        public byte[] data { get; set; }
        public loc[] dict { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public struct loc
    {
        public string ground { get; set; }
        public obj[] objs { get; set; }
        public obj[] regions { get; set; }
    }

    public struct obj
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class MapTileData
    {
        public ushort GroundType;
        public ushort ObjectType;
        public TileRegion Region;
        public string Key;

        // Wmap / Realm
        public string ObjectCfg;
        public byte Elevation;
        public TerrainType Terrain;

        public loc GetEntry()
        {
            var obj = new obj();
            if (ObjectType != 0)
            {
                obj = new obj() { id = XmlLibrary.ObjectDescs[ObjectType].ObjectId, name = Key };
            }

            return new loc() { ground = XmlLibrary.TileDescs[GroundType].GroundId, objs = obj.id != null ? new obj[] { obj } : null, regions = Region != TileRegion.None ? new obj[] { new() { id = Region.ToString() } } : null };
        }
    }

    // https://github.com/dhojka7/realm-server/blob/456166cbd3c43ade24df8f7904db1f7863e4ebde/Game/JSMap.cs#L55
    public class MapData
    {
        private readonly Logger _log = new(typeof(MapData));

        public MapTileData[,] Tiles;
        public int Width;
        public int Height;
        public Dictionary<TileRegion, List<IntPoint>> Regions;

        public MapData(byte[] data, string mapName)
        {
            var wmap = mapName.EndsWith(".wmap");
            if (wmap)
                LoadWMap(data);
            else
                LoadJMap(Encoding.UTF8.GetString(data), mapName);

            InitRegions();
        }

        public MapData(string jsonStr, string mapName)
        {
            LoadJMap(jsonStr, mapName);
            InitRegions();
        }

        private void LoadJMap(string jsonStr, string mapName)
        {
            var json = JsonConvert.DeserializeObject<json_dat>(jsonStr);
            var buffer = ZlibStream.UncompressBuffer(json.data);
            var dict = new Dictionary<ushort, MapTileData>();
            var tiles = new MapTileData[json.width, json.height];

            for (var i = 0; i < json.dict.Length; i++)
            {
                var o = json.dict[i];

                var region = TileRegion.None;
                if (o.regions != null)
                {
                    var regionSuccess = Enum.TryParse(o.regions[0].id.Replace(' ', '_'), out region);
                    if (!regionSuccess)
                        _log.Warn($"Map region unknown. Map: {mapName} Region: {o.regions[0].id}");
                }

                dict[(ushort)i] = new MapTileData { GroundType = o.ground == null ? (ushort)255 : XmlLibrary.Id2Tile(o.ground)?.GroundType ?? 0, ObjectType = o.objs == null ? (ushort)255 : XmlLibrary.Id2Object(o.objs[0].id)?.ObjectType ?? 0, Key = o.objs?[0].name, Region = region };
            }

            using (var rdr = new NetworkReader(new MemoryStream(buffer), false))
                for (var y = 0; y < json.height; y++)
                    for (var x = 0; x < json.width; x++)
                        tiles[x, y] = dict[(ushort)rdr.ReadInt16()];

            //Add composite under cave walls
            for (var x = 0; x < json.width; x++)
                for (var y = 0; y < json.height; y++)
                    if (tiles[x, y].ObjectType != 255)
                    {
                        if (XmlLibrary.ObjectDescs.TryGetValue(tiles[x, y].ObjectType, out var desc))
                            if ((desc.CaveWall || desc.ConnectedWall) && tiles[x, y].GroundType == 255)
                                tiles[x, y].GroundType = 0xfd;
                    }

            Tiles = tiles;
            Width = json.width;
            Height = json.height;
        }

        public string ExportJson()
        {
            var tempDict = new List<loc>();
            var stream = new MemoryStream();
            var wtr = new BinaryWriter(stream);
            var json = new json_dat() { width = Width, height = Height };

            for (var y = 0; y < Height; y++) // Save tile data
            {
                for (var x = 0; x < Width; x++)
                {
                    var tile = Tiles[x, y];
                    tempDict.Add(tile.GetEntry());

                    wtr.Write((short)tempDict.Count);
                }

                _log.Debug($"Y:{y};");
            }

            var buffer = stream.GetBuffer();
            json.dict = tempDict.ToArray();
            json.data = ZlibStream.CompressBuffer(buffer);

            return JsonConvert.SerializeObject(json);
        }

        public void InitRegions()
        {
            Regions = new Dictionary<TileRegion, List<IntPoint>>();
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    var tile = Tiles[x, y];
                    if (tile.Region == TileRegion.None)
                        continue;

                    if (!Regions.ContainsKey(tile.Region))
                        Regions[tile.Region] = new List<IntPoint>();
                    Regions[tile.Region].Add(new IntPoint(x, y));
                }
        }

        private void LoadWMap(byte[] data)
        {
            // Read version from first byte
            int ver = data[0];
            if (ver is < 0 or > 2)
                throw new NotSupportedException("WMap version " + ver);

            // Decompress the entire payload first
            byte[] decompressed;
            using (var ms = new MemoryStream(data, 1, data.Length - 1)) // Skip version byte
                using (var zlib = new ZlibStream(ms, CompressionMode.Decompress))
                    using (var output = new MemoryStream())
                    {
                        zlib.CopyTo(output);
                        decompressed = output.ToArray();
                    }

            var span = decompressed.AsSpan();
            var pos = 0;

            // Read tile templates
            var tileCount = BinaryPrimitives.ReadInt16LittleEndian(span[pos..]);
            pos += 2;

            var tileDatas = new MapTileData[tileCount];
            var objCache = new Dictionary<string, ushort>(tileCount);

            for (var i = 0; i < tileCount; i++)
            {
                // Read tile type
                var tileType = BinaryPrimitives.ReadUInt16LittleEndian(span[pos..]);
                pos += 2;

                // Read object ID string
                var objIdLen = span[pos++];
                var objId = objIdLen > 0
                    ? Encoding.UTF8.GetString(span.Slice(pos, objIdLen))
                    : string.Empty;
                pos += objIdLen;

                // Read object config string
                var cfgLen = span[pos++];
                var objectCfg = cfgLen > 0
                    ? Encoding.UTF8.GetString(span.Slice(pos, cfgLen))
                    : string.Empty;
                pos += cfgLen;

                // Read terrain and region
                var terrain = (TerrainType)span[pos++];
                var region = (TileRegion)span[pos++];

                // Version 1 elevation
                byte elevation = 0;
                if (ver == 1)
                {
                    elevation = span[pos++];
                }

                // Cache XML lookup
                var objType = !string.IsNullOrEmpty(objId)
                    ? GetCachedObjectType(objId, objCache)
                    : (ushort)0;

                tileDatas[i] = new MapTileData
                {
                    GroundType = tileType,
                    ObjectType = objType,
                    ObjectCfg = objectCfg,
                    Terrain = terrain,
                    Region = region,
                    Elevation = elevation
                };
            }

            // Read map dimensions
            Width = BinaryPrimitives.ReadInt32LittleEndian(span[pos..]);
            pos += 4;
            Height = BinaryPrimitives.ReadInt32LittleEndian(span[pos..]);
            pos += 4;
            Tiles = new MapTileData[Width, Height];

            // Read tile indices
            var indexCount = Width * Height;
            var indices = MemoryMarshal.Cast<byte, short>(
                span.Slice(pos, indexCount * 2)
            ).ToArray();
            pos += indexCount * 2;

            // Version 2 elevations
            byte[] elevations = [];
            if (ver == 2)
            {
                elevations = span.Slice(pos, indexCount).ToArray();
                pos += indexCount;
            }

            // Validate full read
            if (pos != span.Length)
            {
                throw new InvalidDataException(
                    $"Data length mismatch. Expected {span.Length} bytes, read {pos} bytes"
                );
            }

            // Parallel tile assignment
            Parallel.For(0, indexCount, i =>
            {
                var x = i % Width;
                var y = i / Width;
                var tile = tileDatas[indices[i]];

                if (ver == 2)
                {
                    tile.Elevation = elevations[i];
                }

                Tiles[x, y] = tile;
            });
        }

        private ushort GetCachedObjectType(string objId, Dictionary<string, ushort> cache)
        {
            if (!cache.TryGetValue(objId, out var type))
            {
                var obj = XmlLibrary.Id2Object(objId);
                type = obj?.ObjectType ?? 0;
                cache[objId] = type;
            }

            return type;
        }
    }
}