using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ClassStat
{
    public int Id { get; set; }

    public ushort? ObjectType { get; set; }

    public ushort? BestLevel { get; set; }

    public uint? BestFame { get; set; }

    public int? AccStatsId { get; set; }

    public virtual AccountStat? AccStats { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)1);

        wtr.Write(Id);
        wtr.Write(ObjectType ?? 0);
        wtr.Write(BestLevel ?? 0);
        wtr.Write(BestFame ?? 0);
        wtr.Write(AccStatsId ?? 0);
    }

    public static ClassStat Read(NetworkReader rdr)
    {
        if (rdr.ReadByte() == 0) // Empty flag
            return null;
        
        var ret = new ClassStat();
        ret.Id = rdr.ReadInt32();
        ret.ObjectType = rdr.ReadUInt16();
        ret.BestLevel = rdr.ReadUInt16();
        ret.BestFame = rdr.ReadUInt32();
        ret.AccStatsId = rdr.ReadInt32();
        return ret;
    }
}
