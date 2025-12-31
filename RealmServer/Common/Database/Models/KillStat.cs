using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class KillStat : IDbSerializable
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
        wtr.Write(MonsterKills!.Value);
        wtr.Write(MonsterAssists!.Value);
        wtr.Write(GodKills!.Value);
        wtr.Write(GodAssists!.Value);
        wtr.Write(OryxKills!.Value);
        wtr.Write(OryxAssists!.Value);
        wtr.Write(CubeKills!.Value);
        wtr.Write(CubeAssists!.Value);
        wtr.Write(BlueBags!.Value);
        wtr.Write(CyanBags!.Value);
        wtr.Write(WhiteBags!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new KillStat()
        {
            Id = rdr.ReadInt32(),
            MonsterKills = rdr.ReadUInt32(),
            MonsterAssists = rdr.ReadUInt32(),
            GodKills = rdr.ReadUInt32(),
            GodAssists = rdr.ReadUInt32(),
            OryxKills = rdr.ReadUInt16(),
            OryxAssists = rdr.ReadUInt16(),
            CubeKills = rdr.ReadUInt16(),
            CubeAssists = rdr.ReadUInt16(),
            BlueBags = rdr.ReadUInt16(),
            CyanBags = rdr.ReadUInt16(),
            WhiteBags = rdr.ReadUInt16()
        };
    }
}
