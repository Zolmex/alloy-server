using System.Globalization;
using System.Reflection;
using Common.Database;
using Common.Resources.Config;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game;

namespace GameServer;

public class Program {
    private static readonly Logger _log = new(typeof(Program));
    
    public static async Task Main(string[] args) {
        Console.Title = $"Realm Server v{Assembly.GetExecutingAssembly().GetName().Version} - GameServer";
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        
        // You will see the use of these classes throughout the entire source, they are static
        // because they don't need to be instantiated, they live and die with the application,
        // and makes it easier to access data wherever in the project's source code
        
        var config = GameServerConfig.Config;
        using (var timer =
               new EasyTimer(LogLevel.Info, "Starting server...", $"Listening on port {config.Port} ([TIME])")) {
            EnumUtils.Load(); // Initialize and prepare our static classes for later use
            XmlLibrary.Load(config.XmlsDir);
            MerchantsLibrary.Load(config.MerchantsDir);
            WorldLibrary.Load(config.WorldsDir);
            // BehaviorLibrary.Load();

            await DbClient.ConnectAsync(DatabaseConfig.Config);

            RealmManager.Init(); // Setup global game logic

            // SocketServer.Start(config.Port, // TODO: look into using System.IO.Pipelines' SocketServer and other networking classes
            //     config.MaxPlayers); // Start the socket server to accept and manage TCP connections
        }

        RealmManager.Run(config.MsPT); // Run world, entities and other game logic
    }
    
    private static void UnhandledException(object sender, UnhandledExceptionEventArgs args) {
        _log.Fatal(args.ExceptionObject);
    }
}