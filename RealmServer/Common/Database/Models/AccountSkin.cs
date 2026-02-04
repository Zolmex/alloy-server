using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountSkin : DbModel, IDbQueryable
{
    public override string Key => $"accountSkin.{AccountId}.{SkinType}";
    
    public int AccountId { get; set; }

    public int SkinType { get; set; }

    public virtual Account Account { get; set; } = null!;
    
    protected override void Prepare()
    {
        RegisterProperty("AccountId",
            wtr => wtr.Write(AccountId),
            rdr => AccountId = rdr.ReadInt32()
        );
        RegisterProperty("SkinType",
            wtr => wtr.Write(SkinType),
            rdr => SkinType = rdr.ReadInt32()
        );
    }

    public static AccountSkin Read(string key)
    {
        var ret = new AccountSkin();
        var split = key.Split('.');
        ret.AccountId = int.Parse(split[1]);
        ret.SkinType = int.Parse(split[2]);
        return ret;
    }

    public static IEnumerable<string> GetIncludes()
    {
        yield break;
    }
}
