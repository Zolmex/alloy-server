using Common.Network;
using DbServer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DbServer.Service;

public class NetworkService : BackgroundService
{
    public static IDbContextFactory<AlloyContext> ContextFactory; // Don't yell at me this is the closest to the correct way I could do
    
    public static AppListener Listener;
    
    public NetworkService(IDbContextFactory<AlloyContext> contextFactory, IConfiguration config)
    {
        ContextFactory = contextFactory;
        
        Listener = new AppListener(int.Parse(config["Server:Port"]!));
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Listener.Start(stoppingToken); // Start accepting connections
        
        while (!stoppingToken.IsCancellationRequested)
        {
            NetworkHealthMonitor();
            
            await using var context = await ContextFactory.CreateDbContextAsync(stoppingToken);
            Console.WriteLine($"Connected: {await context.Database.CanConnectAsync(stoppingToken)} | Connection count: {Listener.AppConnections.Count}");
            
            await Task.Delay(5 * 1000, stoppingToken);
        }
    }

    private void NetworkHealthMonitor()
    {
        var remove = new List<string>();
        foreach (var (name, con) in Listener.AppConnections) // Mark for removal
        {
            if (!con.Socket.Connected)
                remove.Add(name);
        }

        foreach (var key in remove)
        {
            Listener.AppConnections.Remove(key);
        }
    }
}