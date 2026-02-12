using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ExplorationStat : DbModel, IDbQueryable
{
    public const string KEY_BASE = "explorationStat";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public uint TilesUncovered { get; set; }

    public uint QuestsCompleted { get; set; }

    public uint Escapes { get; set; }

    public uint NearDeathEscapes { get; set; }

    public uint MinutesActive { get; set; }

    public uint Teleports { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public ExplorationStat()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("TilesUncovered",
            wtr => wtr.Write(TilesUncovered),
            rdr => TilesUncovered = rdr.ReadUInt32()
        );
        RegisterProperty("QuestsCompleted",
            wtr => wtr.Write(QuestsCompleted),
            rdr => QuestsCompleted = rdr.ReadUInt32()
        );
        RegisterProperty("Escapes",
            wtr => wtr.Write(Escapes),
            rdr => Escapes = rdr.ReadUInt32()
        );
        RegisterProperty("NearDeathEscapes",
            wtr => wtr.Write(NearDeathEscapes),
            rdr => NearDeathEscapes = rdr.ReadUInt32()
        );
        RegisterProperty("MinutesActive",
            wtr => wtr.Write(MinutesActive),
            rdr => Teleports = rdr.ReadUInt32()
        );
        RegisterProperty("Teleports",
            wtr => wtr.Write(Teleports),
            rdr => Teleports = rdr.ReadUInt32()
        );
    }

    public static ExplorationStat Read(string key)
    {
        var ret = new ExplorationStat();
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
