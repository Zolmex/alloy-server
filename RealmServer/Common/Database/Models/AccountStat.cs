using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountStat : IDbSerializable
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
        wtr.Write(BestCharFame!.Value);
        wtr.Write(CurrentFame!.Value);
        wtr.Write(TotalFame!.Value);
        wtr.Write(CurrentCredits!.Value);
        wtr.Write(TotalCredits!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new AccountStat()
        {
            Id = rdr.ReadInt32(),
            BestCharFame = rdr.ReadUInt32(),
            CurrentFame = rdr.ReadUInt32(),
            TotalFame = rdr.ReadUInt32(),
            CurrentCredits = rdr.ReadUInt32(),
            TotalCredits = rdr.ReadUInt32()
        };
    }
}
