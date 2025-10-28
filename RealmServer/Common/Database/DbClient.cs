#region

using Common.Control;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

#endregion

namespace Common.Database
{
    public static class DbClient
    {
        private static readonly Logger _log = new(typeof(DbClient));

        private static readonly string[] _guestNames = new string[] { "Shmarq", "Shmeyzt", "Shmrac", "Shmrol", "Shmeango", "Shmeashy", "Shmeati", "Shmeendi", "Shmehoni", "Shmharr", "Shmiatho", "Shmiawa", "Shmidrae", "Shmiri", "Shmissz", "Shmitani", "Shmlaen", "Shmlauk", "Shmlorz", "Shmoalei", "Shmodaru", "Shmoeti", "Shmorothi", "Shmoshyu", "Shmqueq", "Shmradph", "Shmrayr", "Shmrill", "Shmrilr", "Shmrisrr", "Shmsaylt", "Shmscheev", "Shmsek", "Shmserl", "Shmseus", "Shmtal", "Shmtiar", "Shmitty", "Shmuoro", "Shmurake", "Shmutanu", "Shmvorck", "Shmvorv", "Shmyangu", "Shmyimi", "Shmzhiar" };

        private static readonly ConcurrentDictionary<int, DbAccount> _accounts = new();

        private static readonly ConcurrentDictionary<string, DbChar> _characters = new();

        private static readonly ConcurrentDictionary<string, DbDeathInfo> _deaths = new();

        private static readonly ConcurrentDictionary<int, DbGuild> _guilds = new();
        private static readonly ConcurrentDictionary<int, DbParty> _parties = new();

        private static DatabaseConfig _config;
        private static IDatabase _db;

        private static DbLegends _legends;
        private static DbLogins _logins;

        public static async void Connect(DatabaseConfig config)
        {
            _config = config;

            RedisClient.Connect(config);

            _db = RedisClient.Db;

            _legends = new DbLegends();
            await _legends.Load(_db);
            _logins = new DbLogins();
            await _logins.Load(_db);

            // Stuff to keep on wipe
            // It doesn't get added multiple times, so it's safe to call this every time
            await _db.SetAddAsync(DbKeys.KeepOnWipe, DbKeys.KeepOnWipe);
            await _db.SetAddAsync(DbKeys.KeepOnWipe, DbKeys.Legends);
            await _db.SetAddAsync(DbKeys.KeepOnWipe, DbKeys.AccountLogins);
            await _db.SetAddAsync(DbKeys.KeepOnWipe, DbKeys.AccountIPs);
            await _db.SetAddAsync(DbKeys.KeepOnWipe, DbKeys.AccountNames);
            await _db.SetAddAsync(DbKeys.KeepOnWipe, DbKeys.NextAccountId);
            // Specific account keys get added when the account is registered
        }

        public static bool IsValidUsername(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length > 0 && name.Length < 11 && name.All(char.IsLetter);
        }

        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length > 8;
        }

        public static async void OnWipeCompleted(object sender, ControlMessage<bool> message)
        {
            // Message Content for this channel is the direction of the wipe.
            // True = Message when wipe happened.
            // False = Message to trigger a wipe.
            if (!message.Content)
                return;

            var trans = _db.CreateTransaction();

            var totalAccs = (int)await _db.StringGetAsync(DbKeys.NextAccountId);
            for (var i = totalAccs; i > 0; i--)
            {
                var acc = new DbAccount(i);
                await acc.Load(_db);

                // wipe the account
                acc.MaxNumChars = NewAccountsConfig.Config.MaxChars;
                acc.VaultCount = NewAccountsConfig.Config.VaultCount;
                acc.NextCharId = 0;
                acc.NumStars = 0;
                acc.GuildId = 0;
                acc.GuildName = null;
                acc.GuildRank = 0;
                acc.AliveChars = [];
                acc.DeadChars = [];
                acc.OwnedSkins = [];
                acc.Stats = new DbAccStats(i)
                {
                    Credits = NewAccountsConfig.Config.Credits,
                    TotalCredits = NewAccountsConfig.Config.Credits,
                    Fame = NewAccountsConfig.Config.Fame,
                    TotalFame = NewAccountsConfig.Config.Fame,
                    ClassStats = NewAccountsConfig.CreateClassStats()
                };
                acc.LockedIds = [];
                acc.IgnoredIds = [];

                acc.SaveAll(trans);
            }

            await trans.ExecuteAsync();
        }

