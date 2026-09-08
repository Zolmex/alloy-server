using Arch.Core;
using Common;
using Common.Database.Models;
using Common.Resources.Config;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Entities.Components;
using ArchWorld = Arch.Core.World;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Extensions;

public static class PlayerExtensions {
    extension(Entity player) {
        public void InitPlayer(World world, Account acc, Character chr) {
            ref var stats = ref world.Ecs.Get<Stats>(player);
            ref var inv = ref world.Ecs.Get<Inventory>(player);
            Entity.InitPlayerStats(ref stats, acc, chr);
            Entity.InitPlayerInventory(ref inv, acc, chr);
        }

        private static void InitPlayerStats(ref Stats stats, Account acc, Character chr) {
            stats.Set(StatType.Name, acc.Name);
            stats.Set(StatType.Fame, acc.Stats.CurrentFame);
            stats.Set(StatType.Credits, acc.Stats.CurrentCredits);
            stats.Set(StatType.GuildName, acc.GuildName);
            stats.Set(StatType.GuildRank, acc.GuildRank);
            stats.Set(StatType.NumStars, GetStars(acc.Stats.ClassStats));
            stats.Set(StatType.AccRank, acc.Rank);
            Entity.LoadCharacterStats(ref stats, acc, chr);
        }

        private static void LoadCharacterStats(ref Stats entityStats, Account acc, Character chr) {
            entityStats.Set(StatType.Level, chr.Level);
            entityStats.Set(StatType.CharFame, (int)chr.CurrentFame);
            entityStats.Set(StatType.Experience, (int)chr.XpPoints);
            var classStat = acc.Stats.ClassStats.FirstOrDefault(i => i.ObjectType == chr.ObjectType);
            entityStats.Set(StatType.NextClassQuestFame,
                GetNextClassQuestFame(
                    (int)(classStat.BestFame > chr.CurrentFame ? classStat.BestFame : chr.CurrentFame)));
            entityStats.Set(StatType.NextLevelXp, GetNextLevelXPGoal(chr.Level));
            entityStats.Set(StatType.HealthPotionStack, chr.HealthPotions);
            entityStats.Set(StatType.MagicPotionStack, chr.MagicPotions);

            if (chr.Stats != null) {
                entityStats.Set(StatType.MaxHP, (int)chr.Stats.MaxHp);
                entityStats.Set(StatType.HP, (int)chr.Stats.Hp);
                entityStats.Set(StatType.MaxMP, (int)chr.Stats.MaxMp);
                entityStats.Set(StatType.MP, (int)chr.Stats.Mp);
                entityStats.Set(StatType.Attack, (int)chr.Stats.Attack);
                entityStats.Set(StatType.Defense, (int)chr.Stats.Defense);
                entityStats.Set(StatType.Speed, (int)chr.Stats.Speed);
                entityStats.Set(StatType.Dexterity, (int)chr.Stats.Dexterity);
                entityStats.Set(StatType.Vitality, (int)chr.Stats.Vitality);
                entityStats.Set(StatType.Wisdom, (int)chr.Stats.Wisdom);
            }
        }

        private static void InitPlayerInventory(ref Inventory inv, Account acc, Character chr) {
            using var itemDatas = new MemoryStream(chr.ItemDatas);
            using var rdr = new BinaryReader(itemDatas);
            for (var i = 0; i < chr.ItemTypes.Length; i++) {
                var itemType = chr.ItemTypes[i];
                if (itemType == -1) {
                    inv.SetItem(i, null);
                    continue;
                }

                var item = new Item(XmlLibrary.ItemDescs[(ushort)itemType].Root);
                item.Import(rdr);
                inv.SetItem(i, item);
            }
        }

        public void MoveToSpawn(World world) {
            var spawnTile = world.Map.Data.Regions[TileRegion.Spawn].RandomElement();
            player.Move(world, spawnTile.X, spawnTile.Y);
        }
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