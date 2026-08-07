namespace Common.Database.Models;

public class CombatStats {
    public long Shots { get; set; }
    public long ShotsHit { get; set; }
    public int LevelUpAssists { get; set; }
    public int PotionsDrank { get; set; }
    public int AbilitiesUsed { get; set; }
    public long DamageTaken { get; set; }
    public long DamageDealt { get; set; }
}