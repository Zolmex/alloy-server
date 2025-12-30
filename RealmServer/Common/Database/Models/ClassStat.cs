using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class ClassStat
{
    public int Id { get; set; }

    public ushort? ObjectType { get; set; }

    public ushort? BestLevel { get; set; }

    public uint? BestFame { get; set; }

    public int? AccStatsId { get; set; }

    public virtual AccountStat? AccStats { get; set; }
}
