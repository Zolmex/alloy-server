#region

using Common.Resources.Xml;
using Common.Utilities;
using DbServer.Database;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#endregion

namespace DbServer;

internal class Program {
    private static readonly Logger _log = new(typeof(Program));

    private static void Main(string[] args) {
        ThreadPool.SetMinThreads(2000, 2000);

        var settings = new HostApplicationBuilderSettings
            { Args = args, ContentRootPath = Directory.GetCurrentDirectory(), ApplicationName = "DbServer" };

        var builder = Host.CreateEmptyApplicationBuilder(settings);
        builder.Configuration
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
            .AddUserSecrets<Program>(true)
            .AddEnvironmentVariables();

        XmlLibrary.Load(builder.Configuration["XmlsDir"]);

        builder.Services.AddDbContextFactory<AlloyContext>(optionsBuilder => {
            optionsBuilder.UseMySQL(builder.Configuration.GetConnectionString("Default")!, mysqlOptions => {
                mysqlOptions.CommandTimeout(10);
                mysqlOptions.EnableRetryOnFailure(
                    3,
                    TimeSpan.FromSeconds(10),
                    null);
            });
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior
                .NoTracking); // Default to no tracking, we handle cache manually
        });

        builder.Services.AddHostedService<NetworkService>();

        AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
            var exception = (Exception)e.ExceptionObject;
            _log.Fatal($"Unhandled exception: {exception}");
        };

        using var app = builder.Build();
        app.Run();
    }
}