        public static async Task<RegisterStatus> Register(string username, string password, string ip)
        {
            if (!IsValidUsername(username))
                return RegisterStatus.InvalidName;
            if (!IsValidPassword(password))
                return RegisterStatus.InvalidPassword;

            var accNames = (string)await _db.StringGetAsync(DbKeys.AccountNames);
            if (accNames != null && accNames.Contains(username.ToLower()))
                return RegisterStatus.NameInUse;

            var accCount = (int?)await _db.HashGetAsync(DbKeys.AccountIPs, ip);
            if (accCount != null && accCount > _config.MaxAccountsPerIP)
                return RegisterStatus.MaxAccountsReached;

            var accId = (int)await _db.StringIncrementAsync(DbKeys.NextAccountId);
            var acc = new DbAccount(accId)
            {
                AccountId = accId,
                Name = username,
                MaxNumChars = NewAccountsConfig.Config.MaxChars,
                VaultCount = NewAccountsConfig.Config.VaultCount,
                AliveChars = new int[0],
                DeadChars = new int[0],
                OwnedSkins = new int[0],
                Stats = new DbAccStats(accId)
                {
                    Credits = NewAccountsConfig.Config.Credits,
                    TotalCredits = NewAccountsConfig.Config.Credits,
                    Fame = NewAccountsConfig.Config.Fame,
                    TotalFame = NewAccountsConfig.Config.Fame,
                    ClassStats = NewAccountsConfig.CreateClassStats()
                },
                RegisterTime = DateTime.Now.ToUnixTimestamp(),
                LockedIds = new int[0],
                IgnoredIds = new int[0]
            };

            var trans = _db.CreateTransaction();
            acc.SaveAll(trans);
            _accounts[accId] = acc;

            if (!await trans.ExecuteAsync())
                return RegisterStatus.InternalError;

            if (accCount == null)
                accCount = 1;
            else accCount++; // Increment account count on this ip address

            accNames += $"{acc.Name}|";

            await _logins.Update(_db);
            if (!_logins.AddLogin(accId, username, password))
            {
                _log.Error($"REGISTER FAULT: tried to register an account with an existing account name: {username}");
                return RegisterStatus.NameInUse;
            }

            var saveTrans = _db.CreateTransaction();
            _ = saveTrans.SetAddAsync(DbKeys.KeepOnWipe, $"account.{acc.AccountId}", CommandFlags.FireAndForget);
            _ = saveTrans.StringSetAsync(DbKeys.AccountNames, accNames, flags: CommandFlags.FireAndForget);
            _ = saveTrans.HashSetAsync(DbKeys.AccountIPs, ip, accCount, When.Always, CommandFlags.FireAndForget);
            _logins.SaveAll(saveTrans);

            _ = saveTrans.ExecuteAsync(CommandFlags.FireAndForget);


            return RegisterStatus.Success;
        }

        public static async Task<(DbAccount, VerifyStatus)> VerifyAccount(string username, string password)
        {
            await _logins.Update(_db);
            var loginInfo = _logins.GetLogin(username);
            if (loginInfo == null)
                return (null, VerifyStatus.InvalidCredentials);

            var hashedPass = (password + loginInfo.Salt).ToSHA1();
            if (loginInfo.Hash != hashedPass)
            {
                _log.Warn($"FAILED {username}:{password} => {loginInfo.Hash}!={hashedPass} ({loginInfo.AccId}|{loginInfo.Name})");
                return (null, VerifyStatus.InvalidCredentials);
            }

            if (!_accounts.TryGetValue(loginInfo.AccId, out var acc))
            {
                acc = new DbAccount(loginInfo.AccId);
                await acc.Load(_db);
                _accounts[loginInfo.AccId] = acc;
            }

            await acc.Update(_db);

            return (acc, VerifyStatus.Success);
        }

