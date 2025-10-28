#region

using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

#endregion

namespace Common.Resources.Xml
{
    public static class XmlLibrary
    {
        private static readonly Logger _log = new(typeof(XmlLibrary));

        public static readonly ConcurrentDictionary<ushort, ObjectDesc> ObjectDescs = new();
        private static readonly ConcurrentDictionary<string, ObjectDesc> _id2Object = new();
        private static readonly ConcurrentDictionary<string, ObjectDesc> _id2ObjectNoCase = new(StringComparer.CurrentCultureIgnoreCase);
        public static readonly ConcurrentDictionary<ushort, ContainerDesc> ContainerDescs = new();
        public static readonly ConcurrentDictionary<ushort, PlayerDesc> PlayerDescs = new();
        private static readonly ConcurrentDictionary<string, PlayerDesc> _id2Player = new();
        public static readonly ConcurrentDictionary<ushort, Item> ItemDescs = new();
        private static readonly ConcurrentDictionary<string, Item> _id2Item = new(StringComparer.CurrentCultureIgnoreCase);
        public static readonly ConcurrentDictionary<ushort, TileDesc> TileDescs = new();
        private static readonly ConcurrentDictionary<string, TileDesc> _id2Tile = new();
        public static readonly ConcurrentDictionary<ushort, ProjectileDesc> ProjectileDescs = new();
        public static readonly ConcurrentDictionary<TerrainType, List<ObjectDesc>> TerrainEnemies = new();
        public static readonly ConcurrentDictionary<ushort, Item> Gemstones = new();

        /// <summary>
        /// Loads every .xml file in the directory <paramref name="dir"/>.
        /// </summary>
        /// <param name="dir">Directory containing XML asset files.</param>
        public static void Load(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*xml", SearchOption.AllDirectories);

            Parallel.ForEach(files, file =>
            {
                _log.Debug($"Loading XML {file}...");
                MakeDictionaries(XElement.Parse(File.ReadAllText(file)));
            });
            _log.Info("XML Library loaded successfully.");
        }

        private static void MakeDictionaries(XElement root)
        {
            Parallel.ForEach(root.Elements(), xml =>
            {
                var id = xml.GetAttribute<string>("id");
                var type = xml.GetAttribute<ushort>("type");

                var name = xml.Name.ToString();
                if (name == "Object")
                {
                    if (xml.HasElement("Container"))
                        ContainerDescs.TryAdd(type, new ContainerDesc(xml, id, type));
                    else if (xml.HasElement("Player"))
                    {
                        var plr = new PlayerDesc(xml, id, type);
                        PlayerDescs.TryAdd(type, plr);
                        _id2Player.TryAdd(id, plr);
                    }
                    else if (xml.HasElement("Item"))
                    {
                        var item = new Item(xml);
                        ItemDescs.TryAdd(type, item);
                        _id2Item.TryAdd(id, item);
                        if (xml.HasElement("Gemstone"))
                            Gemstones.TryAdd(type, item);
                    }

                    if (!xml.HasElement("Item") && !xml.HasElement("PetAbility"))
                    {
                        var objDesc = new ObjectDesc(xml, id, type);
                        ObjectDescs.TryAdd(type, objDesc);
                        _id2Object.TryAdd(id, objDesc);
                        _id2ObjectNoCase.TryAdd(id, objDesc);

                        if (objDesc.Terrain != TerrainType.None /* && xml.HasElement("Spawn")*/) // Entities spawned by Oryx in the Realm
                        {
                            var terrain = Enum.Parse<TerrainType>(xml.Element("Terrain").Value);
                            if (!TerrainEnemies.TryGetValue(terrain, out var list))
                                list = TerrainEnemies[terrain] = new List<ObjectDesc>();
                            list.Add(objDesc);
                        }
                    }
                }

                if (name == "Ground")
                {
                    var tile = new TileDesc(xml, id, type);
                    TileDescs.TryAdd(type, tile);
                    _id2Tile.TryAdd(id, tile);
                }
            });
        }

        public static ObjectDesc Id2Object(string id, bool caseSensitive = true)
        {
            ObjectDesc ret;
            if (!caseSensitive)
            {
                if (!_id2ObjectNoCase.TryGetValue(id, out ret))
                    return null;
                return ret;
            }

            if (!_id2Object.TryGetValue(id, out ret))
                return null;
            return ret;
        }

        public static PlayerDesc Id2Player(string id)
        {
            if (!_id2Player.TryGetValue(id, out var ret))
                return null;
            return ret;
        }

        public static Item Id2Item(string id)
        {
            if (!_id2Item.TryGetValue(id, out var ret))
                return null;
            return ret;
        }

        public static TileDesc Id2Tile(string id)
        {
            if (!_id2Tile.TryGetValue(id, out var ret))
                return null;
            return ret;
        }
    }
}