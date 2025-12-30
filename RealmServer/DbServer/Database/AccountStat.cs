using System;
using System.Collections.Generic;

namespace DbServer.Database;

public partial class AccountStat
{
    public int Id { get; set; }

    public uint? BestCharFame { get; set; }

    public uint? CurrentFame { get; set; }

    public uint? TotalFame { get; set; }

    public uint? CurrentCredits { get; set; }

    public uint? TotalCredits { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<ClassStat> ClassStats { get; set; } = new List<ClassStat>();
}
