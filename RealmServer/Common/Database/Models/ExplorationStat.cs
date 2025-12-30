using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ExplorationStat
{
    public int Id { get; set; }

    public uint? TilesUncovered { get; set; }

    public uint? QuestsCompleted { get; set; }

    public uint? Escapes { get; set; }

    public uint? NearDeathEscapes { get; set; }

    public uint? MinutesActive { get; set; }

    public uint? Teleports { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
