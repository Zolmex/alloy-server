using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class DungeonStat : DbModel, IDbQueryable
{
    public override string Key => $"dungeonStat.{Id}";
    
    public int Id { get; set; }

    public string? DungeonName { get; set; }

    public ushort CompletedCount { get; set; }
    
    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("DungeonName",
            wtr => wtr.Write(DungeonName ?? ""),
            rdr => DungeonName = rdr.ReadUTF()
        );
        RegisterProperty("CompletedCount",
            wtr => wtr.Write(CompletedCount),
            rdr => CompletedCount = rdr.ReadUInt16()
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
}
