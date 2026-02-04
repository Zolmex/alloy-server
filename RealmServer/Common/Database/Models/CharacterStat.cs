using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterStat : DbModel
{
    public override string Key => $"characterStat.{Id}";
    
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

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("Hp",
            wtr => wtr.Write(Hp ?? 0),
            rdr => Hp = rdr.ReadUInt32()
        );
        RegisterProperty("Mp",
            wtr => wtr.Write(Mp ?? 0),
            rdr => Mp = rdr.ReadUInt32()
        );
        RegisterProperty("MaxHp",
            wtr => wtr.Write(MaxHp ?? 0),
            rdr => MaxHp = rdr.ReadUInt32()
        );
        RegisterProperty("MaxMp",
            wtr => wtr.Write(MaxMp ?? 0),
            rdr => MaxMp = rdr.ReadUInt32()
        );
        RegisterProperty("Attack",
            wtr => wtr.Write(Attack ?? 0),
            rdr => Attack = rdr.ReadUInt32()
        );
        RegisterProperty("Defense",
            wtr => wtr.Write(Defense ?? 0),
            rdr => Defense = rdr.ReadUInt32()
        );
        RegisterProperty("Speed",
            wtr => wtr.Write(Speed ?? 0),
            rdr => Speed = rdr.ReadUInt32()
        );
        RegisterProperty("Dexterity",
            wtr => wtr.Write(Dexterity ?? 0),
            rdr => Dexterity = rdr.ReadUInt32()
        );
        RegisterProperty("Vitality",
            wtr => wtr.Write(Vitality ?? 0),
            rdr => Vitality = rdr.ReadUInt32()
        );
        RegisterProperty("Wisdom",
            wtr => wtr.Write(Wisdom ?? 0),
            rdr => Wisdom = rdr.ReadUInt32()
        );
    }

    public static CharacterStat Read(string key)
    {
        var ret = new CharacterStat();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }
}
