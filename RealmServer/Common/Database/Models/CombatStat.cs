using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CombatStat : DbModel, IDbQueryable
{
    public override string Key => $"combatStat.{Id}";
    
    public int Id { get; set; }

    public ulong? Shots { get; set; }

    public uint? ShotsHit { get; set; }

    public uint? LevelUpAssists { get; set; }

    public ushort? PotionsDrank { get; set; }

    public ushort? AbilitiesUsed { get; set; }

    public uint? DamageTaken { get; set; }

    public uint? DamageDealt { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("Shots",
            wtr => wtr.Write(Shots ?? 0),
            rdr => Shots = rdr.ReadUInt64()
        );
        RegisterProperty("ShotsHit",
            wtr => wtr.Write(ShotsHit ?? 0),
            rdr => ShotsHit = rdr.ReadUInt32()
        );
        RegisterProperty("LevelUpAssists",
            wtr => wtr.Write(LevelUpAssists ?? 0),
            rdr => LevelUpAssists = rdr.ReadUInt16()
        );
        RegisterProperty("PotionsDrank",
            wtr => wtr.Write(PotionsDrank ?? 0),
            rdr => PotionsDrank = rdr.ReadUInt16()
        );
        RegisterProperty("AbilitiesUsed",
            wtr => wtr.Write(AbilitiesUsed ?? 0),
            rdr => AbilitiesUsed = rdr.ReadUInt16()
        );
        RegisterProperty("DamageTaken",
            wtr => wtr.Write(DamageTaken ?? 0),
            rdr => DamageTaken = rdr.ReadUInt32()
        );
        RegisterProperty("DamageDealt",
            wtr => wtr.Write(DamageDealt ?? 0),
            rdr => DamageDealt = rdr.ReadUInt32()
        );
    }

    public static CombatStat Read(string key)
    {
        var ret = new CombatStat();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static IEnumerable<string> GetIncludes()
    {
        yield break;
    }
}
