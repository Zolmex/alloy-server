#region

using Common.Utilities;
using System;
using System.IO;
using System.Xml.Linq;

#endregion

namespace Common.Resources.Config
{
    public class APIConfig
    {
        private const string ConfigFile = "Resources/Config/Data/apiConfig.xml";

        private static APIConfig _config;

        public static APIConfig Config
            => _config ??= Load();

        public int Port { get; private set; }
        public string Address { get; private set; }
        public string BearerToken { get; private set; }

        public APIConfig(XElement e)
        {
            Port = e.GetValue<int>("Port");
            Address = e.GetValue<string>("Address");
            BearerToken = e.GetValue<string>("BearerToken");
        }

        private static APIConfig Load()
        {
            return new APIConfig(XElement.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, ConfigFile))));
        }
    }
}