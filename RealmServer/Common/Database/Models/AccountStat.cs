using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountStat
{
    public int Id { get; set; }

    public uint? BestCharFame { get; set; }

    public uint? CurrentFame { get; set; }

    public uint? TotalFame { get; set; }

    public uint? CurrentCredits { get; set; }

    public uint? TotalCredits { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<ClassStat> ClassStats { get; set; } = new List<ClassStat>();
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(BestCharFame ?? 0);
        wtr.Write(CurrentFame ?? 0);
        wtr.Write(TotalFame ?? 0);
        wtr.Write(CurrentCredits ?? 0);
        wtr.Write(TotalCredits ?? 0);
    }

    public static AccountStat Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;

        var ret = new AccountStat();
        ret.Id = id;
        ret.BestCharFame = rdr.ReadUInt32();
        ret.CurrentFame = rdr.ReadUInt32();
        ret.TotalFame = rdr.ReadUInt32();
        ret.CurrentCredits = rdr.ReadUInt32();
        ret.TotalCredits = rdr.ReadUInt32();
        return ret;
    }
}
