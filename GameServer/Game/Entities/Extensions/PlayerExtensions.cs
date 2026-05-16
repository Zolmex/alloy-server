using Common;
using Common.Database.Models;
using Common.Resources.Config;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Extensions;

public static class PlayerExtensions {
    extension(ref Entity player) {
        public void Init(World world, Account acc, Character chr) {
            ref var stats = ref world.EntityStats.Get(player.Id);
            ref var inv = ref world.EntityInventories.Get(player.Id);
            Entity.InitPlayerStats(ref stats, acc, chr);
            Entity.InitPlayerInventory(ref inv, acc, chr);
        }

        private static void InitPlayerStats(ref EntityStats stats, Account acc, Character chr) {
            stats.Set(StatType.Name, acc.Name);
            stats.Set(StatType.Fame, acc.AccStats.CurrentFame);
            stats.Set(StatType.Credits, acc.AccStats.CurrentCredits);
            stats.Set(StatType.GuildName, acc.GuildName);
            stats.Set(StatType.GuildRank, acc.GuildMember?.GuildRank ?? 0);
            stats.Set(StatType.NumStars, GetStars(acc.AccStats.ClassStats));
            stats.Set(StatType.AccRank, acc.Rank);
            Entity.LoadCharacterStats(ref stats, acc, chr);
        }

        private static void LoadCharacterStats(ref EntityStats entityStats, Account acc, Character chr) {
            entityStats.Set(StatType.Level, chr.Level);
            entityStats.Set(StatType.CharFame, (int)chr.CurrentFame);
            entityStats.Set(StatType.Experience, (int)chr.XpPoints);
            var classStat = acc.AccStats.ClassStats.FirstOrDefault(i => i.ObjectType == chr.ObjectType);
            entityStats.Set(StatType.NextClassQuestFame, GetNextClassQuestFame((int)(classStat.BestFame > chr.CurrentFame ? classStat.BestFame : chr.CurrentFame)));
            entityStats.Set(StatType.NextLevelXp, GetNextLevelXPGoal(chr.Level));
            entityStats.Set(StatType.HealthPotionStack, chr.HealthPotions);
            entityStats.Set(StatType.MagicPotionStack, chr.MagicPotions);
            
            if (chr.CharStats != null) {
                entityStats.Set(StatType.MaxHP, (int)chr.CharStats.MaxHp);
                entityStats.Set(StatType.HP, (int)chr.CharStats.Hp);
                entityStats.Set(StatType.MaxMP, (int)chr.CharStats.MaxMp);
                entityStats.Set(StatType.MP, (int)chr.CharStats.Mp);
                entityStats.Set(StatType.Attack, (int)chr.CharStats.Attack);
                entityStats.Set(StatType.Defense, (int)chr.CharStats.Defense);
                entityStats.Set(StatType.Speed, (int)chr.CharStats.Speed);
                entityStats.Set(StatType.Dexterity, (int)chr.CharStats.Dexterity);
                entityStats.Set(StatType.Vitality, (int)chr.CharStats.Vitality);
                entityStats.Set(StatType.Wisdom, (int)chr.CharStats.Wisdom);
            }
        }

        private static void InitPlayerInventory(ref EntityInventory inv, Account acc, Character chr) {
            foreach (var slot in chr.CharacterInventories) {
                var itemData = slot.ItemData != null ? new Item(XmlLibrary.ItemDescs[slot.ItemType].Root) : null;
                inv.SetItem(slot.SlotId, itemData);
            }
        }
        
        public void MoveToSpawn(World world) {
            var spawnTile = world.Map.Data.Regions[TileRegion.Spawn].RandomElement();
            player.Move(world, spawnTile.X, spawnTile.Y);
        }
        
        public void Speak(World world, string text) {
            ref var stats = ref world.EntityStats.Get(player.Id);
            ref var chat = ref world.PlayerChat.Get(player.Id);

            if (!chat.ValidateSpeak(GameLogic.WorldTime, text))
                return;
            
            if (text.StartsWith('/')) {
                ExecuteCommand(world.PlayerToUser[player.Id], text);
                return;
            }

            world.PlayerText(text);
            foreach (var otherUser in world.PlayerToUser.Values) {
                otherUser.SendPacket(new Text(
                    stats.GetString(StatType.Name),
                    player.Id,
                    stats.GetInt(StatType.NumStars),
                    5,
                    null,
                    text
                ));
            }
        }
    }
    
    public static void ExecuteCommand(User user, string text) {
        var spaceIndex = text.IndexOf(' ');
        var command = text.Substring(0, spaceIndex == -1 ? text.Length : spaceIndex);
        var args = spaceIndex == -1 ? null : text.Substring(spaceIndex + 1);
        CommandManager.ExecuteCommand(user, command, args);
    }
    
    public static int GetStars(ICollection<ClassStat> classStats) {
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