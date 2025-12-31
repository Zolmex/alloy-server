using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class GuildMember : IDbSerializable
{
    public int GuildId { get; set; }

    public int AccountId { get; set; }

    public short? GuildRank { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Guild Guild { get; set; } = null!;
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(GuildId);
        wtr.Write(AccountId);
        wtr.Write(GuildRank!.Value);
        wtr.Write(LastSeenAt!.Value.ToUnixTimestamp());
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new GuildMember()
        {
            GuildId = rdr.ReadInt32(),
            AccountId = rdr.ReadInt32(),
            GuildRank = rdr.ReadInt16(),
            LastSeenAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        };
    }
}
