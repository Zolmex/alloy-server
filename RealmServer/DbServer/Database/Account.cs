using System;
using System.Collections.Generic;

namespace DbServer.Database;

public partial class Account
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short? Rank { get; set; }

    public string? GuildName { get; set; }

    public bool? IsAdmin { get; set; }

    public bool? IsBanned { get; set; }

    public DateTime? CreatedAd { get; set; }

    public int? AccStatsId { get; set; }

    public int? LoginId { get; set; }

    public virtual AccountStat? AccStats { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public virtual ICollection<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();

    public virtual Login? Login { get; set; }
}
