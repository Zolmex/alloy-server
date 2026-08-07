#region

using System.IO;
using System.Xml.Linq;
using Common.Utilities;

#endregion

namespace Common.Resources.Config;

public class DatabaseConfig {
    private const string ConfigFile = "Resources/Config/Data/databaseConfig.xml";

    private static DatabaseConfig _config;

    public DatabaseConfig(XElement e) {
        DbFile = e.GetValue<string>("DbFile");
    }

    public static DatabaseConfig Config
        => _config ??= Load();

    public string DbFile { get; private set; }

    private static DatabaseConfig Load() {
        return new DatabaseConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }
}