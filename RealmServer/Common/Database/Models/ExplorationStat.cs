using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ExplorationStat
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
        wtr.Write(TilesUncovered ?? 0);
        wtr.Write(QuestsCompleted ?? 0);
        wtr.Write(Escapes ?? 0);
        wtr.Write(NearDeathEscapes ?? 0);
        wtr.Write(MinutesActive ?? 0);
        wtr.Write(Teleports ?? 0);
    }

    public static ExplorationStat Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new ExplorationStat();
        ret.Id = id;
        ret.TilesUncovered = rdr.ReadUInt32();
        ret.QuestsCompleted = rdr.ReadUInt32();
        ret.Escapes = rdr.ReadUInt32();
        ret.NearDeathEscapes = rdr.ReadUInt32();
        ret.MinutesActive = rdr.ReadUInt32();
        ret.Teleports = rdr.ReadUInt32();
        return ret;
    }
}