        public static DbAccount GetGuestAccount()
        {
            if (_accounts.TryGetValue(-1, out var ret))
                return ret;

            ret = new DbAccount(-1);
            ret.Name = _guestNames.RandomElement();
            ret.MaxNumChars = NewAccountsConfig.Config.MaxChars;
            ret.AliveChars = new int[0];
            ret.Stats = new DbAccStats(-1)
            {
                Credits = NewAccountsConfig.Config.Credits,
                TotalCredits = NewAccountsConfig.Config.Credits,
                Fame = NewAccountsConfig.Config.Fame,
                TotalFame = NewAccountsConfig.Config.Fame,
                ClassStats = new ClassStatsInfo[0]
            };
            ret.RegisterTime = DateTime.Now.ToUnixTimestamp();
            _accounts[-1] = ret;
            return ret;
        }

        public static async IAsyncEnumerable<DbChar> GetChars(int accId, params int[] ids) // I can't make this asynchronous, it's an iterator. EDIT: ok I found out about IAsyncEnumerable
        {
            foreach (var id in ids)
            {
                var key = $"char.{accId}.{id}";
                if (await _db.KeyExistsAsync(key))
                {
                    if (!_characters.TryGetValue(key, out var dbChar))
                    {
                        dbChar = new DbChar(accId, id);
                        await dbChar.Load(_db);
                        _characters[key] = dbChar;
                    }

                    await dbChar.Update(_db);
                    yield return dbChar;
                }
            }
        }

        public static async Task<DbDeathInfo> GetDeathInfo(int accId, int charId)
        {
            var key = $"death.{accId}.{charId}";
            if (await _db.KeyExistsAsync(key))
            {
                var deathInfo = new DbDeathInfo(accId, charId);
                await deathInfo.Load(_db);
                return deathInfo;
            }

            return null;
        }

        public static async Task<DbChar> GetChar(int accId, int charId)
        {
            var key = $"char.{accId}.{charId}";
            if (await _db.KeyExistsAsync(key))
            {
                if (!_characters.TryGetValue(key, out var dbChar))
                {
                    dbChar = new DbChar(accId, charId);
                    await dbChar.Load(_db);
                    _characters[key] = dbChar;
                }

                _ = dbChar.Update(_db);
                return dbChar;
            }

            return null;
        }

        public static async Task<(DbChar, CharResult)> CreateCharacter(DbAccount acc, short classType, short skinType)
        {
            if (acc.AliveChars.Length + 1 > acc.MaxNumChars)
                return (null, CharResult.MaxCharactersReached);

            var classDesc = XmlLibrary.PlayerDescs[(ushort)classType];
            var saveTrans = _db.CreateTransaction();
            var charId = acc.NextCharId + 1;
            var chr = new DbChar(acc.AccountId, charId)
            {
                Experience = NewCharsConfig.Config.Experience,
                Level = NewCharsConfig.Config.Level,
                StatPoints = 5,
                CharFame = 0,
                NextLevelXp = 0,
                NextClassQuestFame = 0,
                ClassType = classType,
                HP = (int)classDesc.Stats[StatType.MaxHP],
                MP = (int)classDesc.Stats[StatType.MaxMP],
                MS = (int)classDesc.Stats[StatType.MaxMS],
                SecondaryStats = classDesc.Stats,
                BaseStats = classDesc.Stats,
                MainStats = new Dictionary<StatType, int>(),
                ItemTypes = classDesc.Equipment,
                ItemDatas = new byte[20],
                Fame = NewCharsConfig.Config.Fame,
                Tex1 = NewCharsConfig.Config.Tex1,
                Tex2 = NewCharsConfig.Config.Tex2,
                SkinType = skinType,
                HealthPotions = NewCharsConfig.Config.HealthPotions,
                MagicPotions = NewCharsConfig.Config.MagicPotions,
                CreationTime = DateTime.Now.ToUnixTimestamp(),
                HasBackpack = NewCharsConfig.Config.HasBackpack,
                PrimaryConstellation = -1,
                SecondaryConstellation = -1,
                PrimaryNodeData = -1,
                SecondaryNodeData = -1
            };
            chr.MainStats[StatType.Attack] = (int)classDesc.Stats[StatType.Attack];
            chr.MainStats[StatType.Defense] = (int)classDesc.Stats[StatType.Defense];
            chr.MainStats[StatType.Dexterity] = (int)classDesc.Stats[StatType.Dexterity];
            chr.MainStats[StatType.Wisdom] = (int)classDesc.Stats[StatType.Wisdom];

            chr.SaveAll(saveTrans);

            _characters[$"char.{acc.AccountId}.{charId}"] = chr;

            acc.NextCharId++;
            acc.AliveChars = acc.AliveChars.Append(chr.CharId).ToArray();
            acc.SaveAll(saveTrans);

            if (await saveTrans.ExecuteAsync())
                return (chr, CharResult.Success);

            return (null, CharResult.InternalError);
        }

