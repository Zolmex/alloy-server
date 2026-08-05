using Common.Resources.Xml;
using Common.Utilities;
using Newtonsoft.Json;

namespace DbServer;

internal class Program {
    
    private static readonly Logger _log = new(typeof(Program));
    public static readonly Dictionary<string, string> Config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("config.json"));

    private static void Main(string[] args) {
        XmlLibrary.Load(Config["XmlsDir"]); // Load game data
        
        // TODO: Start LiteDB instance
        
        // TODO: start tcp listener

        AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
            var exception = (Exception)e.ExceptionObject;
            _log.Fatal($"Unhandled exception: {exception}");
        };
    }
}