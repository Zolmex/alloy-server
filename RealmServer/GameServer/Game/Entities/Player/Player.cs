#region

using Common;
using Common.Database;
using Common.Resources.Config;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Chat;
using GameServer.Game.DamageSources;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Entities.Inventory;
using GameServer.Game.Entities.Stacks;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player : Character
    {
        public static Action<Player, string> OnDeath;

        public User User { get; }
        public DbChar Char { get; }
        public PlayerInventory Inventory { get; }
        public Dictionary<ModTypes, Modifier> ActiveModifiers { get; set; }
        public double LootBoost { get; private set; } // todo: loot boost implementation.

        public int AccountId => User.Account.AccountId;
        public bool IsTargetable => !HasConditionEffect(ConditionEffectIndex.Invisible);

        public event Action<RealmTime> OnTick;

        private HashSet<Projectile> _unconfirmedHits = new();
        private List<string> _guildInvites = new();

        public Player(User user, DbChar chr) : base((ushort)chr.ClassType)
        {
            User = user;
            Char = chr;
            IsPlayer = true;

            Inventory = new PlayerInventory(this);
            ActiveModifiers = new Dictionary<ModTypes, Modifier>();
            TradedWith = new HashSet<int>();
            PendingTrades = new HashSet<int>();

            _onDeathHandler = HandleEntityDeath;
            _entityStatChangedHandler = HandleEntityStatChanged;

            SetLootBoost(1d);
        }

        private void Reset()
        {
            Dead = false; // Reset death state
            Position = new WorldPosData();
            PrevPosition = new WorldPosData();
            Stacks.AddStack(ModStacks.ConditionEffect, -1);
        }

        public override void Initialize()
        {
            Reset();
            base.Initialize();

            InitPlayerSight();

            // Load constellation data
            PrimaryConstellation = Char.PrimaryConstellation;
            SecondaryConstellation = Char.SecondaryConstellation;
            PrimaryNodeData = Char.PrimaryNodeData;
            SecondaryNodeData = Char.SecondaryNodeData;
            PrimaryNodes = ConvertNodeData(PrimaryNodeData);
            SecondaryNodes = ConvertNodeData(SecondaryNodeData);

            if (User.Account.Stats.ClassStats.Length <= 0) //if player doesnt have any classstats for some reason
                User.Account.Stats.ClassStats = NewAccountsConfig.CreateClassStats();

            // Load player stats
            Name = User.Account.Name;
            SetParty();
            InitStatBonuses();
            InitLevel(Char);
            NumStars = GetStars();
            TradedWith.Clear();
            PendingTrades.Clear();

            HealthPotions = Char.HealthPotions;
            MagicPotions = Char.MagicPotions;

            // Load stat modifiers
            AddActiveModifiers();
            RecalculateStats();

            // Load inventory data
            Inventory.Load(Char);
            _ability = ResolveAbilityController(Desc.ObjectId);

            if (World is Realm realm)
            {
                SendEnemy("Oryx the Mad God", "You are food for my minions!");
                SendEnemy("Oryx the Mad God",
                    $"I still have {realm.Oryx.EventsMax - realm.Oryx.EventsCounter} guardians left!");
                SendInfo("Use [WASDQE] to move; click to shoot!");
            }

            var spawnPos = World.Map.Regions[TileRegion.Spawn].RandomElement();
            Move(spawnPos.X, spawnPos.Y);

            AddAllPlayers(); // Add all players to this player's visible entities

            World.BroadcastAll(plr => plr.AddPlayerEntity(this));
            World.OnEntityTick += HandleEntityTick;
        }

        protected override void LoadStats()
        {
            if (User.State != ConnectionState.Ready || User.GameInfo.State != GameState.Playing)
                return;

            Stats.Initializing = true;

            Fame = User.Account.Stats.Fame;
            Gold = User.Account.Stats.Credits;
            GuildName = User.Account.GuildName;
            GuildRank = User.Account.GuildRank;
            Skin = Char.SkinType;
            AccRank = User.Account.Rank;
            PartyId = User.Account.PartyId;

            // These stats will be recalculated anyway based on gear, constellations, etc, but no harm in having this here
            MaxHP = Char.HP;
            HP = Char.HP;
            MaxMP = Char.MP;
            MP = Char.MP;
            MaxMS = Char.MS;
            MS = Char.MS;
            Level = Char.Level;
            StatPoints = Char.StatPoints;
            Attack = Char.MainStats[StatType.Attack];
            Defense = Char.MainStats[StatType.Defense];
            Dexterity = Char.MainStats[StatType.Dexterity];
            Wisdom = Char.MainStats[StatType.Wisdom];
            MovementSpeed = Char.BaseStats[StatType.MovementSpeed];
            LifeRegeneration = (int)Char.BaseStats[StatType.MovementSpeed];
            DodgeChance = Char.BaseStats[StatType.DodgeChance];
            CriticalChance = Char.BaseStats[StatType.CriticalChance];
            CriticalDamage = (int)Char.BaseStats[StatType.CriticalDamage];
            ManaRegeneration = (int)Char.BaseStats[StatType.ManaRegeneration];
            MSRegenRate = (int)Char.BaseStats[StatType.MSRegenRate];
            DamageMultiplier = (int)Char.BaseStats[StatType.DamageMultiplier];
            Armor = (int)Char.BaseStats[StatType.Armor];
            AttackSpeed = Char.BaseStats[StatType.AttackSpeed];

            Stats.Initializing = false;
        }

        public int[]
            ConvertNodeData(int data) //turns 4 digit number into array with 4 digits (2312) into [0] = 2 [1] = 3, etc
        {
            if (data == -1)
                return new int[4] { -1, -1, -1, -1 };

            var array = new int[4];
            var newStr = data.ToString();

            for (var i = 0; i < newStr.Length; i++)
                array[i] = int.Parse(newStr[i].ToString());

            return array;
        }

        public void SaveCharacter(bool saveToDb = false)
        {
            if (!Initialized) // Make sure we don't fuck up our character
                return;

            Char.Level = Level;
            Char.Experience = Experience;
            Char.StatPoints = StatPoints;
            Char.CharFame = CharFame;
            Char.NextLevelXp = NextLevelXpGoal;
            Char.NextClassQuestFame = NextClassQuestFame;
            Char.HP = HP;
            Char.MP = MP;
            Char.MS = MS;
            Char.SkinType = Skin;
            Char.HealthPotions = HealthPotions;
            Char.MagicPotions = MagicPotions;
            Char.SecondaryStats[StatType.MaxHP] = MaxHP; // Save secondary stats
            Char.SecondaryStats[StatType.MaxMP] = MaxMP;
            Char.SecondaryStats[StatType.MaxMS] = MaxMS;
            Char.SecondaryStats[StatType.MovementSpeed] = MovementSpeed;
            Char.SecondaryStats[StatType.LifeRegeneration] = LifeRegeneration;
            Char.SecondaryStats[StatType.DodgeChance] = DodgeChance;
            Char.SecondaryStats[StatType.CriticalChance] = CriticalChance;
            Char.SecondaryStats[StatType.CriticalDamage] = CriticalDamage;
            Char.SecondaryStats[StatType.ManaRegeneration] = ManaRegeneration;
            Char.SecondaryStats[StatType.MSRegenRate] = MSRegenRate;
            Char.SecondaryStats[StatType.DamageMultiplier] = DamageMultiplier;
            Char.SecondaryStats[StatType.Armor] = Armor;
            Char.SecondaryStats[StatType.AttackSpeed] = AttackSpeed;
            Char.PrimaryConstellation = PrimaryConstellation;
            Char.SecondaryConstellation = SecondaryConstellation;
            Char.PrimaryNodeData = PrimaryNodeData;
            Char.SecondaryNodeData = SecondaryNodeData;
            Inventory.Save(Char);

            if (saveToDb)
                DbClient.Save(Char);
        }

        public void RemoveReferenceTo(Entity ent)
        {
            _hitEntities.Remove(ent);
        }

        public override bool Tick(RealmTime time)
        {
            if (!base.Tick(time))
                return false;

            OnTick?.Invoke(time);

            SendUpdate();
            SendNewTick();
            TickRegens();
            FindNewQuest(time);

            if (IsInCombat)
                InCombatTick();

            if (TPCooldownLeft > 0)
                TPCooldownLeft -= time.ElapsedMsDelta;

            TimeSinceLastEnemyHit += time.ElapsedMsDelta;
            TimeSinceLastHit += time.ElapsedMsDelta;

            if (User.GameInfo.DamageCounter > 0)
                SendDamageCounterUpdate();

            return true;
        }

        public override bool Death(string killer)
        {
            using (TimedLock.Lock(_deathLock))
            {
                if (Dead)
                    return false;

                Dead = true;
            }

            OnDeath?.Invoke(this, killer);

            SaveCharacter(); // DbChar instance will be saved by DbClient.Death() method
            _ = DbClient.Death(User.Account, Char, killer)
                .ContinueWith(t => DbClient.TryAddLegend(t.Result, Char));

            Network.Messaging.Outgoing.Death.Write(User.Network, AccountId, Char.CharId, killer); //:cry:
            RealmManager.AddTimedAction(1000, () => User.Disconnect(null, DisconnectReason.Death));

            var grave = new Entity(GetGraveType(), 2 * 60000); // 2 min
            grave.Name = Name;
            grave.Move(new Vector2(Position.X, Position.Y));
            grave.EnterWorld(World);

            ChatManager.Announce($"{Name} died at level {Level} killed by {killer}!", false, World.Id, true,
                AccountId);

            LeaveWorld(); // Force LeaveWorld() call after handling death
            return true;
        }

        protected override void LeaveWorld()
        {
            World.OnEntityTick -= HandleEntityTick;
            Stacks.RemoveAllStacks();
            RemoveActiveModifiers();
            base.LeaveWorld();
        }

        public ushort GetGraveType()
        {
            return Level switch
            {
                _ => 1845
            };
        }

        public void SendNotif(string text, int color = 0xFFFFFF, int size = 24, int specifyId = -1)
        {
            Notification.Write(User.Network, specifyId != -1 ? specifyId : Id, text, color, size);
        }

        public void AddCurrency(CurrencyType currency, int amount)
        {
            var accStats = User.Account.Stats;
            switch (currency)
            {
                case CurrencyType.Gold:
                    Gold += amount;
                    accStats.Credits += amount;
                    break;
                case CurrencyType.Fame:
                    Fame += amount;
                    accStats.Fame += amount;
                    break;
                case CurrencyType.GuildFame:
                    _log.Warn("Guild fame not implemented.");
                    break;
            }

            DbClient.Save(accStats);
        }

        public void SetLootBoost(double lootBoost)
        {
            LootBoost = lootBoost;
            foreach (var ent in _hitEntities)
            {
                if (ent == null) continue; // will be cleaned up later in tick
                if (ent is Character c)
                {
                    c.UpdateLootRollCache(this);
                }
            }
        }

        public void StoreUnconfirmedHit(Projectile p)
        {
            using (TimedLock.Lock(_unconfirmedHits))
                _unconfirmedHits.Add(p);
        }

        public void GuildInvite(User invitedBy, string guildName)
        {
            if (!_guildInvites.Contains(guildName))
                _guildInvites.Add(guildName);

            InvitedToGuild.Write(User.Network, invitedBy.Account.Name, guildName);
        }

        public void JoinGuild(string guildname)
        {
            if (!_guildInvites.Remove(guildname))
                return;

            DbClient.JoinGuild(AccountId, guildname);

            GuildName = User.Account.GuildName;
            GuildRank = User.Account.GuildRank;
        }

        public override void Hit(Character hit, DamageSource damageSource)
        {
            if (!_hitEntities.Contains(hit))
            {
                _hitEntities.Add(hit);
            }

            TimeSinceLastEnemyHit = 0;
            IsInCombat = true;
            LastHitTarget = hit;

            if (hit is Enemy)
                EnemyHit(this, damageSource);
        }

        public override void OnHitBy(Character from, DamageSource damageSource)
        {
            IsInCombat = true;

            if (!CheckDodge())
            {
                TimeSinceLastHit = 0;
                Damage(damageSource.GetTotalDamage(), from);
                if (damageSource.Effects != null)
                    ApplyConditionEffects(damageSource.Effects);
            }
        }

        public override void Dispose()
        {
            // Player instance is reused when moving between worlds, so this acts as a Reset() method. Called in RealmManager.Update()
            base.Dispose();

            UnsetParty();

            StacksLost = null;
            OnKill = null;
            OnStatChanged = null;
            OnHeal = null;
            InCombat = null;
            OnEnemyHit = null;
            OnInvChanged = null;
            OnDoShoot = null;
            OnShoot = null;
            OnDamageDealt = null;

            DisposeMods();

            Quest = null;

            _unconfirmedHits.Clear();
            _statBonuses.Clear();

            _visibleTiles.Clear();
            _tilesDiscovered.Clear();
            _newTiles.Clear();
            while (_deadEntities.TryDequeue(out var en))
            {
                en.DeathEvent -= _onDeathHandler;
                using (TimedLock.Lock(en.Stats.StatChangedListeners))
                    en.Stats.StatChangedListeners.Remove(this);
            }

            foreach (var kvp in _visibleEntities)
            {
                var en = kvp.Value;
                en.DeathEvent -= _onDeathHandler;
                using (TimedLock.Lock(en.Stats.StatChangedListeners))
                    en.Stats.StatChangedListeners.Remove(this);
            }

            _visibleEntities.Clear();
            foreach (var kvp in _visibleStaticEntities)
            {
                var en = kvp.Value;
                en.DeathEvent -= _onDeathHandler;
                using (TimedLock.Lock(en.Stats.StatChangedListeners))
                    en.Stats.StatChangedListeners.Remove(this);
            }

            _visibleStaticEntities.Clear();
            _newEntities.Clear();
            _oldEntities.Clear();

            _entityStatUpdates.Clear();

            _guildInvites.Clear();
        }
    }
}