        public static Task<bool> DeleteChar(DbAccount acc, DbChar chr)
        {
            var trans = _db.CreateTransaction();

            chr.Deleted = true; // Don't set dead to true
            acc.AliveChars = acc.AliveChars.Where(i => i != chr.CharId).ToArray();
            acc.SaveAll(trans);
            chr.SaveAll(trans);

            return trans.ExecuteAsync();
        }

        public static async Task<(bool, string)> BanAccount(string accountName)
        {
            var trans = _db.CreateTransaction();
            var account = await GetAccountByName(accountName);
            if (account == null)
                return (false, "Account name \'{0}\' could not be found");

            account.Banned = true;
            account.SaveAll(trans);
            if (!await trans.ExecuteAsync())
                return (false, "Unknown database error occured while banning account name \'{0}\'");

            return (true, "");
        }

        public static async Task<(bool, string)> UnbanAccount(string accountName)
        {
            var trans = _db.CreateTransaction();

            var account = await GetAccountByName(accountName);
            if (account == null)
                return (false, "Account name \'{0}\' could not be found");

            account.Banned = false;
            account.SaveAll(trans);
            if (!await trans.ExecuteAsync())
                return (false, "Unknown database error occured while unbanning account name \'{0}\'.");
            return (true, "");
        }

        public static async Task<(bool, string)> MuteAccount(string accountName)
        {
            var trans = _db.CreateTransaction();
            var account = await GetAccountByName(accountName);
            if (account == null)
                return (false, "Account name \'{0}\' could not be found.");

            account.Muted = true;
            account.SaveAll(trans);
            if (!await trans.ExecuteAsync())
                return (false, "Unknown database error occured while muting account name \'{0}\'.");
            return (true, "");
        }

        public static async Task<(bool, string)> UnmuteAccount(string accountName)
        {
            var trans = _db.CreateTransaction();
            var account = await GetAccountByName(accountName);
            if (account == null)
                return (false, "Account name \'{0}\' could not be found.");

            account.Muted = false;
            account.SaveAll(trans);
            if (!await trans.ExecuteAsync())
                return (false, "Unknown database error occured while muting account name \'{0}\'.");
            return (true, "");
        }

        public static async Task<DbAccount> GetAccountByName(string accountName)
        {
            var login = _logins.GetLogin(accountName);
            if (login == null)
                return null;

            if (!_accounts.TryGetValue(login.AccId, out var acc))
            {
                acc = new DbAccount(login.AccId);
                await acc.Load(_db);
            }

            return acc;
        }

        public static int UnixTime()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private static DbDeathInfo SetCharDeath(DbAccount acc, DbChar chr, bool save)
        {
            chr.Dead = true;
            chr.DeathTime = UnixTime();
            chr.DeathFame = chr.CharFame;

            acc.Stats.Fame += chr.CharFame;
            acc.Stats.TotalFame += chr.CharFame;
            acc.AliveChars = acc.AliveChars.Where(i => i != chr.CharId).ToArray();
            acc.DeadChars = acc.DeadChars.Append(chr.CharId).ToArray();

            var death = new DbDeathInfo(acc.AccountId, chr.CharId, chr.DeathTime, "Unknown", chr.CharFame, chr.CharFame,
                acc.Name);
            _deaths[$"death.{acc.AccountId}.{chr.CharId}"] = death; // Cache death info result

            if (save)
                Save(acc, chr, death);
            return death;
        }

