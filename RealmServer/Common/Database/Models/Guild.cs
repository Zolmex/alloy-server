using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Guild
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public short? Level { get; set; }

    public uint? CurrentFame { get; set; }

    public uint? TotalFame { get; set; }

    public string? GuildBoard { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(Name ?? "");
        wtr.Write(Level ?? 0);
        wtr.Write(CurrentFame ?? 0);
        wtr.Write(TotalFame ?? 0);
        wtr.Write(GuildBoard ?? "");
        wtr.Write((CreatedAt ?? DateTime.MinValue).ToUnixTimestamp());
    }

    public static Guild Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new Guild();
        ret.Id = id;
        ret.Name = rdr.ReadUTF();
        ret.Level = rdr.ReadInt16();
        ret.CurrentFame = rdr.ReadUInt32();
        ret.TotalFame = rdr.ReadUInt32();
        ret.GuildBoard = rdr.ReadUTF();
        ret.CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        return ret;
    }
}
