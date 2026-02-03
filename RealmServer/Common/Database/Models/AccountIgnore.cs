using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountIgnore : IDbModel
{
    public string Key => $"accountIgnore.{AccountId}.{IgnoredId}";
    
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public int IgnoredId { get; set; }
    public Account Ignored { get; set; } = null!;
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(AccountId);
        wtr.Write(IgnoredId);
    }

    public static AccountIgnore Read(NetworkReader rdr)
    {
        var ret = new AccountIgnore();
        ret.AccountId = rdr.ReadInt32();
        ret.IgnoredId = rdr.ReadInt32();
        return ret;
    }
}
