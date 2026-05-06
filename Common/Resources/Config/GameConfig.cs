#region

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Common.Utilities;

#endregion

namespace Common.Resources.Config;

public class GameConfig {
    private const string ConfigFile = "Resources/Config/Data/gameConfig.xml";

    public int[] StarGoals { get; set; }

    private static GameConfig _config;

    public GameConfig(XElement e) {
        StarGoals = e.GetValue<string>("StarGoals")?.CommaToArray<int>();
    }

    public static GameConfig Config
        => _config ??= Load();

    private static GameConfig Load() {
        return new GameConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }
}