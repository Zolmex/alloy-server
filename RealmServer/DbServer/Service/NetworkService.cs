using Common.Network;
using DbServer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DbServer.Service;

public class NetworkService : BackgroundService
{
    public static IDbContextFactory<AlloyContext> ContextFactory; // Don't yell at me this is the closest to the correct way I could do
    
    private readonly AlloyContext _dbContext;
    private readonly AppListener _listener;
    
    public NetworkService(IDbContextFactory<AlloyContext> contextFactory, IConfiguration config)
    {
        ContextFactory = contextFactory;
        
        _dbContext = contextFactory.CreateDbContext();
        _listener = new AppListener(int.Parse(config["Server:Port"]!));
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = _listener.Start(stoppingToken); // Start accepting connections
        
        while (!stoppingToken.IsCancellationRequested)
        {
            NetworkHealthMonitor();
            
            Console.WriteLine($"Connected: {await _dbContext.Database.CanConnectAsync(stoppingToken)} | Connection count: {_listener.AppConnections.Count}");
            
            await Task.Delay(3 * 1000, stoppingToken);
        }
    }

    private void NetworkHealthMonitor()
    {
        var remove = new List<string>();
        foreach (var (ip, con) in _listener.AppConnections) // Mark for removal
        {
            if (!con.Socket.Connected)
                remove.Add(ip);
        }

        foreach (var key in remove)
        {
            _listener.AppConnections.Remove(key);
        }
    }
}