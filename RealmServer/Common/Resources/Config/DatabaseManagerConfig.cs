#region

using Common.Utilities;
using System;
using System.IO;
using System.Xml.Linq;

#endregion

namespace Common.Resources.Config
{
    public class DatabaseManagerConfig
    {
        private const string ConfigFile = "Resources/Config/Data/databaseManagerConfig.xml";

        private static DatabaseManagerConfig _config;

        public static DatabaseManagerConfig Config
            => _config ??= Load();

        public string RedisFolderPath { get; private set; }
        public TimeSpan AutoSaveInterval { get; private set; }
        public int AutoBackupLimit { get; private set; }

        private DatabaseManagerConfig(XElement e)
        {
            RedisFolderPath = e.GetValue<string>("RedisFolderPath");
            var intervalSeconds = e.GetValue<int>("AutoSaveInterval");
            AutoBackupLimit = e.GetValue<int>("AutoBackupLimit");

            AutoSaveInterval = TimeSpan.FromSeconds(intervalSeconds);
        }

        private static DatabaseManagerConfig Load()
        {
            return new DatabaseManagerConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
        }
    }
}