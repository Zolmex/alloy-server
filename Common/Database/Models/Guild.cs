using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public class Guild {
    public int Id { get; set; }
    public string Name { get; set; }
    public short Level { get; set; }
    public uint CurrentFame { get; set; }
    public uint TotalFame { get; set; }
    public string GuildBoard { get; set; }
    public DateTime CreatedAt { get; set; }
}