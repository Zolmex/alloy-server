using System.Collections.Generic;
using System.Linq;
using Common.Database.Models;
using Common.Resources.Config;

namespace Common.Utilities;

public static class GameUtils {
    public static string StatIndexToName(int index) {
        switch (index) {
            case 0: return "MaxHitPoints";
            case 1: return "MaxMagicPoints";
            case 2: return "Attack";
            case 3: return "Defense";
            case 4: return "Speed";
            case 5: return "Dexterity";
            case 6: return "HpRegen";
            case 7: return "MpRegen";
        }

        return null;
    }

    public static int StatNameToIndex(string name) {
        switch (name) {
            case "MaxHitPoints": return 0;
            case "MaxMagicPoints": return 1;
            case "Attack": return 2;
            case "Defense": return 3;
            case "Speed": return 4;
            case "Dexterity": return 5;
            case "HpRegen": return 6;
            case "MpRegen": return 7;
        }

        return -1;
    }

    public static int GetNextLevelXp(int level) {
        // TODO: return real value lol
        return level + 10;
    }

    public static int GetStars(ICollection<ClassStats> classStats) {
        var goals = GameConfig.Config.StarGoals;
        var stars = 0;
        foreach (var classStat in classStats)
            for (var i = 0; i < goals.Length; i++)
                if (classStat.BestFame >= goals[i])
                    stars++;
        return stars;
    }
    
    public static int GetNextLevelXPGoal(int level) {
        return (int)(50f + (level - 1f) * 100f * (1f + level / 10f));
    }

    public static int GetNextClassQuestFame(int fame) {
        var goals = GameConfig.Config.StarGoals;
        for (var i = 0; i < goals.Length; i++) {
            if (fame >= goals[i] && i == goals.Length - 1)
                return 0;
            if (fame < goals[i])
                return goals[i];
        }

        return -1;
    }
}