using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

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
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)1);
        
        wtr.Write(Id);
        wtr.Write(Shots ?? 0);
        wtr.Write(ShotsHit ?? 0);
        wtr.Write(LevelUpAssists ?? 0);
        wtr.Write(PotionsDrank ?? 0);
        wtr.Write(AbilitiesUsed ?? 0);
        wtr.Write(DamageTaken ?? 0);
        wtr.Write(DamageDealt ?? 0);
    }

    public static CombatStat Read(NetworkReader rdr)
    {
        if (rdr.ReadByte() == 0) // Empty flag
            return null;
        
        var ret = new CombatStat();
        ret.Id = rdr.ReadInt32();
        ret.Shots = rdr.ReadUInt64();
        ret.ShotsHit = rdr.ReadUInt32();
        ret.LevelUpAssists = rdr.ReadUInt32();
        ret.PotionsDrank = rdr.ReadUInt16();
        ret.AbilitiesUsed = rdr.ReadUInt16();
        ret.DamageTaken = rdr.ReadUInt32();
        ret.DamageDealt = rdr.ReadUInt32();
        return ret;
    }
}
