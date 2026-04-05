using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CombatStat : DbModel, IDbQueryable
{
    public const string KEY_BASE = "combatStat";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public ulong Shots { get; set; }

    public uint ShotsHit { get; set; }

    public uint LevelUpAssists { get; set; }

    public ushort PotionsDrank { get; set; }

    public ushort AbilitiesUsed { get; set; }

    public uint DamageTaken { get; set; }

    public uint DamageDealt { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public CombatStat()
    {
        RegisterProperty("Id",
           (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("Shots",
           (ref wtr) => wtr.Write(Shots),
            (ref rdr) => Shots = rdr.ReadUInt64()
        );
        RegisterProperty("ShotsHit",
           (ref wtr) => wtr.Write(ShotsHit),
            (ref rdr) => ShotsHit = rdr.ReadUInt32()
        );
        RegisterProperty("LevelUpAssists",
           (ref wtr) => wtr.Write(LevelUpAssists),
            (ref rdr) => LevelUpAssists = rdr.ReadUInt32()
        );
        RegisterProperty("PotionsDrank",
           (ref wtr) => wtr.Write(PotionsDrank),
            (ref rdr) => PotionsDrank = rdr.ReadUInt16()
        );
        RegisterProperty("AbilitiesUsed",
           (ref wtr) => wtr.Write(AbilitiesUsed),
            (ref rdr) => AbilitiesUsed = rdr.ReadUInt16()
        );
        RegisterProperty("DamageTaken",
           (ref wtr) => wtr.Write(DamageTaken),
            (ref rdr) => DamageTaken = rdr.ReadUInt32()
        );
        RegisterProperty("DamageDealt",
           (ref wtr) => wtr.Write(DamageDealt),
            (ref rdr) => DamageDealt = rdr.ReadUInt32()
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
    
    public static string BuildKey(int id)
    {
        return KEY_BASE + $".{id}";
    }
}
