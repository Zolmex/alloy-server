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
    
    public static readonly DbEntityCache<AccountVault> AccountVaults = new();
    
    public static readonly DbEntityCache<AccountGift> AccountGifts = new();
    
    public static readonly DbEntityCache<AccountBan> AccountBans = new();
    
    public static readonly DbEntityCache<AccountMute> AccountMutes = new();

    public static async Task<DbModel?> Find(string key) // Finds entity in one of the sets when you don't know in which table it could be
    {
        var modelName = key.Split('.')[0];
        return modelName switch
        {
            Account.KEY_BASE => await Accounts.GetAsync(key),
            AccountSkin.KEY_BASE => await AccountSkins.GetAsync(key),
            AccountStat.KEY_BASE => await AccountStats.GetAsync(key),
            Character.KEY_BASE => await Characters.GetAsync(key),
            CharacterDeath.KEY_BASE => await CharacterDeaths.GetAsync(key),
            CharacterInventory.KEY_BASE => await CharacterInventories.GetAsync(key),
            CharacterStat.KEY_BASE => await CharacterStats.GetAsync(key),
            ClassStat.KEY_BASE => await ClassStats.GetAsync(key),
            CombatStat.KEY_BASE => await CombatStats.GetAsync(key),
            DungeonStat.KEY_BASE => await DungeonStats.GetAsync(key),
            ExplorationStat.KEY_BASE => await ExplorationStats.GetAsync(key),
            Guild.KEY_BASE => await Guilds.GetAsync(key),
            GuildMember.KEY_BASE => await GuildMembers.GetAsync(key),
            KillStat.KEY_BASE => await KillStats.GetAsync(key),
            Login.KEY_BASE => await Logins.GetAsync(key),
            AccountLock.KEY_BASE => await AccountLocks.GetAsync(key),
            AccountIgnore.KEY_BASE => await AccountIgnores.GetAsync(key),
            AccountVault.KEY_BASE => await AccountVaults.GetAsync(key),
            AccountGift.KEY_BASE => await AccountGifts.GetAsync(key),
            AccountBan.KEY_BASE => await AccountBans.GetAsync(key),
            AccountMute.KEY_BASE => await AccountMutes.GetAsync(key),
            _ => null
        };
    }
    
    public static void Update(string key, DbModel entity, string[] properties)
    {
        var modelName = key.Split('.')[0];
        switch (modelName)
        {
            case Account.KEY_BASE:
                Accounts.Update(entity as Account, properties);
                break;
            case AccountSkin.KEY_BASE:
                AccountSkins.Update(entity as AccountSkin, properties);
                break;
            case AccountStat.KEY_BASE:
                AccountStats.Update(entity as AccountStat, properties);
                break;
            case Character.KEY_BASE:
                Characters.Update(entity as Character, properties);
                break;
            case CharacterDeath.KEY_BASE:
                CharacterDeaths.Update(entity as CharacterDeath, properties);
                break;
            case CharacterInventory.KEY_BASE:
                CharacterInventories.Update(entity as CharacterInventory, properties);
                break;
            case CharacterStat.KEY_BASE:
                CharacterStats.Update(entity as CharacterStat, properties);
                break;
            case ClassStat.KEY_BASE:
                ClassStats.Update(entity as ClassStat, properties);
                break;
            case CombatStat.KEY_BASE:
                CombatStats.Update(entity as CombatStat, properties);
                break;
            case DungeonStat.KEY_BASE:
                DungeonStats.Update(entity as DungeonStat, properties);
                break;
            case ExplorationStat.KEY_BASE:
                ExplorationStats.Update(entity as ExplorationStat, properties);
                break;
            case Guild.KEY_BASE:
                Guilds.Update(entity as Guild, properties);
                break;
            case GuildMember.KEY_BASE:
                GuildMembers.Update(entity as GuildMember, properties);
                break;
            case KillStat.KEY_BASE:
                KillStats.Update(entity as KillStat, properties);
                break;
            case Login.KEY_BASE:
                Logins.Update(entity as Login, properties);
                break;
            case AccountLock.KEY_BASE:
                AccountLocks.Update(entity as AccountLock, properties);
                break;
            case AccountIgnore.KEY_BASE:
                AccountIgnores.Update(entity as AccountIgnore, properties);
                break;
            case AccountVault.KEY_BASE:
                AccountVaults.Update(entity as AccountVault, properties);
                break;
            case AccountGift.KEY_BASE:
                AccountGifts.Update(entity as AccountGift, properties);
                break;
            case AccountBan.KEY_BASE:
                AccountBans.Update(entity as AccountBan, properties);
                break;
            case AccountMute.KEY_BASE:
                AccountMutes.Update(entity as AccountMute, properties);
                break;
        }
    }
    
    public static void CacheEntity(DbModel entity)
    {
        var modelName = entity.Key.Split('.')[0];
        switch (modelName)
        {
            case Account.KEY_BASE:
                Accounts.CacheEntity(entity as Account);
                break;
            case AccountSkin.KEY_BASE:
                AccountSkins.CacheEntity(entity as AccountSkin);
                break;
            case AccountStat.KEY_BASE:
                AccountStats.CacheEntity(entity as AccountStat);
                break;
            case Character.KEY_BASE:
                Characters.CacheEntity(entity as Character);
                break;
            case CharacterDeath.KEY_BASE:
                CharacterDeaths.CacheEntity(entity as CharacterDeath);
                break;
            case CharacterInventory.KEY_BASE:
                CharacterInventories.CacheEntity(entity as CharacterInventory);
                break;
            case CharacterStat.KEY_BASE:
                CharacterStats.CacheEntity(entity as CharacterStat);
                break;
            case ClassStat.KEY_BASE:
                ClassStats.CacheEntity(entity as ClassStat);
                break;
            case CombatStat.KEY_BASE:
                CombatStats.CacheEntity(entity as CombatStat);
                break;
            case DungeonStat.KEY_BASE:
                DungeonStats.CacheEntity(entity as DungeonStat);
                break;
            case ExplorationStat.KEY_BASE:
                ExplorationStats.CacheEntity(entity as ExplorationStat);
                break;
            case Guild.KEY_BASE:
                Guilds.CacheEntity(entity as Guild);
                break;
            case GuildMember.KEY_BASE:
                GuildMembers.CacheEntity(entity as GuildMember);
                break;
            case KillStat.KEY_BASE:
                KillStats.CacheEntity(entity as KillStat);
                break;
            case Login.KEY_BASE:
                Logins.CacheEntity(entity as Login);
                break;
            case AccountLock.KEY_BASE:
                AccountLocks.CacheEntity(entity as AccountLock);
                break;
            case AccountIgnore.KEY_BASE:
                AccountIgnores.CacheEntity(entity as AccountIgnore);
                break;
            case AccountVault.KEY_BASE:
                AccountVaults.CacheEntity(entity as AccountVault);
                break;
            case AccountGift.KEY_BASE:
                AccountGifts.CacheEntity(entity as AccountGift);
                break;
            case AccountBan.KEY_BASE:
                AccountBans.CacheEntity(entity as AccountBan);
                break;
            case AccountMute.KEY_BASE:
                AccountMutes.CacheEntity(entity as AccountMute);
                break;
        }
    }
    
    public static void CacheChildren(DbModel entity)
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
            case Account.KEY_BASE:
                await Accounts.AddAsync(entity as Account);
                break;
            case AccountSkin.KEY_BASE:
                await AccountSkins.AddAsync(entity as AccountSkin);
                break;
            case AccountStat.KEY_BASE:
                await AccountStats.AddAsync(entity as AccountStat);
                break;
            case Character.KEY_BASE:
                await Characters.AddAsync(entity as Character);
                break;
            case CharacterDeath.KEY_BASE:
                await CharacterDeaths.AddAsync(entity as CharacterDeath);
                break;
            case CharacterInventory.KEY_BASE:
                await CharacterInventories.AddAsync(entity as CharacterInventory);
                break;
            case CharacterStat.KEY_BASE:
                await CharacterStats.AddAsync(entity as CharacterStat);
                break;
            case ClassStat.KEY_BASE:
                await ClassStats.AddAsync(entity as ClassStat);
                break;
            case CombatStat.KEY_BASE:
                await CombatStats.AddAsync(entity as CombatStat);
                break;
            case DungeonStat.KEY_BASE:
                await DungeonStats.AddAsync(entity as DungeonStat);
                break;
            case ExplorationStat.KEY_BASE:
                await ExplorationStats.AddAsync(entity as ExplorationStat);
                break;
            case Guild.KEY_BASE:
                await Guilds.AddAsync(entity as Guild);
                break;
            case GuildMember.KEY_BASE:
                await GuildMembers.AddAsync(entity as GuildMember);
                break;
            case KillStat.KEY_BASE:
                await KillStats.AddAsync(entity as KillStat);
                break;
            case Login.KEY_BASE:
                await Logins.AddAsync(entity as Login);
                break;
            case AccountLock.KEY_BASE:
                await AccountLocks.AddAsync(entity as AccountLock);
                break;
            case AccountIgnore.KEY_BASE:
                await AccountIgnores.AddAsync(entity as AccountIgnore);
                break;
            case AccountVault.KEY_BASE:
                await AccountVaults.AddAsync(entity as AccountVault);
                break;
            case AccountGift.KEY_BASE:
                await AccountGifts.AddAsync(entity as AccountGift);
                break;
            case AccountBan.KEY_BASE:
                await AccountBans.AddAsync(entity as AccountBan);
                break;
            case AccountMute.KEY_BASE:
                await AccountMutes.AddAsync(entity as AccountMute);
                break;
        }
    }
    
    public static async Task DeleteEntityAsync(DbModel entity)
    {
        var modelName = entity.Key.Split('.')[0];
        switch (modelName)
        {
            case Account.KEY_BASE:
                await Accounts.DeleteAsync(entity as Account);
                break;
            case AccountSkin.KEY_BASE:
                await AccountSkins.DeleteAsync(entity as AccountSkin);
                break;
            case AccountStat.KEY_BASE:
                await AccountStats.DeleteAsync(entity as AccountStat);
                break;
            case Character.KEY_BASE:
                await Characters.DeleteAsync(entity as Character);
                break;
            case CharacterDeath.KEY_BASE:
                await CharacterDeaths.DeleteAsync(entity as CharacterDeath);
                break;
            case CharacterInventory.KEY_BASE:
                await CharacterInventories.DeleteAsync(entity as CharacterInventory);
                break;
            case CharacterStat.KEY_BASE:
                await CharacterStats.DeleteAsync(entity as CharacterStat);
                break;
            case ClassStat.KEY_BASE:
                await ClassStats.DeleteAsync(entity as ClassStat);
                break;
            case CombatStat.KEY_BASE:
                await CombatStats.DeleteAsync(entity as CombatStat);
                break;
            case DungeonStat.KEY_BASE:
                await DungeonStats.DeleteAsync(entity as DungeonStat);
                break;
            case ExplorationStat.KEY_BASE:
                await ExplorationStats.DeleteAsync(entity as ExplorationStat);
                break;
            case Guild.KEY_BASE:
                await Guilds.DeleteAsync(entity as Guild);
                break;
            case GuildMember.KEY_BASE:
                await GuildMembers.DeleteAsync(entity as GuildMember);
                break;
            case KillStat.KEY_BASE:
                await KillStats.DeleteAsync(entity as KillStat);
                break;
            case Login.KEY_BASE:
                await Logins.DeleteAsync(entity as Login);
                break;
            case AccountLock.KEY_BASE:
                await AccountLocks.DeleteAsync(entity as AccountLock);
                break;
            case AccountIgnore.KEY_BASE:
                await AccountIgnores.DeleteAsync(entity as AccountIgnore);
                break;
            case AccountVault.KEY_BASE:
                await AccountVaults.DeleteAsync(entity as AccountVault);
                break;
            case AccountGift.KEY_BASE:
                await AccountGifts.DeleteAsync(entity as AccountGift);
                break;
            case AccountBan.KEY_BASE:
                await AccountBans.DeleteAsync(entity as AccountBan);
                break;
            case AccountMute.KEY_BASE:
                await AccountMutes.DeleteAsync(entity as AccountMute);
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
            Account.KEY_BASE => Account.GetIncludes(),
            AccountSkin.KEY_BASE => AccountSkin.GetIncludes(),
            AccountStat.KEY_BASE => AccountStat.GetIncludes(),
            Character.KEY_BASE => Character.GetIncludes(),
            CharacterDeath.KEY_BASE => CharacterDeath.GetIncludes(),
            CharacterInventory.KEY_BASE => CharacterInventory.GetIncludes(),
            CharacterStat.KEY_BASE => CharacterStat.GetIncludes(),
            ClassStat.KEY_BASE => ClassStat.GetIncludes(),
            CombatStat.KEY_BASE => CombatStat.GetIncludes(),
            DungeonStat.KEY_BASE => DungeonStat.GetIncludes(),
            ExplorationStat.KEY_BASE => ExplorationStat.GetIncludes(),
            Guild.KEY_BASE => Guild.GetIncludes(),
            GuildMember.KEY_BASE => GuildMember.GetIncludes(),
            KillStat.KEY_BASE => KillStat.GetIncludes(),
            Login.KEY_BASE => Login.GetIncludes(),
            AccountLock.KEY_BASE => AccountLock.GetIncludes(),
            AccountIgnore.KEY_BASE => AccountIgnore.GetIncludes(),
            AccountVault.KEY_BASE => AccountVault.GetIncludes(),
            AccountGift.KEY_BASE => AccountGift.GetIncludes(),
            AccountBan.KEY_BASE => AccountBan.GetIncludes(),
            AccountMute.KEY_BASE => AccountMute.GetIncludes(),
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
        AccountVaults.Save(dbContext);
        AccountGifts.Save(dbContext);
        AccountBans.Save(dbContext);
        AccountMutes.Save(dbContext);

        return await dbContext.SaveChangesAsync();
    }
}