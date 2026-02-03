using Common.Database;
using Common.Database.Models;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace DbServer.Database;

public static class DbCache
{
    public static readonly DbEntityCache<Account> Accounts = new();

    public static readonly DbEntityCache<AccountSkin> AccountSkins = new();

    public static readonly DbEntityCache<AccountStat> AccountStats = new();

    public static readonly DbEntityCache<Character> Characters = new();

    public static readonly DbEntityCache<CharacterDeath> CharacterDeaths = new();

    public static readonly DbEntityCache<CharacterInventory> CharacterInventories = new();

    public static readonly DbEntityCache<CharacterStat> CharacterStats = new();

    public static readonly DbEntityCache<ClassStat> ClassStats = new();

    public static readonly DbEntityCache<CombatStat> CombatStats = new();

    public static readonly DbEntityCache<DungeonStat> DungeonStats = new();

    public static readonly DbEntityCache<ExplorationStat> ExplorationStats = new();

    public static readonly DbEntityCache<Guild> Guilds = new();

    public static readonly DbEntityCache<GuildMember> GuildMembers = new();

    public static readonly DbEntityCache<KillStat> KillStats = new();
    
    public static readonly DbEntityCache<Login> Logins = new();
    
    public static readonly DbEntityCache<AccountLock> AccountLocks = new();
    
    public static readonly DbEntityCache<AccountIgnore> AccountIgnores = new();

    public static async Task<int> SaveChanges() // Returns the number of tables updated
    {
        await using var dbContext = await NetworkService.ContextFactory.CreateDbContextAsync();
        
        Accounts.Save(dbContext);
        AccountSkins.Save(dbContext);
        AccountStats.Save(dbContext);
        Characters.Save(dbContext);
        CharacterDeaths.Save(dbContext);
        CharacterInventories.Save(dbContext);
        CharacterStats.Save(dbContext);
        ClassStats.Save(dbContext);
        CombatStats.Save(dbContext);
        DungeonStats.Save(dbContext);
        ExplorationStats.Save(dbContext);
        Guilds.Save(dbContext);
        GuildMembers.Save(dbContext);
        KillStats.Save(dbContext);
        Logins.Save(dbContext);
        AccountLocks.Save(dbContext);
        AccountIgnores.Save(dbContext);

        return await dbContext.SaveChangesAsync();
    }
}