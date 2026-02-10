using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CombatStat : DbModel, IDbQueryable
{
    public override string Key => $"combatStat.{Id}";
    
    public int Id { get; set; }

    public ulong Shots { get; set; }

    public uint ShotsHit { get; set; }

    public uint LevelUpAssists { get; set; }

    public ushort PotionsDrank { get; set; }

    public ushort AbilitiesUsed { get; set; }

    public uint DamageTaken { get; set; }

    public uint DamageDealt { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("Shots",
            wtr => wtr.Write(Shots),
            rdr => Shots = rdr.ReadUInt64()
        );
        RegisterProperty("ShotsHit",
            wtr => wtr.Write(ShotsHit),
            rdr => ShotsHit = rdr.ReadUInt32()
        );
        RegisterProperty("LevelUpAssists",
            wtr => wtr.Write(LevelUpAssists),
            rdr => LevelUpAssists = rdr.ReadUInt16()
        );
        RegisterProperty("PotionsDrank",
            wtr => wtr.Write(PotionsDrank),
            rdr => PotionsDrank = rdr.ReadUInt16()
        );
        RegisterProperty("AbilitiesUsed",
            wtr => wtr.Write(AbilitiesUsed),
            rdr => AbilitiesUsed = rdr.ReadUInt16()
        );
        RegisterProperty("DamageTaken",
            wtr => wtr.Write(DamageTaken),
            rdr => DamageTaken = rdr.ReadUInt32()
        );
        RegisterProperty("DamageDealt",
            wtr => wtr.Write(DamageDealt),
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
