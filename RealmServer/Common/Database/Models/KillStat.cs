using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class KillStat
{
    public int Id { get; set; }

    public uint? MonsterKills { get; set; }

    public uint? MonsterAssists { get; set; }

    public uint? GodKills { get; set; }

    public uint? GodAssists { get; set; }

    public ushort? OryxKills { get; set; }

    public ushort? OryxAssists { get; set; }

    public ushort? CubeKills { get; set; }

    public ushort? CubeAssists { get; set; }

    public ushort? BlueBags { get; set; }

    public ushort? CyanBags { get; set; }

    public ushort? WhiteBags { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(MonsterKills ?? 0);
        wtr.Write(MonsterAssists ?? 0);
        wtr.Write(GodKills ?? 0);
        wtr.Write(GodAssists ?? 0);
        wtr.Write(OryxKills ?? 0);
        wtr.Write(OryxAssists ?? 0);
        wtr.Write(CubeKills ?? 0);
        wtr.Write(CubeAssists ?? 0);
        wtr.Write(BlueBags ?? 0);
        wtr.Write(CyanBags ?? 0);
        wtr.Write(WhiteBags ?? 0);
    }

    public static KillStat Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new KillStat();
        ret.Id = id;
        ret.MonsterKills = rdr.ReadUInt32();
        ret.MonsterAssists = rdr.ReadUInt32();
        ret.GodKills = rdr.ReadUInt32();
        ret.GodAssists = rdr.ReadUInt32();
        ret.OryxKills = rdr.ReadUInt16();
        ret.OryxAssists = rdr.ReadUInt16();
        ret.CubeKills = rdr.ReadUInt16();
        ret.CubeAssists = rdr.ReadUInt16();
        ret.BlueBags = rdr.ReadUInt16();
        ret.CyanBags = rdr.ReadUInt16();
        ret.WhiteBags = rdr.ReadUInt16();
        return ret;
    }
}
