using System;
using System.Collections.Generic;

namespace DbServer.Database;

public partial class CombatStat
{
    public int Id { get; set; }

    public ulong? Shots { get; set; }

    public uint? ShotsHit { get; set; }

    public uint? LevelUpAssists { get; set; }

    public ushort? PotionsDrank { get; set; }

    public ushort? AbilitiesUsed { get; set; }

    public uint? DamageTaken { get; set; }

    public uint? DamageDealt { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
