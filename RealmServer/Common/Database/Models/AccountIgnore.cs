using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountIgnore : DbModel, IDbQueryable
{
    public const string KEY_BASE = "accountIgnore";
    
    public override string Key => KEY_BASE + $".{AccountId}.{IgnoredId}";
    
    public int AccountId { get; set; }
    
    public Account Account { get; set; } = null!;
    
    public int IgnoredId { get; set; }
    
    public Account Ignored { get; set; } = null!;

    public AccountIgnore()
    {
        RegisterProperty("AccountId",
            wtr => wtr.Write(AccountId),
            rdr => AccountId = rdr.ReadInt32()
        );
        RegisterProperty("IgnoredId",
            wtr => wtr.Write(IgnoredId),
            rdr => IgnoredId = rdr.ReadInt32()
        );
    }

    public static AccountIgnore Read(string key)
    {
        var ret = new AccountIgnore();
        var split = key.Split('.');
        ret.AccountId = int.Parse(split[1]);
        ret.IgnoredId = int.Parse(split[2]);
        return ret;
    }

    public static IEnumerable<string> GetIncludes()
    {
        yield break;
    }
    
    public static string BuildKey(int accountId, int ignoredId)
    {
        return KEY_BASE + $".{accountId}.{ignoredId}";
    }
}
