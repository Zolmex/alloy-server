using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ClassStat : DbModel, IDbQueryable
{
    public const string KEY_BASE = "classStat";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public ushort ObjectType { get; set; }

    public ushort BestLevel { get; set; }

    public uint BestFame { get; set; }

    public int AccStatsId { get; set; }

    public virtual AccountStat? AccStats { get; set; }

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("ObjectType",
            wtr => wtr.Write(ObjectType),
            rdr => ObjectType = rdr.ReadUInt16()
        );
        RegisterProperty("BestLevel",
            wtr => wtr.Write(BestLevel),
            rdr => BestLevel = rdr.ReadUInt16()
        );
        RegisterProperty("BestFame",
            wtr => wtr.Write(BestFame),
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

    public static IEnumerable<string> GetIncludes()
    {
        yield break;
    }
    
    public static string BuildKey(int id)
    {
        return KEY_BASE + $".{id}";
    }
}
