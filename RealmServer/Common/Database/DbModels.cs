#region

using Common.Resources.Config;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Common.Resources.Xml.Descriptors.PlayerDesc;

#endregion

namespace Common.Database
{
    public abstract class DbObject
    {
        private int _version; // This is used to synchronize between appengine and gameserver

        protected readonly DbWriter _writer = new();
        protected readonly DbReader _reader = new();
        protected string _key;

        public virtual async Task Load(IDatabase db)
        {
            _version = (int)await db.HashGetAsync(_key, "dbObjectVersion");
        }

        public virtual void SaveAll(ITransaction trans)
        {
            trans.HashIncrementAsync(_key, "dbObjectVersion");
        }

        public async Task Update(IDatabase db)
        {
            var dbVersion = (int)await db.HashGetAsync(_key, "dbObjectVersion");
            if (dbVersion <= _version) // No need to update
            {
                if (dbVersion < _version) // But if the object in db is outdated, we need to update it
                {
                    var trans = db.CreateTransaction();
                    SaveAll(trans);
                    _ = trans.ExecuteAsync(CommandFlags.FireAndForget);
                }

                return;
            }

            // We're outdated, must load all fields
            await Load(db);
        }
    }

    public class DbDeathInfo : DbObject
    {
        public string AccName;
        public int AccId;
        public int CharId;
        public int DeathTime;
        public string Killer;
        public int BaseFame;
        public int TotalFame;

        public DbDeathInfo(int accId, int charId, int deathTime = -1, string killer = "", int baseFame = -1,
            int totalFame = -1, string accName = "")
        {
            AccName = accName;
            AccId = accId;
            CharId = charId;
            _key = $"death.{AccId}.{CharId}";
            DeathTime = deathTime;
            Killer = killer;
            BaseFame = baseFame;
            TotalFame = totalFame;
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            var hashes = await db.HashGetAsync(_key,
                ["accName", "accId", "charId", "deathTime", "killer", "baseFame", "totalFame"]);
            AccName = (string)hashes[0];
            AccId = (int)hashes[1];
            CharId = (int)hashes[2];
            DeathTime = (int)hashes[3];
            Killer = (string)hashes[4];
            BaseFame = (int)hashes[5];
            TotalFame = (int)hashes[6];
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            trans.HashSetAsync(_key, "accName", AccName);
            trans.HashSetAsync(_key, "accId", AccId);
            trans.HashSetAsync(_key, "charId", CharId);
            trans.HashSetAsync(_key, "deathTime", DeathTime);
            trans.HashSetAsync(_key, "killer", Killer);
            trans.HashSetAsync(_key, "baseFame", BaseFame);
            trans.HashSetAsync(_key, "totalFame", TotalFame);
        }

        public XElement ToXml(DbChar chr)
        {
            var fame = new XElement("Fame");
            var ce = chr.ToXml();
            ce.Add(new XElement("Account", new XElement("Name", AccName)));
            fame.Add(ce);
            //chr.FameStats.ExportTo(fame);
            fame.Add(new XElement("CreatedOn", chr.CreationTime));
            fame.Add(new XElement("KilledOn", DeathTime));
            fame.Add(new XElement("KilledBy", Killer));
            fame.Add(new XElement("BaseFame", BaseFame));
            fame.Add(new XElement("TotalFame", TotalFame));
            return fame;
        }
    }

    public class DbVaultChest : DbObject
    {
        public int AccountId { get; set; }
        public int ChestId { get; set; }
        public int[] ItemTypes { get; set; }
        public byte[] ItemDatas { get; set; }

        public DbVaultChest(int accId, int chestId)
        {
            AccountId = accId;
            ChestId = chestId;
            _key = $"vault.{AccountId}";
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);

            var hashTypes = ChestId + ".types";
            var hashDatas = ChestId + ".datas";
            var hashes = await db.HashGetAsync(_key, [hashTypes, hashDatas]);
            ItemTypes = _reader.ReadIntArray(hashes[0]);
            ItemDatas = hashes[1];
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);

