using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common.Database;
using Common.Database.Models;
using Common.Resources.Config;

namespace Common.Utilities;

public static class ModelUtils {
    extension(Account acc) {
        public XElement ToXml() {
            return new XElement("Account",
                new XElement("AccountId", acc.Id),
                new XElement("Rank", acc.Rank),
                new XElement("Name", acc.Name),
                !string.IsNullOrEmpty(acc.GuildName)
                    ? new XElement("Guild",
                        new XElement("Name", acc.GuildName),
                        new XElement("Rank", acc.GuildRank))
                    : null,
                acc.IsAdmin ? new XElement("Admin") : null,
                acc.Stats.ToXml(acc)
            );
        }

        public XElement ToCharListXml() {
            return new XElement("Chars",
                new XAttribute("nextCharId", acc.NextCharId),
                new XAttribute("maxNumChars", acc.MaxChars),
                new XAttribute("charSlotCost", NewAccountsConfig.Config.CharSlotCost),
                new XElement("OwnedSkins", acc.OwnedSkins.ToCommaSepString(",")),
                acc.ToXml(),
                acc.Characters.Where(c => !c.IsDead && !c.IsDeleted).Select(c => c.ToXml(acc)),
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

    extension(AccountStats stat) {
        public XElement ToXml(Account acc) {
            return new XElement("Stats",
                new XElement("BestCharFame", stat.BestCharFame),
                new XElement("TotalFame", stat.TotalFame),
                new XElement("Fame", stat.CurrentFame),
                new XElement("TotalCredits", stat.TotalCredits),
                new XElement("Credits", stat.CurrentCredits),
                stat.ClassStats.Select(s => s.ToXml())
            );
        }
    }

    extension(Character chr) {
        public XElement ToXml(Account acc) {
            var elements = new List<XElement> {
                new("ObjectType", chr.ObjectType),
                new("Level", chr.Level),
                new("CharFame", chr.CurrentFame),
                new("NextLevelXp", GameUtils.GetNextLevelXp(chr.Level)),
                new("NextClassQuestFame", GameUtils.GetNextClassQuestFame(chr, acc)),
                new("Experience", chr.XpPoints),
                new("CurrentFame", chr.CurrentFame),
                new("Equipment", chr.ItemTypes.ToCommaSepString(",")),
                new("ItemDatas", chr.ItemDatas.ToCommaSepString(",")),
                new("MaxHitPoints", chr.Stats.MaxHp),
                new("HitPoints", chr.Stats.Hp),
                new("MaxMagicPoints", chr.Stats.MaxMp),
                new("MagicPoints", chr.Stats.Mp),
                new("Attack", chr.Stats.Attack),
                new("Defense", chr.Stats.Defense),
                new("Speed", chr.Stats.Speed),
                new("Dexterity", chr.Stats.Dexterity),
                new("Vitality", chr.Stats.Vitality),
                new("Wisdom", chr.Stats.Wisdom),
                new("Tex1", chr.TextureOne),
                new("Tex2", chr.TextureTwo),
                new("Texture", chr.SkinType)
            };
            return new XElement("Char", new XAttribute("id", chr.CharId), elements);
        }
    }

    extension(ClassStats stat) {
        public XElement ToXml() {
            return new XElement("ClassStats",
                new XAttribute("objectType", stat.ObjectType),
                new XElement("BestLevel", stat.BestLevel),
                new XElement("BestFame", stat.BestFame)
            );
        }
    }

    extension(Guild guild) {
        public XElement ToXml() {
            return new XElement("Guild",
                new XAttribute("name", guild.Name),
                new XElement("CurrentFame", guild.CurrentFame),
                DbClient.Accounts.Find(x => x.GuildId == guild.Id).Select(acc =>
                    new XElement("Member",
                        new XElement("Name", acc.Name),
                        new XElement("Rank", acc.GuildRank),
                        new XElement("Fame", acc.Stats.CurrentGuildFame)
                    )
                )
            );
        }
    }
}