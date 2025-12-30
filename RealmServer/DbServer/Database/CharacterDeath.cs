using System;
using System.Collections.Generic;

namespace DbServer.Database;

public partial class CharacterDeath
{
    public int Id { get; set; }

    public DateTime? DeadAt { get; set; }

    public uint? DeathFame { get; set; }

    public int? CharId { get; set; }

    public virtual Character? Char { get; set; }
}
