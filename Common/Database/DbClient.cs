using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Database.Models;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using LiteDB;

namespace Common.Database;

public static class DbClient {
    public static ILiteCollection<Account> Accounts;
    public static ILiteCollection<Login> Logins;
    public static ILiteCollection<Guild> Guilds;
    public static ILiteCollection<MuteRecord> Mutes;
    public static ILiteCollection<BanRecord> Bans;
    
    public static LiteDatabase DbCon;
    
    public static void Load(string dbFilePath) {
        var connectionString = new ConnectionString() {
            Filename = dbFilePath,
            Connection = ConnectionType.Shared
        };
        
        DbCon = new LiteDatabase(connectionString);
        Accounts = DbCon.GetCollection<Account>("accounts");
        Logins = DbCon.GetCollection<Login>("logins");
        Guilds = DbCon.GetCollection<Guild>("guilds");
        Mutes = DbCon.GetCollection<MuteRecord>("mutes");
        Bans = DbCon.GetCollection<BanRecord>("bans");

        DbCache<Account>.Init();
        DbCache<Login>.Init();
        DbCache<Guild>.Init();
        DbCache<MuteRecord>.Init();
        DbCache<BanRecord>.Init();
        
        Accounts.EnsureIndex(x => x.Name, true);
        Accounts.EnsureIndex(x => x.GuildId);
        Logins.EnsureIndex(x => x.Name, true);
        Guilds.EnsureIndex(x => x.Name, true);
        Mutes.EnsureIndex(x => x.TargetAccId);
        Mutes.EnsureIndex(x => x.ModeratorAccId);
        Mutes.EnsureIndex(x => x.Reason);
        Bans.EnsureIndex(x => x.TargetAccId);
        Bans.EnsureIndex(x => x.ModeratorAccId);
        Bans.EnsureIndex(x => x.Reason);
    }

    public static void Flush<T>(T model) where T : class {
        DbCache<T>.Enqueue(model);
    }
    
    public static async Task Dispose() {
        await DbCache<Account>.StopAsync();
        await DbCache<Login>.StopAsync();
        await DbCache<Guild>.StopAsync();
        await DbCache<MuteRecord>.StopAsync();
        await DbCache<BanRecord>.StopAsync();
        DbCon.Dispose();
    }
    
    public static bool IsValidUsername(string name) {
        return !string.IsNullOrWhiteSpace(name) && name.Length > 0 && name.Length < 11 && name.All(char.IsLetter);
    }

    public static bool IsValidPassword(string password) {
        return !string.IsNullOrWhiteSpace(password) && password.Length > 8;
    }
    
    public static async Task<(Character Char, CreateCharacterStatus Status)> CreateCharacterAsync(Account acc, ushort objectType, ushort skinType) {
        Character chr = null;
        var status = CreateCharacterStatus.Success;

        if (acc == null) {
            status = CreateCharacterStatus.InternalError;
        }
        else if (acc.Characters.Count >= acc.MaxChars) {
            status = CreateCharacterStatus.MaxCharactersReached;
        }
        else if (skinType != 0 && !acc.OwnedSkins.Contains(skinType)) {
            status = CreateCharacterStatus.SkinNotOwned;
        }
        else // Success, create character here
        {
            var charId = acc.NextCharId;
            var classDesc = XmlLibrary.PlayerDescs[objectType];
            chr = new Character {
                CharId = charId,
                XpPoints = NewCharsConfig.Config.Experience,
                Level = NewCharsConfig.Config.Level,
                ObjectType = objectType,
                ItemTypes = Enumerable.Repeat(-1, 20).ToArray(),
                ItemDatas = Enumerable.Repeat((byte)0, 20).ToArray(),
                TextureOne = (ushort)NewCharsConfig.Config.Tex1,
                TextureTwo = (ushort)NewCharsConfig.Config.Tex2,
                SkinType = skinType,
                HealthPotions = NewCharsConfig.Config.HealthPotions,
                MagicPotions = NewCharsConfig.Config.MagicPotions,
                HasBackpack = NewCharsConfig.Config.HasBackpack,
                Stats = new CharacterStats {
                    Hp = classDesc.Stats[StatType.MaxHP].StartValue,
                    MaxHp = classDesc.Stats[StatType.MaxHP].StartValue,
                    Mp = classDesc.Stats[StatType.MaxMP].StartValue,
                    MaxMp = classDesc.Stats[StatType.MaxMP].StartValue,
                    Attack = classDesc.Stats[StatType.Attack].StartValue,
                    Defense = classDesc.Stats[StatType.Defense].StartValue,
                    Speed = classDesc.Stats[StatType.Speed].StartValue,
                    Dexterity = classDesc.Stats[StatType.Dexterity].StartValue,
                    Vitality = classDesc.Stats[StatType.Vitality].StartValue,
                    Wisdom = classDesc.Stats[StatType.Wisdom].StartValue
                },
                CombatStats = new CombatStats(),
                ExplorationStats = new ExplorationStats(),
                KillStats = new KillStats(),
                DungeonStats = new DungeonStats()
            };
            
            for (var i = 0; i < classDesc.Equipment.Length; i++) {
                var itemType = classDesc.Equipment[i];
                chr.ItemTypes[i] = itemType;
            }

            acc.NextCharId++;
            acc.Characters.Add(chr);
            Flush(acc);
        }

        return (chr, status);
    }
}