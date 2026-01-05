using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class GuildMember
{
    public int Id { get; set; }

    public short? GuildRank { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public int? GuildId { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Guild? Guild { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)1);
        
        wtr.Write(Id);
        wtr.Write(GuildRank ?? 0);
        wtr.Write((LastSeenAt ?? DateTime.MinValue).ToUnixTimestamp());
        wtr.Write(GuildId ?? 0);
        Guild?.Write(wtr);
    }

    public static GuildMember Read(NetworkReader rdr)
    {
        if (rdr.ReadByte() == 0) // Empty flag
            return null;

        var ret = new GuildMember();
        ret.Id = rdr.ReadInt32();
        ret.GuildRank = rdr.ReadInt16();
        ret.LastSeenAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ret.GuildId = rdr.ReadInt32();
        ret.Guild = Guild.Read(rdr);
        return ret;
    }
}
