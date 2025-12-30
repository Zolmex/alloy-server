using System;
using System.Collections.Generic;

namespace DbServer.Database;

public partial class DungeonStat
{
    public int Id { get; set; }

    public string? DungeonName { get; set; }

    public ushort? CompletedCount { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
