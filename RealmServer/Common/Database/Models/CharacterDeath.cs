using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterDeath : DbModel, IDbQueryable
{
    public const string KEY_BASE = "characterDeath";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public DateTime? DeadAt { get; set; }

    public uint DeathFame { get; set; }

    public int CharId { get; set; }

    public virtual Character? Char { get; set; }

    public CharacterDeath()
    {
        RegisterProperty("Id",
           (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("DeadAt",
           (ref wtr) => wtr.Write((DeadAt ?? DateTime.MinValue).ToUnixTimestamp()),
            (ref rdr) => DeadAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("DeathFame",
           (ref wtr) => wtr.Write(DeathFame),
            (ref rdr) => DeathFame = rdr.ReadUInt32()
        );
        RegisterProperty("Char",
            (ref wtr) =>
            {
                var hasValue = Char != null;
                wtr.Write(hasValue);
                if (hasValue)
                    Char.WriteProperties(ref wtr);
            },
            (ref rdr) =>
            {
                Char = DbModel.Read<Character>(ref rdr);
                CharId = Char?.Id ?? 0;
            }
        );
    }
    
    public static CharacterDeath Read(string key)
    {
        var ret = new CharacterDeath();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }
    
    public static IEnumerable<string> GetIncludes()
    {
        yield return "Char";
    }
    
    public static string BuildKey(int id)
    {
        return KEY_BASE + $".{id}";
    }
}
