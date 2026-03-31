using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterStat : DbModel, IDbQueryable
{
    public const string KEY_BASE = "characterStat";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public uint Hp { get; set; }

    public uint Mp { get; set; }

    public uint MaxHp { get; set; }

    public uint MaxMp { get; set; }

    public uint Attack { get; set; }

    public uint Defense { get; set; }

    public uint Speed { get; set; }

    public uint Dexterity { get; set; }

    public uint Vitality { get; set; }

    public uint Wisdom { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public CharacterStat()
    {
        RegisterProperty("Id",
           (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("Hp",
           (ref wtr) => wtr.Write(Hp),
            (ref rdr) => Hp = rdr.ReadUInt32()
        );
        RegisterProperty("Mp",
           (ref wtr) => wtr.Write(Mp),
            (ref rdr) => Mp = rdr.ReadUInt32()
        );
        RegisterProperty("MaxHp",
           (ref wtr) => wtr.Write(MaxHp),
            (ref rdr) => MaxHp = rdr.ReadUInt32()
        );
        RegisterProperty("MaxMp",
           (ref wtr) => wtr.Write(MaxMp),
            (ref rdr) => MaxMp = rdr.ReadUInt32()
        );
        RegisterProperty("Attack",
           (ref wtr) => wtr.Write(Attack),
            (ref rdr) => Attack = rdr.ReadUInt32()
        );
        RegisterProperty("Defense",
           (ref wtr) => wtr.Write(Defense),
            (ref rdr) => Defense = rdr.ReadUInt32()
        );
        RegisterProperty("Speed",
           (ref wtr) => wtr.Write(Speed),
            (ref rdr) => Speed = rdr.ReadUInt32()
        );
        RegisterProperty("Dexterity",
           (ref wtr) => wtr.Write(Dexterity),
            (ref rdr) => Dexterity = rdr.ReadUInt32()
        );
        RegisterProperty("Vitality",
           (ref wtr) => wtr.Write(Vitality),
            (ref rdr) => Vitality = rdr.ReadUInt32()
        );
        RegisterProperty("Wisdom",
           (ref wtr) => wtr.Write(Wisdom),
            (ref rdr) => Wisdom = rdr.ReadUInt32()
        );
    }

    public static CharacterStat Read(string key)
    {
        var ret = new CharacterStat();
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
