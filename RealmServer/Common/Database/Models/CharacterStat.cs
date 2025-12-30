using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterStat
{
    public int Id { get; set; }

    public uint? Hp { get; set; }

    public uint? Mp { get; set; }

    public uint? MaxHp { get; set; }

    public uint? MaxMp { get; set; }

    public uint? Attack { get; set; }

    public uint? Defense { get; set; }

    public uint? Speed { get; set; }

    public uint? Dexterity { get; set; }

    public uint? Vitality { get; set; }

    public uint? Wisdom { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
