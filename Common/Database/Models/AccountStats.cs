using System.Collections.Generic;

namespace Common.Database.Models;

public class AccountStats {
    public int BestCharFame { get; set; }
    public int CurrentFame { get; set; }
    public int TotalFame { get; set; }
    public int CurrentCredits { get; set; }
    public int TotalCredits { get; set; }
    public int CurrentGuildFame { get; set; }
    public int TotalGuildFame { get; set; }
    public List<ClassStats> ClassStats { get; set; } = [];
}