        public static async Task<DbDeathInfo> Death(DbAccount acc, DbChar chr, string killer)
        {
#if DEBUG
            if (chr == null)
                throw new Exception("Undefined character");
#endif

            var t1 = acc.Update(_db); // Make sure we're working with the latest data
            var t2 = chr.Update(_db);
            await Task.WhenAll(t1, t2);

            if (acc.GuildId != 0)
            {
                var guild = await GetGuild(acc.GuildId);
                if (guild != null)
                {
                    guild.Fame += chr.CharFame;
                    guild.TotalFame += chr.CharFame;
                    acc.Stats.GuildFame += chr.CharFame;
                    acc.Stats.TotalGuildFame += chr.CharFame;
                    Save(guild); // acc.Stats is updated on SetCharDeath
                }
            }

            return SetCharDeath(acc, chr, true);
        }

        public static void Save(params DbObject[] objs)
        {
            var trans = _db.CreateTransaction();

            foreach (var obj in objs)
                obj.SaveAll(trans);

            trans.ExecuteAsync(CommandFlags.FireAndForget);
        }

        public static int GetPlayerCount()
        {
            return (int)_db.StringGetAsync("playerCount").SafeResult();
        }

        public static void SetPlayerCount(int playerCount)
        {
            _ = _db.StringSetAsync("playerCount", playerCount, flags: CommandFlags.FireAndForget);
        }

        public static async void BuyCharSlot(DbAccount acc)
        {
            var trans = _db.CreateTransaction();

            await acc.Update(_db);

            var cost = NewAccountsConfig.Config.CharSlotCost;
            if (acc.Stats.Fame < cost)
                return;

            acc.Stats.Fame -= cost;
            acc.MaxNumChars++;
            acc.SaveAll(trans);

            _ = trans.ExecuteAsync(CommandFlags.FireAndForget);
        }

        public static async Task<DbAccount> GetAccount(int accId)
        {
            if (accId > (int)await _db.StringGetAsync(DbKeys.NextAccountId))
                return null;

            if (!_accounts.TryGetValue(accId, out var acc))
            {
                acc = new DbAccount(accId);
                await acc.Load(_db);
                _accounts[accId] = acc;
            }

            await acc.Update(_db);
            return acc;
        }

        public static bool TryAddLegend(DbDeathInfo death, DbChar chr)
        {
            var ret = _legends.TryAddLegend(death, chr);
            if (ret)
                Save(_legends);

            return ret;
        }

        public static void UpdateLegends()
        {
            var time = UnixTime();
            _legends.UpdateMonth(time);
            _legends.UpdateWeek(time);

            Save(_legends);
        }

        public static async Task<XElement> GetLegends(string timespan)
        {
            await _legends.Update(_db);
            return _legends.GetLeaderboard(timespan);
        }

        public static bool TryCreateParty(DbAccount creator, out DbParty createdParty)
        {
            createdParty = null;

            if (creator.PartyId != -1)
                return false;

            var partyId = (int)_db.StringIncrement(DbKeys.NextPartyId);

            var partySize = creator.Rank switch
            {
                >= 40 => 8,
                >= 30 => 7,
                >= 20 => 6,
                >= 10 => 5,
                _ => 4
            };

            createdParty = new DbParty(partyId) { CreatorAccId = creator.AccountId, MaxMembers = partySize, Members = [creator.AccountId] };
            _parties[partyId] = createdParty;

            creator.PartyId = partyId;
            Save(creator);
            Save(createdParty);

            return true;
        }

        public static void DeleteParty(int partyId)
        {
            _db.KeyDelete($"party.{partyId}");
            _parties.Remove(partyId, out _);
        }

