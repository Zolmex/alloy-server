using Common.Database;
using Common.Database.Models;
using Common.Utilities;
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
            "account" => await Accounts.GetAsync(key),
            "accountSkin" => await AccountSkins.GetAsync(key),
            "accountStat" => await AccountStats.GetAsync(key),
            "character" => await Characters.GetAsync(key),
            "characterDeath" => await CharacterDeaths.GetAsync(key),
            "characterInventory" => await CharacterInventories.GetAsync(key),
            "characterStat" => await CharacterStats.GetAsync(key),
            "classStat" => await ClassStats.GetAsync(key),
            "combatStat" => await CombatStats.GetAsync(key),
            "dungeonStat" => await DungeonStats.GetAsync(key),
            "explorationStat" => await ExplorationStats.GetAsync(key),
            "guild" => await Guilds.GetAsync(key),
            "guildMember" => await GuildMembers.GetAsync(key),
            "killStat" => await KillStats.GetAsync(key),
            "login" => await Logins.GetAsync(key),
            "accountLock" => await AccountLocks.GetAsync(key),
            "accountIgnore" => await AccountIgnores.GetAsync(key),
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
    
    public static void CacheEntity(DbModel entity)
    {
        var modelName = entity.Key.Split('.')[0];
        switch (modelName)
        {
            case "account":
                Accounts.CacheEntity(entity as Account);
                break;
            case "accountSkin":
                AccountSkins.CacheEntity(entity as AccountSkin);
                break;
            case "accountStat":
                AccountStats.CacheEntity(entity as AccountStat);
                break;
            case "character":
                Characters.CacheEntity(entity as Character);
                break;
            case "characterDeath":
                CharacterDeaths.CacheEntity(entity as CharacterDeath);
                break;
            case "characterInventory":
                CharacterInventories.CacheEntity(entity as CharacterInventory);
                break;
            case "characterStat":
                CharacterStats.CacheEntity(entity as CharacterStat);
                break;
            case "classStat":
                ClassStats.CacheEntity(entity as ClassStat);
                break;
            case "combatStat":
                CombatStats.CacheEntity(entity as CombatStat);
                break;
            case "dungeonStat":
                DungeonStats.CacheEntity(entity as DungeonStat);
                break;
            case "explorationStat":
                ExplorationStats.CacheEntity(entity as ExplorationStat);
                break;
            case "guild":
                Guilds.CacheEntity(entity as Guild);
                break;
            case "guildMember":
                GuildMembers.CacheEntity(entity as GuildMember);
                break;
            case "killStat":
                KillStats.CacheEntity(entity as KillStat);
                break;
            case "login":
                Logins.CacheEntity(entity as Login);
                break;
            case "accountLock":
                AccountLocks.CacheEntity(entity as AccountLock);
                break;
            case "accountIgnore":
                AccountIgnores.CacheEntity(entity as AccountIgnore);
                break;
        }
    }
    
    public static void CacheChildren(DbModel entity)
    {
        var modelName = entity.Key.Split('.')[0];
        IEnumerable<string> includePaths = modelName switch
        {
            "account" => Account.GetIncludes(),
            "accountSkin" => AccountSkin.GetIncludes(),
            "accountStat" => AccountStat.GetIncludes(),
            "character" => Character.GetIncludes(),
            "characterDeath" => CharacterDeath.GetIncludes(),
            "characterInventory" => CharacterInventory.GetIncludes(),
            "characterStat" => CharacterStat.GetIncludes(),
            "classStat" => ClassStat.GetIncludes(),
            "combatStat" => CombatStat.GetIncludes(),
            "dungeonStat" => DungeonStat.GetIncludes(),
            "explorationStat" => ExplorationStat.GetIncludes(),
            "guild" => Guild.GetIncludes(),
            "guildMember" => GuildMember.GetIncludes(),
            "killStat" => KillStat.GetIncludes(),
            "login" => Login.GetIncludes(),
            "accountLock" => AccountLock.GetIncludes(),
            "accountIgnore" => AccountIgnore.GetIncludes(),
            _ => throw new InvalidDataException($"Model Name not found: {modelName}")
        };
        
        foreach (var path in includePaths) // Since include paths equal the property names, we can use reflection
        {
            var propName = path.Split('.')[0]; // First property is always child of T, second is always a child of the child of T (inception)
            var prop = entity.GetType().GetProperty(propName);
            if (prop == null)
                continue;

            var val = prop.GetValue(entity);
            if (val is not DbModel child)
            {
                if (val is not ICollection<DbModel> children)
                    continue;

                foreach (var colChild in children)
                {
                    CacheEntity(colChild);
                    CacheChildren(colChild);
                }
                continue;
            }

            Logger.Debug($"Caching {child.Key} | {child.GetHashCode()}");
            
            CacheEntity(child); // Add the child to its respective cache

            // Now cache the children of this child
            CacheChildren(child);
        }
    }
    
    public static async Task AddEntityAsync(DbModel entity)
    {
        var modelName = entity.Key.Split('.')[0];
        switch (modelName)
        {
            case "account":
                await Accounts.AddAsync(entity as Account);
                break;
            case "accountSkin":
                await AccountSkins.AddAsync(entity as AccountSkin);
                break;
            case "accountStat":
                await AccountStats.AddAsync(entity as AccountStat);
                break;
            case "character":
                await Characters.AddAsync(entity as Character);
                break;
            case "characterDeath":
                await CharacterDeaths.AddAsync(entity as CharacterDeath);
                break;
            case "characterInventory":
                await CharacterInventories.AddAsync(entity as CharacterInventory);
                break;
            case "characterStat":
                await CharacterStats.AddAsync(entity as CharacterStat);
                break;
            case "classStat":
                await ClassStats.AddAsync(entity as ClassStat);
                break;
            case "combatStat":
                await CombatStats.AddAsync(entity as CombatStat);
                break;
            case "dungeonStat":
                await DungeonStats.AddAsync(entity as DungeonStat);
                break;
            case "explorationStat":
                await ExplorationStats.AddAsync(entity as ExplorationStat);
                break;
            case "guild":
                await Guilds.AddAsync(entity as Guild);
                break;
            case "guildMember":
                await GuildMembers.AddAsync(entity as GuildMember);
                break;
            case "killStat":
                await KillStats.AddAsync(entity as KillStat);
                break;
            case "login":
                await Logins.AddAsync(entity as Login);
                break;
            case "accountLock":
                await AccountLocks.AddAsync(entity as AccountLock);
                break;
            case "accountIgnore":
                await AccountIgnores.AddAsync(entity as AccountIgnore);
                break;
        }
    }
    
    public static async Task DeleteEntityAsync(DbModel entity)
    {
        var modelName = entity.Key.Split('.')[0];
        switch (modelName)
        {
            case "account":
                await Accounts.DeleteAsync(entity as Account);
                break;
            case "accountSkin":
                await AccountSkins.DeleteAsync(entity as AccountSkin);
                break;
            case "accountStat":
                await AccountStats.DeleteAsync(entity as AccountStat);
                break;
            case "character":
                await Characters.DeleteAsync(entity as Character);
                break;
            case "characterDeath":
                await CharacterDeaths.DeleteAsync(entity as CharacterDeath);
                break;
            case "characterInventory":
                await CharacterInventories.DeleteAsync(entity as CharacterInventory);
                break;
            case "characterStat":
                await CharacterStats.DeleteAsync(entity as CharacterStat);
                break;
            case "classStat":
                await ClassStats.DeleteAsync(entity as ClassStat);
                break;
            case "combatStat":
                await CombatStats.DeleteAsync(entity as CombatStat);
                break;
            case "dungeonStat":
                await DungeonStats.DeleteAsync(entity as DungeonStat);
                break;
            case "explorationStat":
                await ExplorationStats.DeleteAsync(entity as ExplorationStat);
                break;
            case "guild":
                await Guilds.DeleteAsync(entity as Guild);
                break;
            case "guildMember":
                await GuildMembers.DeleteAsync(entity as GuildMember);
                break;
            case "killStat":
                await KillStats.DeleteAsync(entity as KillStat);
                break;
            case "login":
                await Logins.DeleteAsync(entity as Login);
                break;
            case "accountLock":
                await AccountLocks.DeleteAsync(entity as AccountLock);
                break;
            case "accountIgnore":
                await AccountIgnores.DeleteAsync(entity as AccountIgnore);
                break;
        }
    }
    
    public static async Task AddChildrenAsync(DbModel entity)
    {
        var includePaths = GetIncludePaths(entity);
        
        foreach (var path in includePaths) // Since include paths equal the property names, we can use reflection
        {
            var propName = path.Split('.')[0]; // First property is always child of T, second is always a child of the child of T (inception)
            var prop = entity.GetType().GetProperty(propName);
            if (prop == null)
                continue;

            var val = prop.GetValue(entity);
            if (val is not DbModel child)
            {
                if (val is not ICollection<DbModel> children)
                    continue;

                foreach (var colChild in children)
                {
                    await AddEntityAsync(colChild);
                    await AddChildrenAsync(colChild);
                }
                continue;
            }

            Logger.Debug($"Adding {child.Key} | {child.GetHashCode()}");
            
            await AddEntityAsync(child); // Add the child to its respective cache/table

            // Now add the children of this child
            await AddChildrenAsync(child);
        }
    }
    
    public static async Task DeleteChildrenAsync(DbModel entity)
    {
        var includePaths = GetIncludePaths(entity);
        
        foreach (var path in includePaths) // Since include paths equal the property names, we can use reflection
        {
            var propName = path.Split('.')[0]; // First property is always child of T, second is always a child of the child of T (inception)
            var prop = entity.GetType().GetProperty(propName);
            if (prop == null)
                continue;

            var val = prop.GetValue(entity);
            if (val is not DbModel child)
            {
                if (val is not ICollection<DbModel> children)
                    continue;

                foreach (var colChild in children)
                {
                    await DeleteEntityAsync(colChild);
                    await DeleteChildrenAsync(colChild);
                }
                continue;
            }

            Logger.Debug($"Deleting {child.Key} | {child.GetHashCode()}");
            
            await DeleteEntityAsync(child); // Delete the child to its respective cache/table

            // Now delete the children of this child
            await DeleteChildrenAsync(child);
        }
    }

    private static IEnumerable<string> GetIncludePaths(DbModel entity)
    {
        var modelName = entity.Key.Split('.')[0];
        return modelName switch
        {
            "account" => Account.GetIncludes(),
            "accountSkin" => AccountSkin.GetIncludes(),
            "accountStat" => AccountStat.GetIncludes(),
            "character" => Character.GetIncludes(),
            "characterDeath" => CharacterDeath.GetIncludes(),
            "characterInventory" => CharacterInventory.GetIncludes(),
            "characterStat" => CharacterStat.GetIncludes(),
            "classStat" => ClassStat.GetIncludes(),
            "combatStat" => CombatStat.GetIncludes(),
            "dungeonStat" => DungeonStat.GetIncludes(),
            "explorationStat" => ExplorationStat.GetIncludes(),
            "guild" => Guild.GetIncludes(),
            "guildMember" => GuildMember.GetIncludes(),
            "killStat" => KillStat.GetIncludes(),
            "login" => Login.GetIncludes(),
            "accountLock" => AccountLock.GetIncludes(),
            "accountIgnore" => AccountIgnore.GetIncludes(),
            _ => throw new InvalidDataException($"Model Name not found: {modelName}")
        };
    }

    public static async Task<int> SaveChanges() // Returns the number of entries written to the database
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