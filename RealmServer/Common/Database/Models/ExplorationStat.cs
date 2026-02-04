using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ExplorationStat : DbModel, IDbQueryable
{
    public override string Key => $"explorationStat.{Id}";
    
    public int Id { get; set; }

    public uint? TilesUncovered { get; set; }

    public uint? QuestsCompleted { get; set; }

    public uint? Escapes { get; set; }

    public uint? NearDeathEscapes { get; set; }

    public uint? MinutesActive { get; set; }

    public uint? Teleports { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("TilesUncovered",
            wtr => wtr.Write(TilesUncovered ?? 0),
            rdr => TilesUncovered = rdr.ReadUInt32()
        );
        RegisterProperty("QuestsCompleted",
            wtr => wtr.Write(QuestsCompleted ?? 0),
            rdr => QuestsCompleted = rdr.ReadUInt32()
        );
        RegisterProperty("Escapes",
            wtr => wtr.Write(Escapes ?? 0),
            rdr => Escapes = rdr.ReadUInt32()
        );
        RegisterProperty("NearDeathEscapes",
            wtr => wtr.Write(NearDeathEscapes ?? 0),
            rdr => NearDeathEscapes = rdr.ReadUInt32()
        );
        RegisterProperty("MinutesActive",
            wtr => wtr.Write(MinutesActive ?? 0),
            rdr => Teleports = rdr.ReadUInt32()
        );
        RegisterProperty("Teleports",
            wtr => wtr.Write(Teleports ?? 0),
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
}
