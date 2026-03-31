using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountStat : DbModel, IDbQueryable
{
    public const string KEY_BASE = "accountStat";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public uint BestCharFame { get; set; }

    public uint CurrentFame { get; set; }

    public uint TotalFame { get; set; }

    public uint CurrentCredits { get; set; }

    public uint TotalCredits { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<ClassStat> ClassStats { get; set; } = new List<ClassStat>();

    public AccountStat()
    {
        RegisterProperty("Id",
           (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("BestCharFame",
           (ref wtr) => wtr.Write(BestCharFame),
            (ref rdr) => BestCharFame = rdr.ReadUInt32()
        );
        RegisterProperty("CurrentFame",
           (ref wtr) => wtr.Write(CurrentFame),
            (ref rdr) => CurrentFame = rdr.ReadUInt32()
        );
        RegisterProperty("TotalFame",
           (ref wtr) => wtr.Write(TotalFame),
            (ref rdr) => TotalFame = rdr.ReadUInt32()
        );
        RegisterProperty("CurrentCredits",
           (ref wtr) => wtr.Write(CurrentCredits),
            (ref rdr) => CurrentCredits = rdr.ReadUInt32()
        );
        RegisterProperty("TotalCredits",
           (ref wtr) => wtr.Write(TotalCredits),
            (ref rdr) => TotalCredits = rdr.ReadUInt32()
        );
        RegisterProperty("ClassStats",
            (ref wtr) =>
            {
                wtr.Write((short)ClassStats.Count);
                foreach (var stat in ClassStats)
                {
                    var hasValue = stat != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        stat.WriteProperties(ref wtr);
                }
            },
            (ref rdr) => {
                ClassStats.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var classStat = DbModel.Read<ClassStat>(ref rdr);
                    if (classStat != null)
                        ClassStats.Add(classStat);
                }
            }
        );
    }

    public static AccountStat Read(ref SpanReader rdr)
    {
        if (!rdr.ReadBoolean())
            return null;
        
        var ret = new AccountStat();
        ret.ReadProperties(ref rdr);
        return ret;
    }
    
    public static IEnumerable<string> GetIncludes()
    {
        yield return "ClassStats";
    }
    
    public static string BuildKey(int id)
    {
        return KEY_BASE + $".{id}";
    }
}
