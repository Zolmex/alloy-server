#region

using DbServer.Implementation;
using DbServer.Interface;
using DbServer.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#endregion

namespace DbServer;

internal class Program
{
    private static void Main(string[] args)
    {
        var settings = new HostApplicationBuilderSettings { Args = args, ContentRootPath = Directory.GetCurrentDirectory(), ApplicationName = "Database Manager" };

        var builder = Host.CreateEmptyApplicationBuilder(settings);

        builder.Services.AddSingleton<IBackupManager, FileBackupManager>();
        builder.Services.AddSingleton<IRedisConnection, RedisClientConnection>();

        builder.Services.AddHostedService<BackupService>();

        using var app = builder.Build();
        app.Run();
    }
}