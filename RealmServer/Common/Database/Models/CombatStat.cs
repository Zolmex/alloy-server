using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CombatStat : IDbSerializable
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
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(Shots!.Value);
        wtr.Write(ShotsHit!.Value);
        wtr.Write(LevelUpAssists!.Value);
        wtr.Write(PotionsDrank!.Value);
        wtr.Write(AbilitiesUsed!.Value);
        wtr.Write(DamageTaken!.Value);
        wtr.Write(DamageDealt!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new CombatStat()
        {
            Id = rdr.ReadInt32(),
            Shots = rdr.ReadUInt64(),
            ShotsHit = rdr.ReadUInt32(),
            LevelUpAssists = rdr.ReadUInt32(),
            PotionsDrank = rdr.ReadUInt16(),
            AbilitiesUsed = rdr.ReadUInt16(),
            DamageTaken = rdr.ReadUInt32(),
            DamageDealt = rdr.ReadUInt32()
        };
    }
}
