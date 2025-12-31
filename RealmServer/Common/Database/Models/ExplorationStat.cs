using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ExplorationStat : IDbSerializable
{
    public int Id { get; set; }

    public uint? TilesUncovered { get; set; }

    public uint? QuestsCompleted { get; set; }

    public uint? Escapes { get; set; }

    public uint? NearDeathEscapes { get; set; }

    public uint? MinutesActive { get; set; }

    public uint? Teleports { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(TilesUncovered!.Value);
        wtr.Write(QuestsCompleted!.Value);
        wtr.Write(Escapes!.Value);
        wtr.Write(NearDeathEscapes!.Value);
        wtr.Write(MinutesActive!.Value);
        wtr.Write(Teleports!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new ExplorationStat()
        {
            Id = rdr.ReadInt32(),
            TilesUncovered = rdr.ReadUInt32(),
            QuestsCompleted = rdr.ReadUInt32(),
            Escapes = rdr.ReadUInt32(),
            NearDeathEscapes = rdr.ReadUInt32(),
            MinutesActive = rdr.ReadUInt32(),
            Teleports = rdr.ReadUInt32()
        };
    }
}
