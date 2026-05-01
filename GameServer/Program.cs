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
        
        var config = GameServerConfig.Config;
        using (var timer =
               new EasyTimer(LogLevel.Info, "Starting server...", $"Listening on port {config.Port} ([TIME])")) {
            EnumUtils.Load();
            XmlLibrary.Load(config.XmlsDir);
            MerchantsLibrary.Load(config.MerchantsDir);
            WorldLibrary.Load(config.WorldsDir);

            await DbClient.ConnectAsync(DatabaseConfig.Config);

            RealmManager.Init();

            // SocketServer.Start(config.Port, // TODO: look into using System.IO.Pipelines' SocketServer and other networking classes
            //     config.MaxPlayers); // Start the socket server to accept and manage TCP connections
        }

        GameLogic.Run(config.MsPT);
    }
    
    private static void UnhandledException(object sender, UnhandledExceptionEventArgs args) {
        _log.Fatal(args.ExceptionObject);
    }
}