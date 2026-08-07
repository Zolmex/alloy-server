using System;
using Common.Database.Models;
using LiteDB;

namespace Common.Database;

public class DbClient {
    public readonly ILiteCollection<Account> Accounts;
    public readonly ILiteCollection<Login> Logins;
    public readonly ILiteCollection<Guild> Guilds;
    public readonly ILiteCollection<MuteRecord> Mutes;
    public readonly ILiteCollection<BanRecord> Bans;
    
    private readonly LiteDatabase _db;
    
    public DbClient(string dbFilePath) {
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