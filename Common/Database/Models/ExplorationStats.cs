namespace Common.Database.Models;

public class ExplorationStats {
    public int TilesUncovered { get; set; }
    public int QuestsCompleted { get; set; }
    public int Escapes { get; set; }
    public int NearDeathEscapes { get; set; }
    public int MinutesActive { get; set; }
    public int Teleports { get; set; }
}