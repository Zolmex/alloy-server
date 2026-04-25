#region

using Common.Database;
using Common.Resources.Config;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game;
using GameServer.Game.Chat;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Network;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace GameServer;

internal class Program
{
    private static readonly Logger Log = new(typeof(Program));

    private static async Task Main(string[] args)
    {
        ThreadPool.SetMinThreads(2000, 2000);
        
        Console.Title = $"Realm Server v{Assembly.GetExecutingAssembly().GetName().Version} - GameServer";
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        Thread.CurrentThread.Priority = ThreadPriority.Highest; // Elevate program's thread and process priority
        using (var p = Process.GetCurrentProcess())
            p.PriorityClass = ProcessPriorityClass.High; // Note: NEVER set to RealTime

        // The program should always be closed by pressing Ctrl + C
        // to ensure the saving of any data that wasn't saved to redis

        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        
        var config = GameServerConfig.Config;
        using (var timer = new EasyTimer(LogLevel.Info, "Starting server...", $"Listening on port {config.Port} ([TIME])"))
        {
            EnumUtils.Load(); // Initialize and prepare our static classes for later use
            XmlLibrary.Load(config.XmlsDir);
            MerchantsLibrary.Load(config.MerchantsDir);
            WorldLibrary.Load(config.WorldsDir);
            BehaviorLibrary.Load();

            await DbClient.ConnectAsync(DatabaseConfig.Config);

            // ServerControl.Connect(MemberType.AppEngine, "GameServer", new ServerInfo { Port = config.Port, Address = config.Address, MaxPlayers = config.MaxPlayers, AdminOnly = config.AdminOnly });
            //
            // ServerControl.Subscribe<ShutdownInfo>(ControlChannel.Shutdown, OnShutdownRequested);
            // ServerControl.Subscribe<bool>(ControlChannel.DbWipe, DbClientOld.OnWipeCompleted);

            RealmManager.Init(); // Finish setting up the game logic

            SocketServer.Start(config.Port, config.MaxPlayers); // Start the socket server to accept and manage TCP connections
        }

        RealmManager.Run(config.MsPT); // Run world, entities and other game logic
    }

    private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        Log.Fatal(args.ExceptionObject);
    }
}