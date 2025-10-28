#region

using Common.Resources.Config;
using Common.Utilities;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

#endregion

namespace Common.Resources.World
{
    public static class WorldLibrary
    {
        private static readonly Logger Log = new(typeof(WorldLibrary));

        public static readonly ConcurrentDictionary<string, WorldConfig> WorldConfigs = new();
        public static readonly ConcurrentDictionary<string, MapData[]> MapDatas = new();

        /// <summary>
        /// Loads every .json file in the directory <paramref name="dir"/>.
        /// </summary>
        /// <param name="dir">Directory containing world config and map files.</param>
        public static void Load(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*json", SearchOption.AllDirectories);
            Parallel.ForEach(files, file =>
            {
                Log.Debug($"Loading world {file}...");

                var config = JsonConvert.DeserializeObject<WorldConfig>(File.ReadAllText(file));
                var maps = DeserializeMaps(config);
                WorldConfigs.TryAdd(config.Name, config);
                if (config.DisplayName != null)
                    WorldConfigs.TryAdd(config.DisplayName, config); // Add by display name as well
                MapDatas[config.Name] = maps;
            });
            Log.Info("World Library loaded successfully.");
        }

        private static MapData[] DeserializeMaps(WorldConfig config)
        {
            if (config.Maps == null)
                return null;

            var maps = new MapData[config.Maps.Length];
            Parallel.For(0, config.Maps.Length, i =>
            {
                var mapName = config.Maps[i];
                var data = File.ReadAllBytes(GameServerConfig.Config.WorldsDir + mapName);
                maps[i] = new MapData(data, mapName);
            });
            return maps;
        }
    }
}