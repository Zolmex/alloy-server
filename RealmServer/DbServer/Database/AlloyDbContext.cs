using Microsoft.EntityFrameworkCore;

namespace DbServer.Database;

public class AlloyDbContext : DbContext
{
    public AlloyDbContext(DbContextOptions  options) : base(options)
    {
        
    }
}