#region

using Common.Utilities;
using System.IO;
using System.Xml.Linq;

#endregion

namespace Common.Resources.Config
{
    public class DiscordBotConfig
    {
        private const string ConfigFile = "Resources/Config/Data/discordBotConfig.xml";

        private static DiscordBotConfig _config;

        public static DiscordBotConfig Config
            => _config ??= Load();

        public string XmlsDir { get; private set; }
        public string SpritesDir { get; private set; }
        public int Port { get; private set; }
        public string Address { get; private set; }

        public DiscordBotConfig(XElement e)
        {
            XmlsDir = e.GetValue<string>("XmlsDir");
            SpritesDir = e.GetValue<string>("SpritesDir");
            Port = e.GetValue<int>("Port");
            Address = e.GetValue<string>("Address");
        }

        private static DiscordBotConfig Load()
        {
            return new DiscordBotConfig(XElement.Parse(File.ReadAllText(ConfigFile)));
        }
    }
}