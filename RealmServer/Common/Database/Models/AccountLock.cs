using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountLock : DbModel, IDbQueryable
{
    public const string KEY_BASE = "accountLock";
    
    public override string Key => KEY_BASE + $".{AccountId}.{LockedId}";
    
    public int AccountId { get; set; }
    
    public Account Account { get; set; } = null!;
    
    public int LockedId { get; set; }
    
    public Account Locked { get; set; } = null!;
    
    public AccountLock()
    {
        RegisterProperty("AccountId",
            wtr => wtr.Write(AccountId),
            rdr => AccountId = rdr.ReadInt32()
        );
        RegisterProperty("LockedId",
            wtr => wtr.Write(LockedId),
            rdr => LockedId = rdr.ReadInt32()
        );
    }

    public static AccountLock Read(string key)
    {
        var ret = new AccountLock();
        var split = key.Split('.');
        ret.AccountId = int.Parse(split[1]);
        ret.LockedId = int.Parse(split[2]);
        return ret;
    }

    public static IEnumerable<string> GetIncludes()
    {
        yield break;
    }
    
    public static string BuildKey(int accountId, int lockedId)
    {
        return KEY_BASE + $".{accountId}.{lockedId}";
    }
}
