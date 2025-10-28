#region

using Common;
using Common.Database;
using GameServer.Game.Chat;
using GameServer.Game.Network.Messaging.Outgoing;
using System;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        public const int LevelCap = 50; //for testing
        public const int XPPerFame = 500;
        public static readonly int[] Stars = [20, 150, 400, 800, 2000];
        private const int FIND_QUEST_COOLDOWN = 5000; // Find new quest every 10 seconds

        public Enemy Quest { get; private set; }

        public int QuestId
        {
            get => Stats.Get<int>(StatType.QuestId);
            set => Stats.Set(StatType.QuestId, value, true);
        }

        public int Experience
        {
            get => Stats.Get<int>(StatType.Experience);
            set => Stats.Set(StatType.Experience, value, true);
        }

        public int Level
        {
            get => Stats.Get<int>(StatType.Level);
            set => Stats.Set(StatType.Level, value);
        }

        public int StatPoints
        {
            get => Stats.Get<int>(StatType.StatPoints);
            set => Stats.Set(StatType.StatPoints, value, true);
        }

        public int CharFame
        {
            get => Stats.Get<int>(StatType.CharFame);
            set => Stats.Set(StatType.CharFame, value, true);
        }

        public int NextLevelXpGoal
        {
            get => Stats.Get<int>(StatType.NextLevelXp);
            set => Stats.Set(StatType.NextLevelXp, value, true);
        }

        public int NextClassQuestFame
        {
            get => Stats.Get<int>(StatType.NextClassQuestFame);
            set => Stats.Set(StatType.NextClassQuestFame, value, true);
        }

        public int NumStars
        {
            get => Stats.Get<int>(StatType.NumStars);
            set => Stats.Set(StatType.NumStars, value);
        }

        private long _findNewQuestTime;

        public void FindNewQuest(RealmTime time)
        {
            if (Quest != null)
            {
                if (Quest.Dead)
                    Quest = null;
                else return;
            }

            if (time.TotalElapsedMs < _findNewQuestTime)
            {
                return;
            }

            _findNewQuestTime = time.TotalElapsedMs + FIND_QUEST_COOLDOWN;

            var minDist = 0f;
            foreach (var en in World.Quests.Values)
            { // Find closest
                var levelDiff = Math.Abs(Level - en.Desc.Level);
                if (Quest != null)
                {
                    var questLevelDiff = Math.Abs(Level - Quest.Desc.Level);
                    if (levelDiff >
                        questLevelDiff) // The level diff has to be same or lower than current selected quest
                        continue;

                    if (levelDiff == questLevelDiff)
                    { // Distance only matters if the level diff is the same, we prioritize lower level difference
                        var dist = this.DistSqr(en); // Find the closest to player
                        if (minDist == 0 || dist < minDist)
                            minDist = dist;
                        else continue;
                    }
                }

                Quest = en; // Don't set the quest id yet!
            }

            if (Quest != null && Quest.Dead)
                Quest = null;
        }

        public bool GainXP(Character en, int baseXp)
        {
            var xp = CalculateXPGain(en, baseXp);

            Experience += xp;
            CharFame += xp / XPPerFame;

            var classStat = User.Account.Stats.GetClassStats(Char.ClassType);
            classStat.BestFame = CharFame > classStat.BestFame ? CharFame : classStat.BestFame;

            var newClassQuestFame = GetNextClassQuestFame(classStat.BestFame);
            if (newClassQuestFame > NextClassQuestFame || (newClassQuestFame == 0 && NextClassQuestFame == 2000))
            {
                Notification.Write(User.Network, Id, "Class QuestId Complete!", 0x00FF00);
                NextClassQuestFame = newClassQuestFame;
            }

            var levelledUp = false;
            while (Experience - NextLevelXpGoal >= 0 && Level < LevelCap)
            {
                levelledUp = true;
                Level++;
                classStat.BestLevel = Level > classStat.BestLevel ? Level : classStat.BestLevel;

                Experience -= NextLevelXpGoal;

                NextLevelXpGoal = GetNextLevelXPGoal(Level);
                StatPoints += 5;
                HP = MaxHP;
                MP = MaxMP;
                MS = MaxMS;

                if (Level == LevelCap)
                {
                    ChatManager.Announce($"{Name} has reached the maximum level!", false, World.Id, false);
                    break;
                }
            }

            return levelledUp;
        }

        private int CalculateXPGain(Character en, int baseXp)
        {
            if (en == null)
                return baseXp;

            var max = NextLevelXpGoal * 0.1f;
            if (Quest == en)
                max = NextLevelXpGoal * 0.5f;

            return (int)Math.Min(baseXp, max);
        }

        public void InitLevel(DbChar @char)
        {
            CharFame = @char.CharFame;
            Experience = @char.Experience;
            var classStat = User.Account.Stats.GetClassStats(@char.ClassType);
            NextClassQuestFame = GetNextClassQuestFame(classStat.BestFame > CharFame ? classStat.BestFame : CharFame);
            NextLevelXpGoal = GetNextLevelXPGoal(Level);
        }

        public static int GetNextLevelXPGoal(int level)
        {
            return (int)(50f + ((level - 1f) * 100f * (1f + (level / 10f))));
        }

        public static int GetNextClassQuestFame(int fame)
        {
            for (var i = 0; i < Stars.Length; i++)
            {
                if (fame >= Stars[i] && i == Stars.Length - 1)
                    return 0;
                if (fame < Stars[i])
                    return Stars[i];
            }

            return -1;
        }

        public int GetStars()
        {
            var stars = 0;
            foreach (var classStat in User.Account.Stats.ClassStats)
                for (var i = 0; i < Stars.Length; i++)
                    if (classStat.BestFame >= Stars[i])
                        stars++;
            return stars;
        }
    }
}