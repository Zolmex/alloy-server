using System.Globalization;
using System.Reflection;
using Common.Database;
using Common.Messaging;
using Common.Resources.Config;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Network;
using GameServer.Messaging;

namespace GameServer;

public class Program {
    private static readonly Logger _log = new(typeof(Program));

    public static readonly Guid Guid = Guid.NewGuid();
    
    public static IAccountServerRpc AccountServerRpc { get; private set; }
    
    public static async Task Main(string[] args) {
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        Console.Title = $"Alloy Server v{version} - GameServer";
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        AppDomain.CurrentDomain.ProcessExit += async (s, e) => await OnShutdownAsync();
        
        var config = GameServerConfig.Config;
        using (var timer = new EasyTimer(LogLevel.Info, "Starting server...", $"Listening on port {config.Port} ([TIME])")) {
            EnumUtils.Load();
            XmlLibrary.Load(config.XmlsDir);
            MerchantsLibrary.Load(config.MerchantsDir);
            WorldLibrary.Load(config.WorldsDir);
            BehaviorLibrary.Load();
            CommandManager.Load();

            (_, AccountServerRpc) = await IpcClient.ConnectAsync(new GameServerRpcHandler(), TaskUtils.Timeout(5));
            await AccountServerRpc.GameServerConnected(Guid);
            _log.Info($"[RPC] Connected to AccountServer. GUID: {Guid}");
            
            DbClient.Load(DatabaseConfig.Config.DbFile);

            RealmManager.Init();

            // Start the socket server to accept and manage TCP connections
            SocketServer.Start(config.Port, config.MaxPlayers);
        }

        GameLogic.Run(config.MsPT);
    }
    
    private static async Task OnShutdownAsync()
    {
        Console.WriteLine("Stopping database...");
        
        await DbClient.Dispose();
        
        Console.WriteLine("Database closed cleanly.");
    }
    
    private static void UnhandledException(object sender, UnhandledExceptionEventArgs args) {
        _log.Fatal(args.ExceptionObject);
    }
}