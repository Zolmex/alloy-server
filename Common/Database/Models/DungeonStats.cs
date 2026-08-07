using System.Collections.Generic;

namespace Common.Database.Models;

public class DungeonStats {
    public Dictionary<string, int> Completions { get; set; } = [];
}
