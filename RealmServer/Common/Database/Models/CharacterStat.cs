using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterStat : IDbModel
{
    public string Key => $"characterStat.{Id}";
    
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
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(Hp ?? 0);
        wtr.Write(Mp ?? 0);
        wtr.Write(MaxHp ?? 0);
        wtr.Write(MaxMp ?? 0);
        wtr.Write(Attack ?? 0);
        wtr.Write(Defense ?? 0);
        wtr.Write(Speed ?? 0);
        wtr.Write(Dexterity ?? 0);
        wtr.Write(Vitality ?? 0);
        wtr.Write(Wisdom ?? 0);
    }

    public static CharacterStat Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new CharacterStat();
        ret.Id = id;
        ret.Hp = rdr.ReadUInt32();
        ret.Mp = rdr.ReadUInt32();
        ret.MaxHp = rdr.ReadUInt32();
        ret.MaxMp = rdr.ReadUInt32();
        ret.Attack = rdr.ReadUInt32();
        ret.Defense = rdr.ReadUInt32();
        ret.Speed = rdr.ReadUInt32();
        ret.Dexterity = rdr.ReadUInt32();
        ret.Vitality = rdr.ReadUInt32();
        ret.Wisdom = rdr.ReadUInt32();
        return ret;
    }
}