            var hashTypes = ChestId + ".types";
            var hashDatas = ChestId + ".datas";
            trans.HashSetAsync(_key, hashTypes, _writer.Write(ItemTypes));
            trans.HashSetAsync(_key, hashDatas, ItemDatas);
        }
    }

    public class
        DbVault : DbObject // Each redis key is an entire account's vault, each vault chest has 2 hashes (1 for types, 1 for item datas)
    {
        public int AccountId { get; set; }
        public DbVaultChest[] VaultChests { get; set; }

        public DbVault(int accId)
        {
            AccountId = accId;
            _key = $"vault.{AccountId}";
            VaultChests = new DbVaultChest[0];
        }

        public void SetVaultCount(int vaultCount)
        {
            VaultChests = new DbVaultChest[vaultCount];
        }

        public void AddVaultChest()
        {
            var newChests = new DbVaultChest[VaultChests.Length + 1];
            for (var i = 0; i < newChests.Length; i++)
            {
                if (i == VaultChests.Length)
                    newChests[i] = new DbVaultChest(AccountId, i);
                else newChests[i] = VaultChests[i];
            }

            VaultChests = newChests;
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            for (var i = 0; i < VaultChests.Length; i++)
            {
                VaultChests[i] = new DbVaultChest(AccountId, i);
                await VaultChests[i].Load(db);
            }
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            for (var i = 0; i < VaultChests.Length; i++)
                VaultChests[i].SaveAll(trans);
        }
    }

    public class DbGifts : DbObject
    {
        public int AccountId { get; set; }
        public List<int> ItemTypes { get; set; }
        public List<byte> ItemDatas { get; set; }

        public DbGifts(int accId)
        {
            AccountId = accId;
            _key = $"gifts.{AccountId}";
        }

        public void Clear()
        {
            ItemTypes.Clear();
            ItemDatas.Clear();
        }

        public void AddGifts(IEnumerable<Item> items)
        {
            var data = new List<byte>();
            foreach (var item in items)
            {
                if (item == null)
                    continue;

                ItemTypes.Add(item.ObjectType);
                data = item.Export(data);
            }

            ItemDatas.AddRange(data);
        }

        public void AddGift(int itemType, byte[] itemData = null)
        {
            ItemTypes.Add(itemType);
            if (itemData != null)
                ItemDatas.AddRange(itemData);
            else ItemDatas.Add(0);
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            var hashes = await db.HashGetAsync(_key, ["itemTypes", "itemDatas"]);
            ItemTypes = _reader.ReadIntArray(hashes[0])?.ToList();
            ItemDatas = ((byte[])hashes[1])?.ToList();

            ItemTypes ??= new List<int>();
            ItemDatas ??= new List<byte>();
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            trans.HashSetAsync(_key, "itemTypes", _writer.Write(ItemTypes?.ToArray()));
            trans.HashSetAsync(_key, "itemDatas", ItemDatas?.ToArray());
        }
    }

    public class DbAccount : DbObject
    {
        public const int MaxDeadCharsStored = 20;

        public int AccountId { get; set; }
        public string Name { get; set; }
        public int NextCharId { get; set; }
        public int MaxNumChars { get; set; }
        public int NumStars { get; set; }
        public int[] AliveChars { get; set; }
        public int[] DeadChars { get; set; }
        public int[] OwnedSkins { get; set; }
        public bool Admin { get; set; }
        public int Rank { get; set; }
        public bool Muted { get; set; }
        public bool Banned { get; set; }
        public int GuildId { get; set; }
        public string GuildName { get; set; }
        public int GuildRank { get; set; }
        public DbAccStats Stats { get; set; }
        public int RegisterTime { get; set; }
        public int[] LockedIds { get; set; }
        public int[] IgnoredIds { get; set; }
        public int VaultCount { get; set; }
        public DbVault Vault { get; set; }
        public DbGifts Gifts { get; set; }
        public int PartyId { get; set; } = -1;

        public DbAccount(int accId)
        {
            AccountId = accId;
            _key = $"account.{AccountId}";

            Stats = new DbAccStats(accId);
            Vault = new DbVault(accId);
            Gifts = new DbGifts(accId);
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            var hashes = await db.HashGetAsync(_key,
            [
                "name", "nextCharId", "maxChars", "numStars", "aliveChars", "deadChars", "admin", "rank", "muted",
                "banned", "guildId", "guildName", "guildRank", "registerTime", "lockedIds", "ignoredIds", "vaultCount",
                "partyId"
            ]);
            Name = (string)hashes[0];
            NextCharId = (int)hashes[1];
            MaxNumChars = (int)hashes[2];
            NumStars = (int)hashes[3];
            AliveChars = _reader.ReadIntArray(hashes[4]);
            DeadChars = _reader.ReadIntArray(hashes[5]);
            Admin = (bool)hashes[6];
            Rank = (int)hashes[7];
            Muted = (bool)hashes[8];
            Banned = (bool)hashes[9];
            GuildId = (int)hashes[10];
            GuildName = (string)hashes[11];
            GuildRank = (int)hashes[12];
            await Stats.Load(db);
            RegisterTime = (int)hashes[13];
            LockedIds = _reader.ReadIntArray(hashes[14]);
            IgnoredIds = _reader.ReadIntArray(hashes[15]);
            VaultCount = (int)hashes[16];
            PartyId = hashes[17].HasValue ? (int)hashes[17] : -1;
            Vault.SetVaultCount(VaultCount);
            await Vault.Load(db);
            await Gifts.Load(db);
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            trans.HashSetAsync(_key, "name", Name);
            trans.HashSetAsync(_key, "nextCharId", NextCharId);
            trans.HashSetAsync(_key, "maxChars", MaxNumChars);
            trans.HashSetAsync(_key, "numStars", NumStars);
            trans.HashSetAsync(_key, "aliveChars", _writer.Write(AliveChars));
            trans.HashSetAsync(_key, "deadChars", _writer.Write(DeadChars));
            trans.HashSetAsync(_key, "ownedSkins", _writer.Write(OwnedSkins));
            trans.HashSetAsync(_key, "admin", Admin);
            trans.HashSetAsync(_key, "rank", Rank);
            trans.HashSetAsync(_key, "muted", Muted);
            trans.HashSetAsync(_key, "banned", Banned);
            trans.HashSetAsync(_key, "guildId", GuildId);
            trans.HashSetAsync(_key, "guildName", GuildName);
            trans.HashSetAsync(_key, "guildRank", GuildRank);
            Stats.SaveAll(trans);
            trans.HashSetAsync(_key, "registerTime", RegisterTime);
            trans.HashSetAsync(_key, "lockedIds", _writer.Write(LockedIds));
            trans.HashSetAsync(_key, "ignoredIds", _writer.Write(IgnoredIds));
            Vault.SaveAll(trans);
            Gifts.SaveAll(trans);
            trans.HashSetAsync(_key, "vaultCount", VaultCount);
            trans.HashSetAsync(_key, "partyId", PartyId);
        }

        public XElement ToXml()
        {
            return new XElement("Account",
                new XElement("AccountId", AccountId),
                new XElement("Rank", Rank),
                new XElement("Name", Name),
                new XElement("Guild",
                    new XElement("Name", GuildName),
                    new XElement("Rank", GuildRank)),
                Admin ? new XElement("Admin") : null,
                Stats.ToXml()
            );
        }

        public XElement ToCharListXml()
        {
            return new XElement("Chars",
                new XAttribute("nextCharId", NextCharId),
                new XAttribute("maxNumChars", MaxNumChars),
                new XAttribute("charSlotCost", NewAccountsConfig.Config.CharSlotCost),
                new XElement("OwnedSkins", OwnedSkins.ToCommaSepString(",")),
                ToXml(),
                DbClient.GetChars(AccountId, AliveChars).ToEnumerable().Select(c => c.ToXml()),
                NewsConfig.Config.Models.Select(n => n.ToXml()),
                new XElement("Servers",
                    new XElement("Server",
                        new XElement("Name", GameServerConfig.Config.ServerName),
                        new XElement("DNS", GameServerConfig.Config.Address),
                        new XElement("Port", GameServerConfig.Config.Port),
                        new XElement("Players", DbClient.GetPlayerCount()),
                        new XElement("MaxPlayers", GameServerConfig.Config.MaxPlayers),
                        new XElement("AdminOnly", GameServerConfig.Config.AdminOnly ? "true" : "false")
                    )
                )
            );
        }
    }

    public class DbAccStats : DbObject
    {
        public int AccId { get; set; }
        public int BestCharFame { get; set; }
        public int TotalFame { get; set; }
        public int Fame { get; set; }
        public int Credits { get; set; }
        public int TotalCredits { get; set; }
        public int GuildFame { get; set; }
        public int TotalGuildFame { get; set; }
        public ClassStatsInfo[] ClassStats { get; set; }

        public DbAccStats(int accId)
        {
            AccId = accId;
            _key = $"account.{AccId}";
        }

        public ClassStatsInfo GetClassStats(int type)
        {
            return ClassStats.FirstOrDefault(s => s.ObjectType == type);
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            var hashes = await db.HashGetAsync(_key,
            [
                "bestCharFame", "totalFame", "fame", "credits", "totalCredits", "guildFame", "totalGuildFame",
                "classStats"
            ]);
            BestCharFame = (int)hashes[0];
            TotalFame = (int)hashes[1];
            Fame = (int)hashes[2];
            Credits = (int)hashes[3];
            TotalCredits = (int)hashes[4];
            GuildFame = (int)hashes[5];
            TotalGuildFame = (int)hashes[6];
            ClassStats =
                JsonConvert.DeserializeObject<ClassStatsInfo[]>((string)hashes[7]);
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            trans.HashSetAsync(_key, "bestCharFame", BestCharFame);
            trans.HashSetAsync(_key, "totalFame", TotalFame);
            trans.HashSetAsync(_key, "fame", Fame);
            trans.HashSetAsync(_key, "credits", Credits);
            trans.HashSetAsync(_key, "totalCredits", TotalCredits);
            trans.HashSetAsync(_key, "guildFame", GuildFame);
            trans.HashSetAsync(_key, "totalGuildFame", TotalGuildFame);
            trans.HashSetAsync(_key, "classStats", JsonConvert.SerializeObject(ClassStats));
        }

        public XElement ToXml()
        {
            return new XElement("Stats",
                new XElement("BestCharFame", BestCharFame),
                new XElement("TotalFame", TotalFame),
                new XElement("Fame", Fame),
                new XElement("TotalCredits", TotalCredits),
                new XElement("Credits", Credits),
                new XElement("TotalGuildFame", TotalGuildFame),
                new XElement("GuildFame", GuildFame),
                ClassStats?.Select(s => s.ToXml() ?? null)
            );
        }
    }

    public class ClassStatsInfo
    {
        public int ObjectType { get; set; }
        public int BestLevel { get; set; }
        public int BestFame { get; set; }

        public XElement ToXml()
        {
            return new XElement("ClassStats",
                new XAttribute("objectType", ObjectType),
                new XElement("BestLevel", BestLevel),
                new XElement("BestFame", BestFame)
            );
        }
    }

    public class DbLogins : DbObject
    {
        public Dictionary<string, AccountLogin> Logins { get; set; }

        public DbLogins()
        {
            _key = DbKeys.AccountLogins;
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            Logins = new Dictionary<string, AccountLogin>();
            foreach (var hash in await db.HashGetAllAsync(DbKeys.AccountLogins))
            {
                var name = (string)hash.Name;
                if (name == "dbObjectVersion")
                    continue;

                var login = JsonConvert.DeserializeObject<AccountLogin>(hash.Value);
                Logins[name] = login;
            }
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            foreach (var kvp in Logins)
                trans.HashSetAsync(DbKeys.AccountLogins, kvp.Key, JsonConvert.SerializeObject(kvp.Value));
        }

        public bool AddLogin(int accId, string username, string password)
        {
            var salt = MathUtils.GenerateSalt();
            var login = new AccountLogin();
            login.AccId = accId;
            login.Name = username;
            login.Hash = (password + salt).ToSHA1();
            login.Salt = salt;

            return Logins.TryAdd(username, login);
        }

        public AccountLogin GetLogin(string username)
        {
            if (username == null || !Logins.TryGetValue(username, out var ret))
                return null;
            return ret;
        }
    }

    public class AccountLogin
    {
        public int AccId { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }

    public class DbChar : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int StatPoints { get; set; }
        public int CharFame { get; set; }
        public int NextLevelXp { get; set; }
        public int NextClassQuestFame { get; set; }
        public int ClassType { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }

        public int MS { get; set; }

        public Dictionary<StatType, float> BaseStats { get; set; } // This overrides the secondary stat's base stat

        public Dictionary<StatType, float>
            SecondaryStats { get; set; } // The last stats of the player, includes boosts (all stats)

        public Dictionary<StatType, int>
            MainStats { get; set; } // Stat points invested by the player in att, def, dex, or wis

        public int[] ItemTypes { get; set; }
        public byte[] ItemDatas { get; set; }
        public int Fame { get; set; }
        public int Tex1 { get; set; }
        public int Tex2 { get; set; }
        public int SkinType { get; set; }
        public int HealthPotions { get; set; }
        public int MagicPotions { get; set; }
        public int CreationTime { get; set; }
        public bool Deleted { get; set; }
        public bool Dead { get; set; }
        public int DeathFame { get; set; }
        public int DeathTime { get; set; }
        public bool HasBackpack { get; set; }
        public DbFameStats FameStats { get; set; }
        public int PrimaryConstellation { get; set; }
        public int SecondaryConstellation { get; set; }
        public int PrimaryNodeData { get; set; }
        public int SecondaryNodeData { get; set; }
        public int PetId { get; set; }

        public DbChar(int accId, int charId)
        {
            CharId = charId;
            AccId = accId;
            _key = $"char.{AccId}.{CharId}";

            FameStats = new DbFameStats(accId, charId);
            BaseStats = new Dictionary<StatType, float>();
            SecondaryStats = new Dictionary<StatType, float>();
            MainStats = new Dictionary<StatType, int>();
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            var hashes = await db.HashGetAsync(_key,
            [
                "experience", "level", "statPoints", "charFame", "nextLevelXp", "nextClassQuestFame", "classType", "hp",
                "mp", "ms", "stats", "baseStats", "itemTypes", "itemDatas", "fame", "tex1", "tex2", "skinType",
                "healthPotions", "magicPotions", "creationTime", "deleted", "dead", "deathFame", "deathTime",
                "hasBackpack", "petId", "primaryConstellation", "secondaryConstellation", "primaryNodeData",
                "secondaryNodeData", "overrideStats"
            ]);
            Experience = (int)hashes[0];
            Level = (int)hashes[1];
            StatPoints = (int)hashes[2];
            CharFame = (int)hashes[3];
            NextLevelXp = (int)hashes[4];
            NextClassQuestFame = (int)hashes[5];
            ClassType = (int)hashes[6];
            HP = (int)hashes[7];
            MP = (int)hashes[8];
            MS = (int)hashes[9];

            //gets the array (key, value, key, value..etc) then converts it to dictionary
            var statsArray = _reader.ReadFloatArray(hashes[10]);
            SecondaryStats = Enumerable.Range(0, statsArray.Length / 2)
                .ToDictionary(i => (StatType)statsArray[i * 2], i => statsArray[(i * 2) + 1]);

            //gets the array (key, value, key, value..etc) then converts it to dictionary
            var baseStatsArray = _reader.ReadFloatArray(hashes[31]) ?? [];
            BaseStats = Enumerable.Range(0, baseStatsArray.Length / 2)
                .ToDictionary(i => (StatType)baseStatsArray[i * 2], i => baseStatsArray[(i * 2) + 1]);

            //gets the array (key, value, key, value..etc) then converts it to dictionary
            var statPointsArray = _reader.ReadIntArray(hashes[11]);
            MainStats = Enumerable.Range(0, statPointsArray.Length / 2)
                .ToDictionary(i => (StatType)statPointsArray[i * 2], i => statPointsArray[(i * 2) + 1]);

            //if the player is missing a stat, give the player that stat at its default value. (this is for if we add new stats in the future, to prevent issues)
            foreach (PlayerDesc.PlayerStatType statType in Enum.GetValues(typeof(PlayerDesc.PlayerStatType)))
                if (!SecondaryStats.TryGetValue((StatType)statType, out var statValue))
                    SecondaryStats[(StatType)statType] = GetDefaultStatValue((StatType)statType);

            ItemTypes = _reader.ReadIntArray(hashes[12]);
            ItemDatas = hashes[13];
            Fame = (int)hashes[14];
            Tex1 = (int)hashes[15];
            Tex2 = (int)hashes[16];
            SkinType = (int)hashes[17];
            HealthPotions = (int)hashes[18];
            MagicPotions = (int)hashes[19];
            CreationTime = (int)hashes[20];
            Deleted = (bool)hashes[21];
            Dead = (bool)hashes[22];
            DeathFame = (int)hashes[23];
            DeathTime = (int)hashes[24];
            HasBackpack = (bool)hashes[25];
            PetId = (int)hashes[26];
            PrimaryConstellation = (int)hashes[27];
            SecondaryConstellation = (int)hashes[28];
            PrimaryNodeData = (int)hashes[29];
            SecondaryNodeData = (int)hashes[30];
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            trans.HashSetAsync(_key, "experience", Experience);
            trans.HashSetAsync(_key, "level", Level);
            trans.HashSetAsync(_key, "statPoints", StatPoints);
            trans.HashSetAsync(_key, "charFame", CharFame);
            trans.HashSetAsync(_key, "nextLevelXp", NextLevelXp);
            trans.HashSetAsync(_key, "nextClassQuestFame", NextClassQuestFame);
            trans.HashSetAsync(_key, "classType", ClassType);
            trans.HashSetAsync(_key, "hp", HP);
            trans.HashSetAsync(_key, "mp", MP);
            trans.HashSetAsync(_key, "ms", MS);

            var statsArray = SecondaryStats.SelectMany(stat => new[] { (int)stat.Key, stat.Value }).ToArray();
            trans.HashSetAsync(_key, "stats", _writer.Write(statsArray));
            var baseStatsArray = BaseStats.SelectMany(stat => new[] { (int)stat.Key, stat.Value }).ToArray();
            trans.HashSetAsync(_key, "overrideStats", _writer.Write(baseStatsArray));
            var statPointsArray = MainStats.SelectMany(stat => new[] { (int)stat.Key, stat.Value }).ToArray();
            trans.HashSetAsync(_key, "baseStats", _writer.Write(statPointsArray));

            trans.HashSetAsync(_key, "itemTypes", _writer.Write(ItemTypes));
            trans.HashSetAsync(_key, "itemDatas", ItemDatas);
            trans.HashSetAsync(_key, "fame", Fame);
            trans.HashSetAsync(_key, "tex1", Tex1);
            trans.HashSetAsync(_key, "tex2", Tex2);
            trans.HashSetAsync(_key, "skinType", SkinType);
            trans.HashSetAsync(_key, "healthPotions", HealthPotions);
            trans.HashSetAsync(_key, "magicPotions", MagicPotions);
            trans.HashSetAsync(_key, "creationTime", CreationTime);
            trans.HashSetAsync(_key, "deleted", Deleted);
            trans.HashSetAsync(_key, "dead", Dead);
            trans.HashSetAsync(_key, "deathFame", DeathFame);
            trans.HashSetAsync(_key, "deathTime", DeathTime);
            trans.HashSetAsync(_key, "hasBackpack", HasBackpack);
            trans.HashSetAsync(_key, "petId", PetId);
            trans.HashSetAsync(_key, "primaryConstellation", PrimaryConstellation);
            trans.HashSetAsync(_key, "secondaryConstellation", SecondaryConstellation);
            trans.HashSetAsync(_key, "primaryNodeData", PrimaryNodeData);
            trans.HashSetAsync(_key, "secondaryNodeData", SecondaryNodeData);
        }

        public XElement ToXml()
        {
            var elements = new List<XElement>
            {
                new("ObjectType", ClassType),
                new("Level", Level),
                new("StatPoints", StatPoints),
                new("CharFame", CharFame),
                new("NextLevelXp", NextLevelXp),
                new("NextClassQuestFame", NextClassQuestFame),
                new("Experience", Experience),
                new("CurrentFame", Fame),
                new("Equipment", ItemTypes.ToCommaSepString(",")),
                new("ItemDatas", ItemDatas.ToCommaSepString(",")),
                new("MaxHitPoints", SecondaryStats[StatType.MaxHP]),
                new("HitPoints", HP),
                new("MaxMagicPoints", SecondaryStats[StatType.MaxMP]),
                new("MagicPoints", MP),
                new("MaxMagicShield", SecondaryStats[StatType.MaxMS]),
                new("MagicShield", MS),
                new("Attack", SecondaryStats[StatType.Attack]),
                new("Defense", SecondaryStats[StatType.Defense]),
                new("Dexterity", SecondaryStats[StatType.Dexterity]),
                new("Wisdom", SecondaryStats[StatType.Wisdom]),
                new("MovementSpeed", SecondaryStats[StatType.MovementSpeed]),
                new("LifeRegeneration", SecondaryStats[StatType.LifeRegeneration]),
                new("DodgeChance", SecondaryStats[StatType.DodgeChance]),
                new("CriticalChance", SecondaryStats[StatType.CriticalChance]),
                new("CriticalDamage", SecondaryStats[StatType.CriticalDamage]),
                new("ManaRegeneration", SecondaryStats[StatType.ManaRegeneration]),
                new("MagicShieldRegenRate", SecondaryStats[StatType.MSRegenRate]),
                new("DamageMultiplier", SecondaryStats[StatType.DamageMultiplier]),
                new("Armor", SecondaryStats[StatType.Armor]),
                new("AttackSpeed", SecondaryStats[StatType.AttackSpeed]),
                new("Tex1", Tex1),
                new("Tex2", Tex2),
                new("Texture", SkinType),
                new("PrimaryConstellation", PrimaryConstellation),
                new("SecondaryConstellation", SecondaryConstellation),
                new("PrimaryNodeData", PrimaryNodeData),
                new("SecondaryNodeData", SecondaryNodeData)
            };
            return new XElement("Char", new XAttribute("id", CharId), elements);
        }
    }

    public class DbFameStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public DbExplorationStats Exploration { get; set; }
        public DbCombatStats Combat { get; set; }
        public DbKillStats Kills { get; set; }
        public DbDungeonStats Dungeons { get; set; }

        public DbFameStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;

            Exploration = new DbExplorationStats(accId, charId);
            Combat = new DbCombatStats(accId, charId);
            Kills = new DbKillStats(accId, charId);
            Dungeons = new DbDungeonStats(accId, charId);
        }

        public override async Task Load(IDatabase db)
        {
            await Exploration.Load(db);
            await Combat.Load(db);
            await Kills.Load(db);
            await Dungeons.Load(db);
        }

        public override void SaveAll(ITransaction trans)
        {
            Exploration.SaveAll(trans);
            Combat.SaveAll(trans);
            Kills.SaveAll(trans);
            Dungeons.SaveAll(trans);
        }
    }

    public class DbExplorationStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int TilesUncovered { get; set; }
        public int QuestsCompleted { get; set; }
        public int Escapes { get; set; }
        public int NearDeathEscapes { get; set; }
        public int MinutesActive { get; set; }
        public int Teleports { get; set; }

        public DbExplorationStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;
            _key = $"charExploration.{AccId}.{CharId}";
        }

        public override async Task Load(IDatabase db)
        {
            var hashes = await db.HashGetAsync(_key,
                ["tilesUncovered", "questsCompleted", "escapes", "nearDeathEscapes", "minutesActive", "teleports"]);
            TilesUncovered = (int)hashes[0];
            QuestsCompleted = (int)hashes[1];
            Escapes = (int)hashes[2];
            NearDeathEscapes = (int)hashes[3];
            MinutesActive = (int)hashes[4];
            Teleports = (int)hashes[5];
        }

        public override void SaveAll(ITransaction trans)
        {
            trans.HashSetAsync(_key, "tilesUncovered", TilesUncovered);
            trans.HashSetAsync(_key, "questsCompleted", QuestsCompleted);
            trans.HashSetAsync(_key, "escapes", Escapes);
            trans.HashSetAsync(_key, "nearDeathEscapes", NearDeathEscapes);
            trans.HashSetAsync(_key, "minutesActive", MinutesActive);
            trans.HashSetAsync(_key, "teleports", Teleports);
        }
    }

    public class DbCombatStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int Shots { get; set; }
        public int ShotsThatDamage { get; set; }
        public int LevelUpAssists { get; set; }
        public int PotionsDrank { get; set; }
        public int AbilitiesUsed { get; set; }
        public int DamageTaken { get; set; }
        public int DamageDealt { get; set; }

        public DbCombatStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;
            _key = $"charExploration.{AccId}.{CharId}";
        }

        public override async Task Load(IDatabase db)
        {
            var hashes = await db.HashGetAsync(_key,
            [
                "shots", "shotsThatDamage", "levelUpAssists", "potionsDrank", "abilitiesUsed", "damageTaken",
                "damageDealt"
            ]);
            Shots = (int)hashes[0];
            ShotsThatDamage = (int)hashes[1];
            LevelUpAssists = (int)hashes[2];
            PotionsDrank = (int)hashes[3];
            AbilitiesUsed = (int)hashes[4];
            DamageTaken = (int)hashes[5];
            DamageDealt = (int)hashes[6];
        }

        public override void SaveAll(ITransaction trans)
        {
            trans.HashSetAsync(_key, "shots", Shots);
            trans.HashSetAsync(_key, "shotsThatDamage", ShotsThatDamage);
            trans.HashSetAsync(_key, "levelUpAssists", LevelUpAssists);
            trans.HashSetAsync(_key, "potionsDrank", PotionsDrank);
            trans.HashSetAsync(_key, "abilitiesUsed", AbilitiesUsed);
            trans.HashSetAsync(_key, "damageTaken", DamageTaken);
            trans.HashSetAsync(_key, "damageDealt", DamageDealt);
        }
    }

    public class DbKillStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int MonsterKills { get; set; }
        public int MonsterAssists { get; set; }
        public int GodKills { get; set; }
        public int GodAssists { get; set; }
        public int OryxKills { get; set; }
        public int OryxAssists { get; set; }
        public int CubeKills { get; set; }
        public int CubeAssists { get; set; }
        public int BlueBags { get; set; }
        public int CyanBags { get; set; }
        public int WhiteBags { get; set; }

        public DbKillStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;
            _key = $"charExploration.{AccId}.{CharId}";
        }

        public override async Task Load(IDatabase db)
        {
            var hashes = await db.HashGetAsync(_key,
            [
                "monsterKills", "monsterAssists", "godKills", "godAssists", "oryxKills", "oryxAssists", "cubeKills",
                "cubeAssists", "blueBags", "cyanBags", "whiteBags"
            ]);
            MonsterKills = (int)hashes[0];
            MonsterAssists = (int)hashes[1];
            GodKills = (int)hashes[2];
            GodAssists = (int)hashes[3];
            OryxKills = (int)hashes[4];
            OryxAssists = (int)hashes[5];
            CubeKills = (int)hashes[6];
            CubeAssists = (int)hashes[7];
            BlueBags = (int)hashes[8];
            CyanBags = (int)hashes[9];
            WhiteBags = (int)hashes[10];
        }

        public override void SaveAll(ITransaction trans)
        {
            trans.HashSetAsync(_key, "monsterKills", MonsterKills);
            trans.HashSetAsync(_key, "monsterAssists", MonsterAssists);
            trans.HashSetAsync(_key, "godKills", GodKills);
            trans.HashSetAsync(_key, "godAssists", GodAssists);
            trans.HashSetAsync(_key, "oryxKills", OryxKills);
            trans.HashSetAsync(_key, "oryxAssists", OryxAssists);
            trans.HashSetAsync(_key, "cubeKills", CubeKills);
            trans.HashSetAsync(_key, "cubeAssists", CubeAssists);
            trans.HashSetAsync(_key, "blueBags", BlueBags);
            trans.HashSetAsync(_key, "cyanBags", CyanBags);
            trans.HashSetAsync(_key, "whiteBags", WhiteBags);
        }
    }

    public class DbDungeonStats : DbObject
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int PirateCavesCompleted { get; set; }
        public int UndeadLairsCompleted { get; set; }
        public int AbyssOfDemonsCompleted { get; set; }
        public int SnakePitsCompleted { get; set; }
        public int SpiderDensCompleted { get; set; }
        public int SpriteWorldsCompleted { get; set; }
        public int TombsCompleted { get; set; }

        public DbDungeonStats(int accId, int charId)
        {
            AccId = accId;
            CharId = charId;
            _key = $"charExploration.{AccId}.{CharId}";
        }

        public override async Task Load(IDatabase db)
        {
            var hashes = await db.HashGetAsync(_key,
            [
                "pirateCavesCompleted", "undeadLairsCompleted", "abyssOfDemonsCompleted", "snakePitsCompleted",
                "spiderDensCompleted", "spriteWorldsCompleted", "tombsCompleted"
            ]);
            PirateCavesCompleted = (int)hashes[0];
            UndeadLairsCompleted = (int)hashes[1];
            AbyssOfDemonsCompleted = (int)hashes[2];
            SnakePitsCompleted = (int)hashes[3];
            SpiderDensCompleted = (int)hashes[4];
            SpriteWorldsCompleted = (int)hashes[5];
            TombsCompleted = (int)hashes[6];
        }

        public override void SaveAll(ITransaction trans)
        {
            trans.HashSetAsync(_key, "pirateCavesCompleted", PirateCavesCompleted);
            trans.HashSetAsync(_key, "undeadLairsCompleted", UndeadLairsCompleted);
            trans.HashSetAsync(_key, "abyssOfDemonsCompleted", AbyssOfDemonsCompleted);
            trans.HashSetAsync(_key, "snakePitsCompleted", SnakePitsCompleted);
            trans.HashSetAsync(_key, "spiderDensCompleted", SpiderDensCompleted);
            trans.HashSetAsync(_key, "spriteWorldsCompleted", SpriteWorldsCompleted);
            trans.HashSetAsync(_key, "tombsCompleted", TombsCompleted);
        }
    }

    public class DbLegends : DbObject
    {
        private const int
            MAX_ENTRIES =
                30; // The client will only show the top 20, but we save more so if some deaths expire the month/week we can still have 20 and not less

        private static readonly Logger _log = new(typeof(DbLegends));

        private List<LegendsEntry> _allTime; // These are ordered in the client
        private List<LegendsEntry> _month;
        private List<LegendsEntry> _week;

        private int
            _allTimeMinimum; // Deaths will be compared to each of these fame amounts to see in which category they will go

        private int _monthMinimum;
        private int _weekMinimum;

        public DbLegends()
        {
            _key = DbKeys.Legends;
            _allTime = new List<LegendsEntry>();
            _month = new List<LegendsEntry>();
            _week = new List<LegendsEntry>();
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            foreach (var hash in await db.HashGetAllAsync(_key)) // 3 hashes (allTime, month, weekly)
            {
                var name = (string)hash.Name;
                if (name == "dbObjectVersion")
                    continue;

                var entries = JsonConvert.DeserializeObject<LegendsEntry[]>(hash.Value).ToList();
                switch (name)
                {
                    case "allTime":
                        _allTime = entries;
                        break;
                    case "month":
                        _month = entries;
                        break;
                    case "week":
                        _week = entries;
                        break;
                }
            }

            if (_allTime.Count != 0)
                _allTimeMinimum = _allTime.Min(e => e.TotalFame);

            if (_month.Count != 0)
                _monthMinimum = _month.Min(e => e.TotalFame);

            if (_week.Count != 0)
                _weekMinimum = _week.Min(e => e.TotalFame);
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);

            trans.HashSetAsync(_key, "allTime", JsonConvert.SerializeObject(_allTime));
            trans.HashSetAsync(_key, "month", JsonConvert.SerializeObject(_month));
            trans.HashSetAsync(_key, "week", JsonConvert.SerializeObject(_week));
        }

        public bool TryAddLegend(DbDeathInfo death, DbChar chr)
        {
            if (!TryAddToBoard("week", death, chr)) // Add to week leaderboard
                return false;

            TryAddToBoard("month", death,
                chr); // Try adding to these other boards, return true cus the death was added to week leaderboard
            TryAddToBoard("allTime", death, chr);
            return true;
        }

        private bool TryAddToBoard(string timespan, DbDeathInfo death, DbChar chr)
        {
            var minimum = 0;
            List<LegendsEntry> list = null;
            switch (timespan)
            {
                case "allTime":
                    list = _allTime;
                    minimum = _allTimeMinimum;
                    break;
                case "month":
                    list = _month;
                    minimum = _monthMinimum;
                    break;
                case "week":
                    list = _week;
                    minimum = _weekMinimum;
                    break;
                default:
                    _log.Error($"Invalid legends timespan: {timespan}");
                    return false;
            }

            using (TimedLock.Lock(list))
            {
                var idx = 0;
                if (list.Count < MAX_ENTRIES)
                {
                    idx = list.Count; // Add new entry
                    list.Add(new LegendsEntry());
                }
                else if (death.TotalFame > minimum)
                {
                    var removed = list.FirstOrDefault(e => e.TotalFame == minimum);
                    idx = list.IndexOf(removed); // Find the index
                    if (idx == -1) // How could this be...
                    {
                        _log.Error($"Could not find replacement for {minimum} top 20 {timespan} legends board.");
                        return false;
                    }
                }
                else
                    return false;

                var entry = list[idx]; // Update entry info with new char's info
                entry.AccId = death.AccId;
                entry.CharId = death.CharId;
                entry.DeathTime = death.DeathTime;
                entry.TotalFame = death.TotalFame;

                entry.ObjectType = chr.ClassType;
                entry.SkinType = chr.SkinType;
                entry.Tex1 = chr.Tex1;
                entry.Tex2 = chr.Tex2;
                entry.Name = death.AccName;
                entry.ItemTypes = chr.ItemTypes;
                entry.ItemDatas = chr.ItemDatas.ToCommaSepString();

                minimum = list.Min(e => e.TotalFame);

                switch (timespan)
                {
                    case "allTime":
                        _allTime = list;
                        _allTimeMinimum = minimum;
                        break;
                    case "month":
                        _month = list;
                        _monthMinimum = minimum;
                        break;
                    case "week":
                        _week = list;
                        _weekMinimum = minimum;
                        break;
                }
            }

            return true;
        }

        public void UpdateMonth(int timeNow) // If the char has been dead for over 30 days, remove from leaderboard
        {
            if (_month.Count == 0)
                return;

            for (var i = 0; i < _month.Count; i++)
                if (timeNow - _month[i].DeathTime > TimeSpan.FromDays(30).TotalSeconds)
                {
                    _month.RemoveAt(i);
                    i--;
                }
        }

        public void UpdateWeek(int timeNow)
        {
            if (_week.Count == 0)
                return;

            for (var i = 0; i < _week.Count; i++)
                if (timeNow - _week[i].DeathTime > TimeSpan.FromDays(7).TotalSeconds)
                {
                    _week.RemoveAt(i);
                    i--;
                }
        }

        public XElement GetLeaderboard(string timespan)
        {
            List<LegendsEntry> list = null;
            switch (timespan)
            {
                case "all":
                    list = _allTime;
                    break;
                case "month":
                    list = _month;
                    break;
                case "week":
                    list = _week;
                    break;
            }

            ;

            if (list == null)
                return null;

            return new XElement("Legends",
                list.Select(e => e.ToXml())
            );
        }
    }

    public class LegendsEntry
    {
        public int AccId { get; set; }
        public int CharId { get; set; }
        public int DeathTime { get; set; }
        public int TotalFame { get; set; }

        public int ObjectType { get; set; }
        public int SkinType { get; set; }
        public int Tex1 { get; set; }
        public int Tex2 { get; set; }
        public string Name { get; set; }
        public int[] ItemTypes { get; set; }
        public string ItemDatas { get; set; }

        public XElement ToXml()
        {
            return new XElement("FameListElem",
                new XAttribute("accountId", AccId),
                new XAttribute("charId", CharId),
                new XElement("TotalFame", TotalFame),
                new XElement("ObjectType", ObjectType),
                new XElement("Texture", SkinType),
                new XElement("Tex1", Tex1),
                new XElement("Tex2", Tex2),
                new XElement("Name", Name),
                new XElement("Equipment", ItemTypes.ToCommaSepString()),
                new XElement("ItemDatas", ItemDatas)
            );
        }
    }

    public class DbGuild : DbObject
    {
        public int GuildId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Fame { get; set; }
        public int TotalFame { get; set; }
        public List<int> Members { get; set; }
        public string Board { get; set; }

        public DbGuild(int guildId)
        {
            GuildId = guildId;
            _key = $"guild.{guildId}";
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            var hashes = await db.HashGetAsync(_key, ["name", "level", "fame", "totalFame", "members", "board"]);
            Name = (string)hashes[0];
            Level = (int)hashes[1];
            Fame = (int)hashes[2];
            TotalFame = (int)hashes[3];
            Members = _reader.ReadIntArray(hashes[4]).ToList();
            Board = (string)hashes[5];
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            trans.HashSetAsync(_key, "name", Name);
            trans.HashSetAsync(_key, "level", Level);
            trans.HashSetAsync(_key, "fame", Fame);
            trans.HashSetAsync(_key, "totalFame", TotalFame);
            trans.HashSetAsync(_key, "members", _writer.Write(Members.ToArray()));
            trans.HashSetAsync(_key, "board", Board);
        }

        public XElement ToXML()
        {
            return new XElement("Guild",
                new XAttribute("name", Name),
                new XElement("CurrentFame", Fame),
                Members.Select(async m =>
                {
                    var acc = await DbClient.GetAccount(m);
                    return new XElement("Member",
                        new XElement("Name", acc.Name),
                        new XElement("Rank", acc.GuildRank),
                        new XElement("Fame", acc.Stats.GuildFame)
                    );
                })
            );
        }
    }

    public class DbParty : DbObject
    {
        public int PartyId { get; set; }
        public int CreatorAccId { get; set; }
        public int MaxMembers { get; set; }
        public List<int> Members { get; set; }

        public DbParty(int partyId)
        {
            PartyId = partyId;
            _key = $"party.{partyId}";
        }

        public override async Task Load(IDatabase db)
        {
            await base.Load(db);
            var hashes = await db.HashGetAsync(_key, ["partyId", "creatorAccId", "maxMembers", "members"]);
            PartyId = (int)hashes[0];
            CreatorAccId = (int)hashes[1];
            MaxMembers = (int)hashes[2];
            Members = _reader.ReadIntArray(hashes[3]).ToList();
        }

        public override void SaveAll(ITransaction trans)
        {
            base.SaveAll(trans);
            trans.HashSetAsync(_key, "partyId", PartyId);
            trans.HashSetAsync(_key, "creatorAccId", CreatorAccId);
            trans.HashSetAsync(_key, "maxMembers", MaxMembers);
            trans.HashSetAsync(_key, "members", _writer.Write(Members.ToArray()));
        }
    }
}