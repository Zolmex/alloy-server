using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class DungeonStat : DbModel, IDbQueryable
{
    public const string KEY_BASE = "dungeonStat";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public string? DungeonName { get; set; }

    public ushort CompletedCount { get; set; }
    
    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public DungeonStat()
    {
        RegisterProperty("Id",
           (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("DungeonName",
           (ref wtr) => wtr.WriteUTF(DungeonName ?? ""),
            (ref rdr) => DungeonName = rdr.ReadUTF()
        );
        RegisterProperty("CompletedCount",
           (ref wtr) => wtr.Write(CompletedCount),
            (ref rdr) => CompletedCount = rdr.ReadUInt16()
        );
    }

    public static DungeonStat Read(string key)
    {
        var ret = new DungeonStat();
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
