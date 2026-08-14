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
    private const int MaxAccountsPerIp = 3000;
    
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
        Accounts = DbCon.GetCollection<Account>();
        Logins = DbCon.GetCollection<Login>();
        Guilds = DbCon.GetCollection<Guild>();
        Mutes = DbCon.GetCollection<MuteRecord>();
        Bans = DbCon.GetCollection<BanRecord>();

        DbWriter<Account>.Init();
        DbWriter<Login>.Init();
        DbWriter<Guild>.Init();
        DbWriter<MuteRecord>.Init();
        DbWriter<BanRecord>.Init();
        
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

    public static async Task FlushAsync<T>(T model) where T : class {
        await DbWriter<T>.WriteAsync(model);
    }
    
    public static async Task Dispose() {
        await DbWriter<Account>.StopAsync();
        await DbWriter<Login>.StopAsync();
        await DbWriter<Guild>.StopAsync();
        await DbWriter<MuteRecord>.StopAsync();
        await DbWriter<BanRecord>.StopAsync();
        DbCon.Dispose();
    }
    
    public static bool IsValidUsername(string name) {
        return !string.IsNullOrWhiteSpace(name) && name.Length > 0 && name.Length < 11 && name.All(char.IsLetter);
    }

    public static bool IsValidPassword(string password) {
        return !string.IsNullOrWhiteSpace(password) && password.Length > 8;
    }
    
    public static async Task<RegisterStatus> RegisterAsync(string username, string password, string ip) {
        if (!IsValidUsername(username))
            return RegisterStatus.InvalidName;
        if (!IsValidPassword(password))
            return RegisterStatus.InvalidPassword;

        var status = RegisterStatus.Success;

        var lowerName = username.ToLower();

        // Check name in use
        if (Logins.Exists(i => i.Name.Equals(lowerName)))
            status = RegisterStatus.NameInUse;

        // Check accounts per ip
        else if (Logins.Count(i => i.IpAddress == ip) >= MaxAccountsPerIp)
            status = RegisterStatus.MaxAccountsReached;

        if (status == RegisterStatus.Success) {
            // Used for password encryption
            var salt = MathUtils.GenerateSalt();

            var acc = new Account {
                Name = username,
                MaxChars = NewAccountsConfig.Config.MaxChars,
                VaultCount = NewAccountsConfig.Config.VaultCount,
                Stats = new AccountStats {
                    CurrentCredits = NewAccountsConfig.Config.Credits,
                    TotalCredits = NewAccountsConfig.Config.Credits,
                    CurrentFame = NewAccountsConfig.Config.Fame,
                    TotalFame = NewAccountsConfig.Config.Fame,
                    ClassStats = NewAccountsConfig.CreateClassStats()
                }
            };
            var login = new Login {
                Name = lowerName, IpAddress = ip, PasswordHash = (password + salt).ToSHA1(),
                PasswordSalt = salt
            };

            await FlushAsync(acc);
            await FlushAsync(login);
        }

        return status;
    }
    
    public static (Account Acc, VerifyStatus Status) VerifyAccount(string username, string password, Guid gameServerGuid) {
        var status = VerifyStatus.Success;

        var login = Logins.FindOne(l => l.Name == username);
        if (login == null) {
            status = VerifyStatus.InvalidCredentials;
            return (null, status);
        }

        var hash = (password + login.PasswordSalt).ToSHA1();
        if (login.PasswordHash != hash) {
            status = VerifyStatus.InvalidCredentials;
            return (null, status);
        }

        var acc = Accounts.FindOne(acc => acc.Name == login.Name);
        if (acc == null) {
            status = VerifyStatus.InternalError;
            return (null, status);
        }

        if (acc.LockOwner != gameServerGuid) {
            if (acc.LockOwner != Guid.Empty) {
                status = VerifyStatus.AccountInUse;
                return (null, status);
            }
            
            // Lock account to the specified GameServer instance
            acc.LockOwner = gameServerGuid;
        }

        return (acc, status);
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
            await FlushAsync(acc);
        }

        return (chr, status);
    }
    
    public static async Task<bool> DeleteCharacterAsync(int accId, int charId) {
        var success = true;

        var acc = Accounts.FindById(accId);
        if (acc == null || charId < 0 || charId >= acc.NextCharId) {
            success = false;
        }
        else {
            var chr = acc.Characters[charId];
            if (chr == null) {
                success = false;
            }
            else {
                // Perform a "soft" delete, doesn't actually delete from database, instead we mark it as deleted
                chr.IsDeleted = true;
                await FlushAsync(chr);
            }
        }

        return success;
    }
    
    public static async Task<BuyStatus> BuyCharSlotAsync(Account acc) {
        var cost = NewAccountsConfig.Config.CharSlotCost;
        if (acc.Stats.CurrentFame < cost)
            return BuyStatus.NotEnoughFame;

        acc.Stats.CurrentFame -= cost;
        acc.MaxChars++;

        await FlushAsync(acc);
        return BuyStatus.Success;
    }
    
    public static Character GetCharacter(int accId, int charId) {
        var acc = Accounts.FindById(accId);
        if (acc == null)
            return null;

        if (charId < 0 || charId >= acc.NextCharId)
            return null;
        
        return acc.Characters[charId];
    }
}