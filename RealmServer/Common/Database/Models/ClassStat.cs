using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ClassStat : DbModel
{
    public override string Key => $"classStat.{Id}";
    
    public int Id { get; set; }

    public ushort? ObjectType { get; set; }

    public ushort? BestLevel { get; set; }

    public uint? BestFame { get; set; }

    public int? AccStatsId { get; set; }

    public virtual AccountStat? AccStats { get; set; }

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("ObjectType",
            wtr => wtr.Write(ObjectType ?? 0),
            rdr => ObjectType = rdr.ReadUInt16()
        );
        RegisterProperty("BestLevel",
            wtr => wtr.Write(BestLevel ?? 0),
            rdr => BestLevel = rdr.ReadUInt16()
        );
        RegisterProperty("BestFame",
            wtr => wtr.Write(BestFame ?? 0),
            rdr => BestFame = rdr.ReadUInt32()
        );
    }

    public static ClassStat Read(string key)
    {
        var ret = new ClassStat();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }
}
