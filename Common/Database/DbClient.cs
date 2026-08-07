using System;
using Common.Database.Models;
using LiteDB;

namespace Common.Database;

public static class DbClient {
    public static ILiteCollection<Account> Accounts;
    public static ILiteCollection<Login> Logins;
    public static ILiteCollection<Guild> Guilds;
    public static ILiteCollection<MuteRecord> Mutes;
    public static ILiteCollection<BanRecord> Bans;
    
    private static LiteDatabase _db;
    
    public static void Load(string dbFilePath) {
        var connectionString = new ConnectionString() {
            Filename = dbFilePath,
            Connection = ConnectionType.Shared
        };
        
        _db = new LiteDatabase(connectionString);
        Accounts = _db.GetCollection<Account>("accounts");
        Logins = _db.GetCollection<Login>("logins");
        Guilds = _db.GetCollection<Guild>("guilds");
        Mutes = _db.GetCollection<MuteRecord>("mutes");
        Bans = _db.GetCollection<BanRecord>("bans");
        
        Accounts.EnsureIndex(x => x.Name, true);
        Accounts.EnsureIndex(x => x.GuildId);
        Logins.EnsureIndex(x => x.Name, true);
        Guilds.EnsureIndex(x => x.Name, true);
        Mutes.EnsureIndex(x => x.TargetAccId);
        Mutes.EnsureIndex(x => x.ModeratorAccId);
        Mutes.EnsureIndex(x => x.Reason);
        Bans.EnsureIndex(x => x.TargetAccId);
        Bans.EnsureIndex(x => x.ModeratorAccId);
        Bans.EnsureIndex(x => x.Reason);
    }
}