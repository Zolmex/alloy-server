using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class KillStat : DbModel, IDbQueryable
{
    public override string Key => $"killStat.{Id}";
    
    public int Id { get; set; }

    public uint MonsterKills { get; set; }

    public uint MonsterAssists { get; set; }

    public uint GodKills { get; set; }

    public uint GodAssists { get; set; }

    public ushort OryxKills { get; set; }

    public ushort OryxAssists { get; set; }

    public ushort CubeKills { get; set; }

    public ushort CubeAssists { get; set; }

    public ushort BlueBags { get; set; }

    public ushort CyanBags { get; set; }

    public ushort WhiteBags { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("MonsterKills",
            wtr => wtr.Write(MonsterKills),
            rdr => MonsterKills = rdr.ReadUInt32()
        );
        RegisterProperty("MonsterAssists",
            wtr => wtr.Write(MonsterAssists),
            rdr => MonsterAssists = rdr.ReadUInt32()
        );
        RegisterProperty("GodKills",
            wtr => wtr.Write(GodKills),
            rdr => GodKills = rdr.ReadUInt32()
        );
        RegisterProperty("GodAssists",
            wtr => wtr.Write(GodAssists),
            rdr => GodAssists = rdr.ReadUInt32()
        );
        RegisterProperty("OryxKills",
            wtr => wtr.Write(OryxKills),
            rdr => OryxKills = rdr.ReadUInt16()
        );
        RegisterProperty("OryxAssists",
            wtr => wtr.Write(OryxAssists),
            rdr => OryxAssists = rdr.ReadUInt16()
        );
        RegisterProperty("CubeKills",
            wtr => wtr.Write(CubeKills),
            rdr => CubeKills = rdr.ReadUInt16()
        );
        RegisterProperty("CubeAssists",
            wtr => wtr.Write(CubeAssists),
            rdr => CubeAssists = rdr.ReadUInt16()
        );
        RegisterProperty("BlueBags",
            wtr => wtr.Write(BlueBags),
            rdr => BlueBags = rdr.ReadUInt16()
        );
        RegisterProperty("CyanBags",
            wtr => wtr.Write(CyanBags),
            rdr => CyanBags = rdr.ReadUInt16()
        );
        RegisterProperty("WhiteBags",
            wtr => wtr.Write(WhiteBags),
            rdr => WhiteBags = rdr.ReadUInt16()
        );
    }

    public static KillStat Read(string key)
    {
        var ret = new KillStat();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static IEnumerable<string> GetIncludes()
    {
        yield break;
    }
}
