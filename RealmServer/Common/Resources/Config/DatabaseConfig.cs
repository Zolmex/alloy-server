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
        Host = e.GetValue<string>("Host");
        Port = e.GetValue<int>("Port");
        Password = e.GetValue<string>("Password");
        Redis = new RedisConfig(e.Element("Redis"));
    }

    public static DatabaseConfig Config
        => _config ??= Load();

    public string Host { get; private set; }
    public int Port { get; private set; }
    public string Password { get; private set; }
    public RedisConfig Redis { get; private set; }

    private static DatabaseConfig Load() {
        return new DatabaseConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
    }
}

public class RedisConfig {
    public RedisConfig(XElement e) {
        Host = e.GetValue<string>("Host");
        Port = e.GetValue<int>("Port");
        DbIndex = e.GetValue<int>("DbIndex");
        Password = e.GetValue<string>("Password");
    }

    public string Host { get; private set; }
    public int Port { get; private set; }
    public int DbIndex { get; private set; }
    public string Password { get; private set; }
}