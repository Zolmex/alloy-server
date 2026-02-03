using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ClassStat : IDbModel
{
    public string Key => $"classStat.{Id}";
    
    public int Id { get; set; }

    public ushort? ObjectType { get; set; }

    public ushort? BestLevel { get; set; }

    public uint? BestFame { get; set; }

    public int? AccStatsId { get; set; }

    public virtual AccountStat? AccStats { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(ObjectType ?? 0);
        wtr.Write(BestLevel ?? 0);
        wtr.Write(BestFame ?? 0);
        if (AccStats != null)
            AccStats.Write(wtr);
        else wtr.Write(0);
    }

    public static ClassStat Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new ClassStat();
        ret.Id = id;
        ret.ObjectType = rdr.ReadUInt16();
        ret.BestLevel = rdr.ReadUInt16();
        ret.BestFame = rdr.ReadUInt32();
        ret.AccStats = AccountStat.Read(rdr);
        ret.AccStatsId = ret.AccStats?.Id ?? 0;
        return ret;
    }
}
