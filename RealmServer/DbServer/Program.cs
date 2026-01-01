#region

using Common.Network.Messaging;
using Common.Resources.Xml;
using DbServer.Database;
using DbServer.Implementation;
using DbServer.Interface;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#endregion

namespace DbServer;

internal class Program
{
    private static void Main(string[] args)
    {
        
        var settings = new HostApplicationBuilderSettings { Args = args, ContentRootPath = Directory.GetCurrentDirectory(), ApplicationName = "DbServer" };

        var builder = Host.CreateEmptyApplicationBuilder(settings);
        builder.Configuration
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables();
        
        XmlLibrary.Load(builder.Configuration["XmlsDir"]);

        builder.Services.AddDbContextFactory<AlloyContext>(optionsBuilder =>
        {
            optionsBuilder.UseMySQL(builder.Configuration.GetConnectionString("Default")!);
        });
        
        //builder.Services.AddSingleton<IBackupManager, FileBackupManager>();
        //builder.Services.AddSingleton<IRedisConnection, RedisClientConnection>();

        // builder.Services.AddHostedService<BackupService>();

        builder.Services.AddHostedService<NetworkService>();

        using var app = builder.Build();
        app.Run();
    }
}