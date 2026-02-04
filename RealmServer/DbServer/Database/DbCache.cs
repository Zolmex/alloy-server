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

    public static async Task<DbModel?> Find(string key) // Finds entity in one of the sets when you don't know in which table it could be
    {
        var modelName = key.Split('.')[0];
        return modelName switch
        {
            "account" => await Accounts.Get(key),
            "accountSkin" => await AccountSkins.Get(key),
            "accountStat" => await AccountStats.Get(key),
            "character" => await Characters.Get(key),
            "characterDeath" => await CharacterDeaths.Get(key),
            "characterInventory" => await CharacterInventories.Get(key),
            "characterStat" => await CharacterStats.Get(key),
            "classStat" => await ClassStats.Get(key),
            "combatStat" => await CombatStats.Get(key),
            "dungeonStat" => await DungeonStats.Get(key),
            "explorationStat" => await ExplorationStats.Get(key),
            "guild" => await Guilds.Get(key),
            "guildMember" => await GuildMembers.Get(key),
            "killStat" => await KillStats.Get(key),
            "login" => await Logins.Get(key),
            "accountLock" => await AccountLocks.Get(key),
            "accountIgnore" => await AccountIgnores.Get(key),
            _ => null
        };
    }
    
    public static void Update(string key, DbModel entity, string[] properties)
    {
        var modelName = key.Split('.')[0];
        switch (modelName)
        {
            case "account":
                Accounts.Update(entity as Account, properties);
                break;
            case "accountSkin":
                AccountSkins.Update(entity as AccountSkin, properties);
                break;
            case "accountStat":
                AccountStats.Update(entity as AccountStat, properties);
                break;
            case "character":
                Characters.Update(entity as Character, properties);
                break;
            case "characterDeath":
                CharacterDeaths.Update(entity as CharacterDeath, properties);
                break;
            case "characterInventory":
                CharacterInventories.Update(entity as CharacterInventory, properties);
                break;
            case "characterStat":
                CharacterStats.Update(entity as CharacterStat, properties);
                break;
            case "classStat":
                ClassStats.Update(entity as ClassStat, properties);
                break;
            case "combatStat":
                CombatStats.Update(entity as CombatStat, properties);
                break;
            case "dungeonStat":
                DungeonStats.Update(entity as DungeonStat, properties);
                break;
            case "explorationStat":
                ExplorationStats.Update(entity as ExplorationStat, properties);
                break;
            case "guild":
                Guilds.Update(entity as Guild, properties);
                break;
            case "guildMember":
                GuildMembers.Update(entity as GuildMember, properties);
                break;
            case "killStat":
                KillStats.Update(entity as KillStat, properties);
                break;
            case "login":
                Logins.Update(entity as Login, properties);
                break;
            case "accountLock":
                AccountLocks.Update(entity as AccountLock, properties);
                break;
            case "accountIgnore":
                AccountIgnores.Update(entity as AccountIgnore, properties);
                break;
        }
    }

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