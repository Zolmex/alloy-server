using System.Collections.Generic;

namespace Common.Database.Models;

public class KillStat : DbModel, IDbQueryable {
    public const string KEY_BASE = "killStat";

    public KillStat() {
        RegisterProperty("Id",
            (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("MonsterKills",
            (ref wtr) => wtr.Write(MonsterKills),
            (ref rdr) => MonsterKills = rdr.ReadUInt32()
        );
        RegisterProperty("MonsterAssists",
            (ref wtr) => wtr.Write(MonsterAssists),
            (ref rdr) => MonsterAssists = rdr.ReadUInt32()
        );
        RegisterProperty("GodKills",
            (ref wtr) => wtr.Write(GodKills),
            (ref rdr) => GodKills = rdr.ReadUInt32()
        );
        RegisterProperty("GodAssists",
            (ref wtr) => wtr.Write(GodAssists),
            (ref rdr) => GodAssists = rdr.ReadUInt32()
        );
        RegisterProperty("OryxKills",
            (ref wtr) => wtr.Write(OryxKills),
            (ref rdr) => OryxKills = rdr.ReadUInt16()
        );
        RegisterProperty("OryxAssists",
            (ref wtr) => wtr.Write(OryxAssists),
            (ref rdr) => OryxAssists = rdr.ReadUInt16()
        );
        RegisterProperty("CubeKills",
            (ref wtr) => wtr.Write(CubeKills),
            (ref rdr) => CubeKills = rdr.ReadUInt16()
        );
        RegisterProperty("CubeAssists",
            (ref wtr) => wtr.Write(CubeAssists),
            (ref rdr) => CubeAssists = rdr.ReadUInt16()
        );
        RegisterProperty("BlueBags",
            (ref wtr) => wtr.Write(BlueBags),
            (ref rdr) => BlueBags = rdr.ReadUInt16()
        );
        RegisterProperty("CyanBags",
            (ref wtr) => wtr.Write(CyanBags),
            (ref rdr) => CyanBags = rdr.ReadUInt16()
        );
        RegisterProperty("WhiteBags",
            (ref wtr) => wtr.Write(WhiteBags),
            (ref rdr) => WhiteBags = rdr.ReadUInt16()
        );
    }

    public override string Key => KEY_BASE + $".{Id}";

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

    public static IEnumerable<string> GetIncludes() {
        yield break;
    }

    public static KillStat Read(string key) {
        var ret = new KillStat();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static string BuildKey(int id) {
        return KEY_BASE + $".{id}";
    }
}