using DbServer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DbServer.Service;

public class TestService : BackgroundService
{
    private readonly AlloyContext _dbContext;
    
    public TestService(AlloyContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Connected: {await _dbContext.Database.CanConnectAsync(stoppingToken)}");
            
            await Task.Delay(3 * 1000, stoppingToken);
        }
    }
}