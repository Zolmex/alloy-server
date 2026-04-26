using System.Collections.Generic;

namespace Common.Database.Models;

public class ExplorationStat : DbModel, IDbQueryable {
    public const string KEY_BASE = "explorationStat";

    public ExplorationStat() {
        RegisterProperty("Id",
            (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("TilesUncovered",
            (ref wtr) => wtr.Write(TilesUncovered),
            (ref rdr) => TilesUncovered = rdr.ReadUInt32()
        );
        RegisterProperty("QuestsCompleted",
            (ref wtr) => wtr.Write(QuestsCompleted),
            (ref rdr) => QuestsCompleted = rdr.ReadUInt32()
        );
        RegisterProperty("Escapes",
            (ref wtr) => wtr.Write(Escapes),
            (ref rdr) => Escapes = rdr.ReadUInt32()
        );
        RegisterProperty("NearDeathEscapes",
            (ref wtr) => wtr.Write(NearDeathEscapes),
            (ref rdr) => NearDeathEscapes = rdr.ReadUInt32()
        );
        RegisterProperty("MinutesActive",
            (ref wtr) => wtr.Write(MinutesActive),
            (ref rdr) => Teleports = rdr.ReadUInt32()
        );
        RegisterProperty("Teleports",
            (ref wtr) => wtr.Write(Teleports),
            (ref rdr) => Teleports = rdr.ReadUInt32()
        );
    }

    public override string Key => KEY_BASE + $".{Id}";

    public int Id { get; set; }

    public uint TilesUncovered { get; set; }

    public uint QuestsCompleted { get; set; }

    public uint Escapes { get; set; }

    public uint NearDeathEscapes { get; set; }

    public uint MinutesActive { get; set; }

    public uint Teleports { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public static IEnumerable<string> GetIncludes() {
        yield break;
    }

    public static ExplorationStat Read(string key) {
        var ret = new ExplorationStat();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static string BuildKey(int id) {
        return KEY_BASE + $".{id}";
    }
}