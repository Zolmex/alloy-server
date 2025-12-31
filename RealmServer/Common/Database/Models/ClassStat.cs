using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ClassStat : IDbSerializable
{
    public int Id { get; set; }

    public ushort? ObjectType { get; set; }

    public ushort? BestLevel { get; set; }

    public uint? BestFame { get; set; }

    public int? AccStatsId { get; set; }

    public virtual AccountStat? AccStats { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(ObjectType!.Value);
        wtr.Write(BestLevel!.Value);
        wtr.Write(BestFame!.Value);
        wtr.Write(AccStatsId!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new ClassStat()
        {
            Id = rdr.ReadInt32(),
            ObjectType = rdr.ReadUInt16(),
            BestLevel = rdr.ReadUInt16(),
            BestFame = rdr.ReadUInt32(),
            AccStatsId = rdr.ReadInt32()
        };
    }
}
