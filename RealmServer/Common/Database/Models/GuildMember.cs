using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class GuildMember
{
    public int GuildId { get; set; }

    public int AccountId { get; set; }

    public short? GuildRank { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Guild Guild { get; set; } = null!;
}
