using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountSkin : IDbModel
{
    public string Key => $"accountSkin.{AccountId}.{SkinType}";
    
    public int AccountId { get; set; }

    public int SkinType { get; set; }

    public virtual Account Account { get; set; } = null!;
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(AccountId);
        wtr.Write(SkinType);
    }

    public static AccountSkin Read(NetworkReader rdr)
    {
        var ret = new AccountSkin();
        ret.AccountId = rdr.ReadInt32();
        ret.SkinType = rdr.ReadInt32();
        return ret;
    }
}