        public static async Task<DbParty> GetParty(int partyId)
        {
            if (_parties.TryGetValue(partyId, out var party))
                return party;

            if (!_db.KeyExists($"party.{partyId}"))
                return null;

            party = new DbParty(partyId);
            await party.Load(_db);

            _parties[partyId] = party;

            return party;
        }

        public static string CreateGuild(DbAccount acc, string guildName)
        {
            if (acc.GuildId > 0)
                return "Already in a guild.";

            if (acc.Stats.Fame < 1000)
                return "Not enough fame";

            if (guildName.Length > 20 || !guildName.Any(char.IsLetter))
                return "Invalid guild name.";

            if (!_db.HashSet(DbKeys.GuildNames, guildName, 0,
                    When.NotExists)) // If this passes we set the guild id value to the correct one
                return "A guild with that name already exists.";

            var trans = _db.CreateTransaction();
            var guildId = (int)_db.StringIncrement(DbKeys.NextGuildId);
            trans.HashSetAsync(DbKeys.GuildNames, guildName, guildId);

            var guild = new DbGuild(guildId) { Name = guildName, Level = 0, Members = new List<int>() { acc.AccountId } };
            _guilds[guildId] = guild;

            acc.GuildId = guildId;
            acc.GuildName = guildName;
            acc.GuildRank = (int)GuildRank.Founder;
            acc.Stats.Fame -= 1000;

            acc.SaveAll(trans);
            guild.SaveAll(trans);

            trans.ExecuteAsync(CommandFlags.FireAndForget);
            return "Success!";
        }

        public static async Task<DbGuild> GetGuild(int guildId)
        {
            if (!_guilds.TryGetValue(guildId, out var guild))
            {
                if (!await _db.KeyExistsAsync($"guild.{guildId}")) // Make sure the guild exists first
                    return null;

                guild = new DbGuild(guildId);
                await guild.Load(_db);

                _guilds[guildId] = guild;
                return guild;
            }

            await guild.Update(_db);
            return guild;
        }

        public static async void RemoveFromGuild(int accId, int guildId)
        {
            var guild = await GetGuild(guildId);
            if (guild == null)
                return;

            var acc = await GetAccount(accId);
            if (acc == null)
                return;

            if (guild.Members.Count == 1)
            {
                DeleteGuild(guild);
                return;
            }

            LeaveGuild(acc);
            guild.Members.Remove(accId);

            Save(guild, acc);
        }

        public static async void DeleteGuild(DbGuild guild)
        {
            var trans = _db.CreateTransaction();
            foreach (var accId in guild.Members)
                LeaveGuild(await GetAccount(accId), trans);

            _ = trans.KeyDeleteAsync($"guild.{guild.GuildId}");
            _ = trans.HashDeleteAsync(DbKeys.GuildNames, guild.Name);

            _ = trans.ExecuteAsync(CommandFlags.FireAndForget);
        }

        public static void LeaveGuild(DbAccount acc, ITransaction trans = null)
        {
            acc.GuildId = 0;
            acc.GuildRank = 0;
            acc.GuildName = null;
            acc.Stats.GuildFame = 0;
            acc.Stats.TotalGuildFame = 0;

            if (trans != null)
                acc.SaveAll(trans);
        }

        public static void EnterGuild(DbAccount acc, DbGuild guild)
        {
            acc.GuildId = guild.GuildId;
            acc.GuildRank = 0;
            acc.GuildName = guild.Name;
            acc.Stats.GuildFame = 0;
            acc.Stats.TotalGuildFame = 0;
        }

        public static async void JoinGuild(int accId, string guildName)
        {
            var acc = await GetAccount(accId);
            if (acc == null)
                return;

            var guild = await GetGuild((int)await _db.HashGetAsync(DbKeys.GuildNames, guildName));
            if (guild == null)
                return;

            if (acc.GuildId != 0)
                return;

            if (guild.Members.Count == 50)
                return;

            guild.Members.Add(accId);
            EnterGuild(acc, guild);

            Save(guild, acc);
        }
    }
}