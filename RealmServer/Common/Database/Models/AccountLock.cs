using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountLock
{
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public int LockedId { get; set; }
    public Account Locked { get; set; } = null!;
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(AccountId);
        wtr.Write(LockedId);
    }

    public static AccountLock Read(NetworkReader rdr)
    {
        var ret = new AccountLock();
        ret.AccountId = rdr.ReadInt32();
        ret.LockedId = rdr.ReadInt32();
        return ret;
    }
}
