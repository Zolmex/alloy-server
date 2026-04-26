#region

using System;
using System.Collections.Generic;
using System.Numerics;
using Common;
using Common.Database;
using Common.Database.Models;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Chat;
using GameServer.Game.Entities.DamageSources;
using GameServer.Game.Entities.DamageSources.Types;
using GameServer.Game.Entities.Inventory;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds.Logic;

#endregion

namespace GameServer.Game.Entities.Types;

public partial class Player : CharacterEntity {
    public static Action<Player, string> OnDeath;
    private readonly List<string> _guildInvites = new();

    private readonly HashSet<Projectile> _unconfirmedHits = new();

    public Player(User user, Character chr) : base(chr.ObjectType) {
        User = user;
        Char = chr;
        IsPlayer = true;

        Inventory = new PlayerInventory(this);
        TradedWith = new HashSet<int>();
        PendingTrades = new HashSet<int>();

        _onDeathHandler = HandleEntityDeath;
        _entityStatChangedHandler = HandleEntityStatChanged;

        SetLootBoost(1d);
    }

    public User User { get; }
    public Character Char { get; }
    public PlayerInventory Inventory { get; }
    public double LootBoost { get; private set; } // todo: loot boost implementation.

    public int AccountId => User.Account.Id;
    public bool IsTargetable => !HasConditionEffect(ConditionEffectIndex.Invisible);

    public event Action<RealmTime> OnTick;

    private void Reset() {
        Dead = false; // Reset death state
        Position = new WorldPosData();
        PrevPosition = new WorldPosData();
    }

