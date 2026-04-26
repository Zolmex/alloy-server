using Common;
using Common.Database.Models;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using DbServer.Database;

namespace DbServer.Messaging;

public class CreateCharacterHandler : IMessageHandler {
    public AppMessageId MessageId => AppMessageId.CreateCharacter;

    public async Task HandleAsync(IAppMessage msg, AppConnection con) {
        var pkt = (CreateCharacterMessage)msg;
        var response = new CreateCharacterAck(pkt.Sequence);
        Logger.Debug($"CreateCharacter: {pkt.AccountId}|{pkt.ClassType}|{pkt.SkinType}");

        var status = CreateCharacterStatus.Success;

        var acc = await DbCache.Accounts.GetAsync(Account.BuildKey(pkt.AccountId));
        if (acc == null) {
            status = CreateCharacterStatus.InternalError;
        }
        else if (acc.Characters.Count >= acc.MaxChars) {
            status = CreateCharacterStatus.MaxCharactersReached;
        }
        else if (pkt.SkinType != 0 &&
                 !await DbCache.AccountSkins.AnyAsync(s => s.AccountId == acc.Id && s.SkinType == pkt.SkinType)) {
            status = CreateCharacterStatus.SkinNotOwned;
        }
        else // Success, create character here
        {
            var accCharId = acc.NextCharId;
            var classDesc = XmlLibrary.PlayerDescs[pkt.ClassType];
            var chr = new Character {
                AccCharId = accCharId,
                XpPoints = (uint)NewCharsConfig.Config.Experience,
                Level = (ushort)NewCharsConfig.Config.Level,
                ObjectType = pkt.ClassType,
                CharacterInventories = new List<CharacterInventory>(20), // Fill these with 20 instances
                TextureOne = (ushort)NewCharsConfig.Config.Tex1,
                TextureTwo = (ushort)NewCharsConfig.Config.Tex2,
                SkinType = pkt.SkinType,
                HealthPotions = (ushort)NewCharsConfig.Config.HealthPotions,
                MagicPotions = (ushort)NewCharsConfig.Config.MagicPotions,
                HasBackpack = NewCharsConfig.Config.HasBackpack,
                CharStats = new CharacterStat {
                    Hp = (uint)classDesc.Stats[StatType.MaxHP].StartValue,
                    MaxHp = (uint)classDesc.Stats[StatType.MaxHP].StartValue,
                    Mp = (uint)classDesc.Stats[StatType.MaxMP].StartValue,
                    MaxMp = (uint)classDesc.Stats[StatType.MaxMP].StartValue,
                    Attack = (uint)classDesc.Stats[StatType.Attack].StartValue,
                    Defense = (uint)classDesc.Stats[StatType.Defense].StartValue,
                    Speed = (uint)classDesc.Stats[StatType.Speed].StartValue,
                    Dexterity = (uint)classDesc.Stats[StatType.Dexterity].StartValue,
                    Vitality = (uint)classDesc.Stats[StatType.Vitality].StartValue,
                    Wisdom = (uint)classDesc.Stats[StatType.Wisdom].StartValue
                },
                CombatStats = new CombatStat(),
                ExploStats = new ExplorationStat(),
                KillStats = new KillStat(),
                DungeonStats = new DungeonStat()
            };

            acc.NextCharId++;
            acc.Characters.Add(chr);

            DbCache.Accounts.Update(acc, a => a.NextCharId); // Write changes to cache
            await DbCache.Characters.AddAsync(chr);

            await DbCache.SaveChanges(); // Save changes to database

            response.Character = chr;
        }

        response.Status = status;
        con.Send(response);
    }
}