using DbServer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DbServer.Service;

public class TestService : BackgroundService
{
    private readonly AlloyDbContext _dbContext;
    
    public TestService(AlloyDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Test: {await _dbContext.Database.CanConnectAsync(stoppingToken)} | ConString: {_dbContext.Database.GetConnectionString()}");
            
            await Task.Delay(3 * 1000, stoppingToken);
        }
    }
}