    public override void Initialize() {
        Reset();
        base.Initialize();

        InitPlayerSight();

        // if (User.Account.Stats.ClassStats.Length <= 0) //if player doesnt have any classstats for some reason
        //     User.Account.Stats.ClassStats = NewAccountsConfig.CreateClassStats();

        // Load player stats
        Name = User.Account.Name;
        InitLevel(Char);
        NumStars = GetStars();
        TradedWith.Clear();
        PendingTrades.Clear();

        HealthPotions = Char.HealthPotions;
        MagicPotions = Char.MagicPotions;

        // Load inventory data
        Inventory.Load(Char);

        if (World is Realm realm) {
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

    protected override void LoadStats() {
        if (User.State != ConnectionState.Ready || User.GameInfo.State != GameState.Playing)
            return;

        Stats.Initializing = true;

        Fame = (int)User.Account.AccStats!.CurrentFame;
        Gold = (int)User.Account.AccStats.CurrentCredits;
        GuildName = User.Account.GuildName;
        GuildRank = User.Account.GuildMember?.GuildRank ?? 0;
        Skin = Char.SkinType;
        AccRank = User.Account.Rank;

        // These stats will be recalculated anyway based on gear, constellations, etc, but no harm in having this here
        if (Char.CharStats != null) {
            MaxHP = (int)Char.CharStats.MaxHp;
            HP = (int)Char.CharStats.Hp;
            MaxMP = (int)Char.CharStats.MaxMp;
            MP = (int)Char.CharStats.Mp;
            Level = Char.Level;
            Attack = (int)Char.CharStats.Attack;
            Defense = (int)Char.CharStats.Defense;
            Speed = (int)Char.CharStats.Speed;
            Dexterity = (int)Char.CharStats.Dexterity;
            Vitality = (int)Char.CharStats.Vitality;
            Wisdom = (int)Char.CharStats.Wisdom;
        }

        Stats.Initializing = false;
    }

    public void SaveCharacter(bool saveToDb = false) {
        if (!Initialized) // Make sure we don't fuck up our character
            return;

        Char.Level = (ushort)Level;
        Char.XpPoints = (uint)Experience;
        Char.CurrentFame = (uint)CharFame;
        // Char.NextLevelXp = NextLevelXpGoal; // TODO: fix
        // Char.NextClassQuestFame = NextClassQuestFame;
        if (Char.CharStats != null) {
            Char.CharStats.Hp = (uint)HP;
            Char.CharStats.Mp = (uint)MP;
            Char.SkinType = (ushort)Skin;
            Char.HealthPotions = (ushort)HealthPotions;
            Char.MagicPotions = (ushort)MagicPotions;
            Char.CharStats.MaxHp = (uint)MaxHP; // Save secondary stats
            Char.CharStats.MaxMp = (uint)MaxMP;
            Char.CharStats.Attack = (uint)Attack;
            Char.CharStats.Defense = (uint)Defense;
            Char.CharStats.Speed = (uint)Speed;
            Char.CharStats.Dexterity = (uint)Dexterity;
            Char.CharStats.Vitality = (uint)Vitality;
            Char.CharStats.Wisdom = (uint)Wisdom;
        }

        Inventory.Save(Char);

        if (saveToDb)
            DbClient.FlushAsync(Char);
    }

    public void RemoveReferenceTo(Entity ent) {
        _hitEntities.Remove(ent);
    }

    public override bool Tick(RealmTime time) {
        if (!base.Tick(time))
            return false;

        OnTick?.Invoke(time);

        SendUpdate();
        SendNewTick();
        TickRegens();
        FindNewQuest(time);

        if (TPCooldownLeft > 0)
            TPCooldownLeft -= time.ElapsedMsDelta;

        return true;
    }

    public override bool Death(string killer) {
        if (Dead)
            return false;

        Dead = true;

        OnDeath?.Invoke(this, killer);

        SaveCharacter(); // DbChar instance will be saved by DbClient.Death() method
        // _ = DbClientOld.Death(User.Account, Char, killer) // TODO: fix
        //     .ContinueWith(t => DbClientOld.TryAddLegend(t.Result, Char));

        User.SendPacket(new Death(AccountId, Char.AccCharId, killer)); // 😭😭😭
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

    protected override void LeaveWorld() {
        World.OnEntityTick -= HandleEntityTick;
        base.LeaveWorld();
    }

    public ushort GetGraveType() {
        return Level switch {
            _ => 1845
        };
    }

    public void SendNotif(string text, int color = 0xFFFFFF, int size = 24, int specifyId = -1) {
        User.SendPacket(new Notification(specifyId != -1 ? specifyId : Id, text, color, size));
    }

    public void AddCurrency(CurrencyType currency, int amount) {
        var accStats = User.Account.AccStats;
        if (accStats == null)
            throw new Exception($"Null account stats. AccId:{User.Account.Id}");

        switch (currency) {
            case CurrencyType.Gold:
                Gold += amount;
                accStats.CurrentCredits += (uint)amount;
                break;
            case CurrencyType.Fame:
                Fame += amount;
                accStats.CurrentFame += (uint)amount;
                break;
            case CurrencyType.GuildFame:
                _log.Warn("Guild fame not implemented.");
                break;
        }

        _ = DbClient.FlushAsync(accStats);
    }

    public void SetLootBoost(double lootBoost) {
        LootBoost = lootBoost;
        foreach (var ent in _hitEntities) {
            if (ent == null) continue; // will be cleaned up later in tick
            if (ent is CharacterEntity c) c.UpdateLootRollCache(this);
        }
    }

    public void StoreUnconfirmedHit(Projectile p) {
        _unconfirmedHits.Add(p);
    }

    public void GuildInvite(User invitedBy, string guildName) {
        if (!_guildInvites.Contains(guildName))
            _guildInvites.Add(guildName);

        User.SendPacket(new InvitedToGuild(invitedBy.Account.Name, guildName));
    }

    public void JoinGuild(string guildname) {
        if (!_guildInvites.Remove(guildname))
            return;

        // DbClient.JoinGuild(AccountId, guildname); // TODO: fix

        GuildName = User.Account.GuildName;
        GuildRank = User.Account.GuildMember?.GuildRank ?? 0;
    }

    public override void Hit(CharacterEntity hit, DamageSource damageSource) {
        if (!_hitEntities.Contains(hit)) _hitEntities.Add(hit);

        if (hit is Enemy)
            EnemyHit(this, damageSource);
    }

    public override void OnHitBy(CharacterEntity from, DamageSource damageSource) {
        Damage(damageSource.GetTotalDamage(), from);
        if (damageSource.Effects != null)
            ApplyConditionEffects(damageSource.Effects);
    }

    public override void Dispose() {
        // Player instance is reused when moving between worlds, so this acts as a Reset() method. Called in RealmManager.Update()
        base.Dispose();

        OnKill = null;
        OnHeal = null;
        InCombat = null;
        OnEnemyHit = null;
        OnInvChanged = null;
        OnDoShoot = null;
        OnShoot = null;
        OnDamageDealt = null;

        Quest = null;

        _unconfirmedHits.Clear();

        _visibleTiles.Clear();
        _tilesDiscovered.Clear();
        _newTiles.Clear();
        while (_deadEntities.TryDequeue(out var en)) {
            en.DeathEvent -= _onDeathHandler;
            en.Stats.StatChangedListeners.Remove(this);
        }

        foreach (var kvp in _visibleEntities) {
            var en = kvp.Value;
            en.DeathEvent -= _onDeathHandler;
            en.Stats.StatChangedListeners.Remove(this);
        }

        _visibleEntities.Clear();
        foreach (var kvp in _visibleStaticEntities) {
            var en = kvp.Value;
            en.DeathEvent -= _onDeathHandler;
            en.Stats.StatChangedListeners.Remove(this);
        }

        _visibleStaticEntities.Clear();
        _newEntities.Clear();
        _oldEntities.Clear();

        _entityStatUpdates.Clear();

        _guildInvites.Clear();
    }
}