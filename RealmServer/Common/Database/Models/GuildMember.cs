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
        wtr.Write(Id);
        wtr.Write(GuildRank ?? 0);
        wtr.Write((LastSeenAt ?? DateTime.MinValue).ToUnixTimestamp());
        if (Guild != null)
            Guild.Write(wtr);
        else wtr.Write(0);
    }

    public static GuildMember Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;

        var ret = new GuildMember();
        ret.Id = id;
        ret.GuildRank = rdr.ReadInt16();
        ret.LastSeenAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ret.Guild = Guild.Read(rdr);
        ret.GuildId = ret.Guild?.Id ?? 0;
        return ret;
    }
}
