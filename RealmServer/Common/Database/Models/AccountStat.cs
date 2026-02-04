using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountStat : DbModel
{
    public override string Key => $"accountStat.{Id}";
    
    public int Id { get; set; }

    public uint? BestCharFame { get; set; }

    public uint? CurrentFame { get; set; }

    public uint? TotalFame { get; set; }

    public uint? CurrentCredits { get; set; }

    public uint? TotalCredits { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<ClassStat> ClassStats { get; set; } = new List<ClassStat>();

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("BestCharFame",
            wtr => wtr.Write(BestCharFame ?? 0),
            rdr => BestCharFame = rdr.ReadUInt32()
        );
        RegisterProperty("CurrentFame",
            wtr => wtr.Write(CurrentFame ?? 0),
            rdr => CurrentFame = rdr.ReadUInt32()
        );
        RegisterProperty("TotalFame",
            wtr => wtr.Write(TotalFame ?? 0),
            rdr => TotalFame = rdr.ReadUInt32()
        );
        RegisterProperty("CurrentCredits",
            wtr => wtr.Write(CurrentCredits ?? 0),
            rdr => CurrentCredits = rdr.ReadUInt32()
        );
        RegisterProperty("TotalCredits",
            wtr => wtr.Write(TotalCredits ?? 0),
            rdr => TotalCredits = rdr.ReadUInt32()
        );
    }

    public static AccountStat Read(NetworkReader rdr)
    {
        if (!rdr.ReadBoolean())
            return null;
        
        var ret = new AccountStat();
        ret.ReadProperties(rdr);
        return ret;
    }
}
