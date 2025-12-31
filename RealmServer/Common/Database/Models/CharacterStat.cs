using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterStat : IDbSerializable
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
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(Hp!.Value);
        wtr.Write(Mp!.Value);
        wtr.Write(MaxHp!.Value);
        wtr.Write(MaxMp!.Value);
        wtr.Write(Attack!.Value);
        wtr.Write(Defense!.Value);
        wtr.Write(Speed!.Value);
        wtr.Write(Dexterity!.Value);
        wtr.Write(Vitality!.Value);
        wtr.Write(Wisdom!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new CharacterStat()
        {
            Id = rdr.ReadInt32(),
            Hp = rdr.ReadUInt32(),
            Mp = rdr.ReadUInt32(),
            MaxHp = rdr.ReadUInt32(),
            MaxMp = rdr.ReadUInt32(),
            Attack = rdr.ReadUInt32(),
            Defense = rdr.ReadUInt32(),
            Speed = rdr.ReadUInt32(),
            Dexterity = rdr.ReadUInt32(),
            Vitality = rdr.ReadUInt32(),
            Wisdom = rdr.ReadUInt32()
        };
    }
}
