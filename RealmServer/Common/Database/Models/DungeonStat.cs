using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class DungeonStat
{
    public int Id { get; set; }

    public string? DungeonName { get; set; }

    public ushort? CompletedCount { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(DungeonName ?? "");
        wtr.Write(CompletedCount ?? 0);
    }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public static DungeonStat Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new DungeonStat();
        ret.Id = id;
        ret.DungeonName = rdr.ReadUTF();
        ret.CompletedCount = rdr.ReadUInt16();
        return ret;
    }
}
