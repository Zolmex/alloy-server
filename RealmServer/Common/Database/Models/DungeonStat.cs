using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class DungeonStat : IDbSerializable
{
    public int Id { get; set; }

    public string? DungeonName { get; set; }

    public ushort? CompletedCount { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(DungeonName!);
        wtr.Write(CompletedCount!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new DungeonStat()
        {
            Id = rdr.ReadInt32(),
            DungeonName =  rdr.ReadUTF(),
            CompletedCount = rdr.ReadUInt16()
        };
    }
}
