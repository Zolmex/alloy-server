#region

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Common.Database;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using WebServer.Handlers;

#endregion

namespace WebServer;

internal class Program {
    private static readonly Logger Log = new(typeof(Program));

    private static async Task Main(string[] args) {
        ThreadPool.SetMinThreads(1000, 1000);

        Console.Title = $"Realm Server v{Assembly.GetExecutingAssembly().GetName().Version} - WebServer";
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        // WebServer - as opposed to GameServer - doesn't need to be a high priority process
        // since its operations are not time-critical

        // The program should always be closed by pressing Ctrl + C
        // to ensure the saving of any data that wasn't saved to the database

        AppDomain.CurrentDomain.UnhandledException += UnhandledException;

        var listener = new HttpListener();
        var config = AppEngineConfig.Config;
        using (var timer = new EasyTimer(LogLevel.Info, "Starting server...",
                   $"Listening on {config.Address + ":" + config.Port} ({EasyTimer.Time})")) {
            EnumUtils.Load(); // Initialize and prepare our static classes for later use
            RequestHandler.Load();
            XmlLibrary.Load(config.XmlsDir);

            //DbClient.Connect(DatabaseConfig.Config); // Connect redis client used for communication with the database
            await DbClient.ConnectAsync(DatabaseConfig.Config);

            // ServerControl.Connect(MemberType.AppEngine, "WebServer",
            //     new ServerInfo { Address = config.Address });

            // ServerControl.Subscribe<ShutdownInfo>(ControlChannel.Shutdown, OnShutdownRequested);

            listener.Prefixes.Add($"http://{config.Address}:{config.Port}/"); // Start receiving HTTP requests
            listener.Start();
        }

        var semaphore = new SemaphoreSlim(200);

        while (true) {
            HttpListenerContext context;
            try {
                context = await listener.GetContextAsync(); // non-blocking — frees the loop immediately
            }
            catch (HttpListenerException) {
                break; // listener was stopped (e.g. on shutdown)
            }

            await semaphore.WaitAsync(); // back-pressure: don't accept unbounded work

            _ = Task.Run(async () => {
                try {
                    await HandleRequestAsync(context);
                }
                finally {
                    semaphore.Release();
                }
            });
        }
    }

    private static async Task HandleRequestAsync(HttpListenerContext context) {
        var request = context.Request.Url.LocalPath;
        var ip = GetContextIP(context);

        if (!RequestHandler.Exists(request)) {
            Log.Warn($"Unknown request '{request}' from '{ip}'");
            try {
                context.Response.Close();
            }
            catch { }

            return;
        }

        Log.Debug($"Received '{request}' from '{ip}'");

        NameValueCollection query;
        try {
            using var inputStream = context.Request.InputStream;
            using var reader = new StreamReader(inputStream, Encoding.UTF8);
            query = HttpUtility.ParseQueryString(await reader.ReadToEndAsync());
        }
        catch (HttpListenerException) {
            return;
        }

        var response = await RequestHandler.Handle(request, ip, query);
        response ??= "<Error>Internal error.</Error>";

        try {
            var data = Encoding.UTF8.GetBytes(response);
            context.Response.ContentType = "text/*";
            await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
            context.Response.Close();
        }
        catch (HttpListenerException) { }
    }

    private static void UnhandledException(object sender, UnhandledExceptionEventArgs args) {
        Log.Fatal(args.ExceptionObject);
    }

    private static string GetContextIP(HttpListenerContext context) {
        return context.Request.RemoteEndPoint.ToString().Split(':')[0];
    }
}