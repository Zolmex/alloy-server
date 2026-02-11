using Common.Database;
using Common.Database.Models;
using Common.Resources.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Common.Utilities;

public static class ModelUtils
{
    extension(Account acc)
    {
        public XElement ToXml()
        {
            return new XElement("Account",
                new XElement("AccountId", acc.Id),
                new XElement("Rank", acc.Rank),
                new XElement("Name", acc.Name),
                !string.IsNullOrEmpty(acc.GuildName) ?
                new XElement("Guild",
                    new XElement("Name", acc.GuildName),
                    new XElement("Rank", acc.GuildMember!.GuildRank))
                : null,
                acc.IsAdmin ? new XElement("Admin") : null,
                acc.AccStats.ToXml(acc)
            );
        }
        
        public XElement ToCharListXml()
        {
            return new XElement("Chars",
                new XAttribute("nextCharId", acc.NextCharId!),
                new XAttribute("maxNumChars", acc.MaxChars!),
                new XAttribute("charSlotCost", NewAccountsConfig.Config.CharSlotCost),
                new XElement("OwnedSkins", acc.AccountSkins.ToCommaSepString(",")),
                acc.ToXml(),
                acc.Characters.Select(c => c.ToXml(acc)),
                NewsConfig.Config.Models.Select(n => n.ToXml()),
                new XElement("Servers",
                    new XElement("Server",
                        new XElement("Name", GameServerConfig.Config.ServerName),
                        new XElement("DNS", GameServerConfig.Config.Address),
                        new XElement("Port", GameServerConfig.Config.Port),
                        new XElement("Players", 0),
                        new XElement("MaxPlayers", GameServerConfig.Config.MaxPlayers),
                        new XElement("AdminOnly", GameServerConfig.Config.AdminOnly ? "true" : "false")
                    )
                )
            );
        }
    }

    extension(AccountStat stat)
    {
        public XElement ToXml(Account acc)
        {
            Logger.Debug(acc.AccStats);
            Logger.Debug(acc.AccStats.ClassStats);
            return new XElement("Stats",
                new XElement("BestCharFame", stat.BestCharFame),
                new XElement("TotalFame", stat.TotalFame),
                new XElement("Fame", stat.CurrentFame),
                new XElement("TotalCredits", stat.TotalCredits),
                new XElement("Credits", stat.CurrentCredits),
                new XElement("TotalGuildFame", acc.GuildMember?.Guild!.TotalFame ?? 0),
                new XElement("GuildFame", acc.GuildMember?.Guild!.CurrentFame ?? 0),
                stat.ClassStats.Select(s => s.ToXml())
            );
        }
    }

    extension(Character chr)
    {
        public XElement ToXml(Account acc)
        {
            Logger.Debug(chr);
            Logger.Info(chr.CharStats); // TODO: figure out whats null here bruh
            Logger.Warn(acc);
            foreach (var slot in chr.CharacterInventories)
                Logger.Debug(slot);
            var elements = new List<XElement>
            {
                new("ObjectType", chr.ObjectType),
                new("Level", chr.Level),
                new("CharFame", chr.CurrentFame),
                new("NextLevelXp", GameUtils.GetNextLevelXp(chr.Level)),
                new("NextClassQuestFame", GameUtils.GetNextClassQuestFame(chr, acc)),
                new("Experience", chr.XpPoints),
                new("CurrentFame", chr.CurrentFame),
                new("Equipment", chr.CharacterInventories.Select(slot => slot.ItemType).ToCommaSepString(",")),
                new("ItemDatas", chr.CharacterInventories.Select(slot => slot.ItemData).ToCommaSepString(",")),
                new("MaxHitPoints", chr.CharStats!.MaxHp),
                new("HitPoints", chr.CharStats.Hp),
                new("MaxMagicPoints", chr.CharStats.MaxMp),
                new("MagicPoints", chr.CharStats.Mp),
                new("Attack", chr.CharStats.Attack),
                new("Defense", chr.CharStats.Defense),
                new("Speed", chr.CharStats.Speed),
                new("Dexterity", chr.CharStats.Dexterity),
                new("Vitality", chr.CharStats.Vitality),
                new("Wisdom", chr.CharStats.Wisdom),
                new("Tex1", chr.TextureOne),
                new("Tex2", chr.TextureTwo),
                new("Texture", chr.SkinType)
            };
            return new XElement("Char", new XAttribute("id", chr.AccCharId!), elements);
        }
    }

    extension(ClassStat stat)
    {
        public XElement ToXml()
        {
            return new XElement("ClassStats",
                new XAttribute("objectType", stat.ObjectType!),
                new XElement("BestLevel", stat.BestLevel),
                new XElement("BestFame", stat.BestFame)
            );
        }
    }
}