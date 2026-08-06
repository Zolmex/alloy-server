using Common.Resources.Xml;
using Common.Utilities;
using LiteDB;
using Newtonsoft.Json;

namespace DbServer;

public class Program {
    private static readonly Logger _log = new(typeof(Program));

    public static void Main(string[] args) {
        var configPath = args.Length > 0 ? args[0] : "config.json";
        var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configPath));
        var dbFilePath = config["LiteDbPath"];

        var dbFileDir = Path.GetDirectoryName(dbFilePath);
        if (!string.IsNullOrEmpty(dbFileDir))
            Directory.CreateDirectory(dbFileDir);

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton(_ => new LiteDatabase($"Filename={dbFilePath};Connection=direct"));

        builder.Services.AddGrpc();

        // Optional: gRPC reflection - handy for debugging with grpcurl / Postman.
        builder.Services.AddGrpcReflection();

        var app = builder.Build();

        app.MapGrpcService<Services.DatabaseServiceImpl>();

        // Expose gRPC reflection for `grpc_reflection.v1alpha.ServerReflection`.
        if (app.Environment.IsDevelopment())
            app.MapGrpcReflectionService();

        // Tiny health probe for docker / compose readiness checks.
        app.MapGet("/health", () => "ok");

        XmlLibrary.Load(config["XmlsDir"]); // Load game data
        _log.Info($"DbServer listening on {string.Join(',', app.Urls)}. LiteDB file: {Path.GetFullPath(dbFilePath)}");

        AppDomain.CurrentDomain.UnhandledException += (_, e) => {
            var ex = (Exception)e.ExceptionObject;
            _log.Fatal($"Unhandled domain exception: {ex}");
        };

        app.Run();
    }
}
