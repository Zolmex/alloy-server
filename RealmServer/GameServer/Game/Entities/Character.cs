#region

using Common;
using Common.ProjectilePaths;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.DamageSources;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Loot;
using GameServer.Game.Entities.Stacks;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using static GameServer.Game.DamageSources.Projectiles.Projectile;
using static GameServer.Game.Entities.Behaviors.BehaviorScript;
using static GameServer.Game.Entities.Player;

#endregion

namespace GameServer.Game.Entities;

public class Character : Entity
{
    public int Condition1
    {
        get => Stats.Get<int>(StatType.Condition1);
        set => Stats.Set(StatType.Condition1, value);
    }

    public int Condition2
    {
        get => Stats.Get<int>(StatType.Condition2);
        set => Stats.Set(StatType.Condition2, value);
    }

    public int AltTexture
    {
        get => Stats.Get<int>(StatType.AltTexture);
        set => Stats.Set(StatType.AltTexture, value);
    }

    public LazyCollection<Projectile> Projectiles = new();
    public StateResourceController StateResources { get; } = new();
    public string Killer { get; private set; }
    public EntityBehavior Behavior;
    public BehaviorController ClassicBehavior;

    public readonly Random Rand = new();
    public readonly StackController Stacks;

    public readonly DamageStorage DamageStorage = new();

    public event Action<Character, Character, int> OnDamagedBy; // <This, From, DamageDealt>
    public event Action<Character, Player, string> OnPlayerText; // <This, From, text>

    private HealthLock? healthLock;
    private readonly Dictionary<ProjectileTargetType, IEnumerable<Character>> _targetCache = new();
    private readonly List<ItemLoot> _lootInfo = new();
    private readonly Dictionary<int, LootCache> _playerLootCaches = new();

    protected readonly object _dmgLock = new();
    protected List<Entity> _hitEntities = new();


    protected readonly ConditionEffectStack _condEffects;

    protected readonly object _projIdLock = new();
    protected ushort _nextProjectileId;

    public Character(ushort objType) : base(objType)
    {
        LoadBehavior();

        Stacks = new StackController(this);
        Stacks.AddStack(ModStacks.ConditionEffect, -1); // Condition effect stack needs to be permanent
        _condEffects = (ConditionEffectStack)Stacks.GetStack(ModStacks.ConditionEffect);

        Name = Desc.DisplayName;

        DeathEvent += CloseDamageCounterForAttackers;
    }

    private void CloseDamageCounterForAttackers(Entity en)
    {
        RealmManager.AddTimedAction(5000, () =>
        {
            foreach (var player in DamageStorage.GetAttackers())
                DamageCounterUpdate.WriteClose(player.User.Network.SendState.Writer);
        });
    }

    public EntityBehavior GetBehavior()
    {
        return Behavior;
    }

    public void LoadBehavior()
    {
        StateResources.ClearResources();

        if (BehaviorLibrary.EntityBehaviors.TryGetValue(Regex.Replace(Desc.ObjectId, @"[\s']", ""),
                out var behaviorType))
        {
            Behavior = (EntityBehavior)Activator.CreateInstance(behaviorType);
            Behavior.StateManager.RegisterEntity(this);
        }

        if (BehaviorLibrary.ClassicBehaviors.TryGetValue(Desc.ObjectId, out var rootState))
            ClassicBehavior = new BehaviorController(this, rootState);

        if (Initialized)
        {
            Behavior?.Initialize(this);
            ClassicBehavior?.Initialize();
        }

        if (Behavior != null && ClassicBehavior != null)
            _log.Warn($"Two behavior systems are defined for {Desc.ObjectId}");
    }

    public T ResolveResource<T>(IStateChild child)
    {
        return StateResources.ResolveResource<T>(child);
    }

    public override void Initialize()
    {
        if (!Initialized)
        {
            Behavior?.Initialize(this);
            ClassicBehavior?.Initialize();
        }

        base.Initialize();
    }

    public void RegisterLoot(string itemId, float dropChancePerc)
    {
        _lootInfo.Add(new ItemLoot(itemId, dropChancePerc / 100));
    }

    public void RegisterLoot(ItemLoot loot)
    {
        _lootInfo.Add(loot);
    }

    public void UpdateLootRollCache(Player p)
    {
        if (!_playerLootCaches.TryGetValue(p.AccountId, out var cache))
        {
            cache = new LootCache(_lootInfo.Count, p.LootBoost);
            _playerLootCaches.Add(p.AccountId, cache);
        }

        cache.UpdateLootBoost(p.LootBoost);
    }

    public static readonly int PROJECTILE_DAMAGE_RANGE = 32;

    public override bool Tick(RealmTime time)
    {
        if (!base.Tick(time))
            return false;

        Behavior?.Tick(this, time);
        ClassicBehavior?.Tick(time);

        Projectiles.Update();

        if (Projectiles.Count > 0)
        {
            CheckForDeadProjectiles(time);
            if (!IsPlayer)
            {
                CacheEntities();

                foreach (var kvp in Projectiles)
                {
                    var proj = kvp.Value;
                    if (proj.Dead)
                        continue;

                    var projPos = proj.PositionAt(time.TotalElapsedMs);
                    if (World.IsOccupied(projPos.X, projPos.Y))
                        proj.Dead = true;
                    else
                        proj.Position = projPos;
                    proj.CheckCollisions(_targetCache[proj.TargetType], time.TotalElapsedMs);
                }

                EmptyEntityCache();
            }
        }

        Stacks.Update(time);

        return true;
    }

    public virtual bool CanBeDamaged(bool ignoreInvincible)
    {
        return ignoreInvincible
            ? !HasConditionEffect(ConditionEffectIndex.Invulnerable)
            : !HasConditionEffect(ConditionEffectIndex.Invincible) &&
              !HasConditionEffect(ConditionEffectIndex.Invulnerable);
    }

    public void CheckForDeadProjectiles(RealmTime time)
    {
        foreach (var kvp in Projectiles)
        {
            var proj = kvp.Value;
            if (proj.ShouldBeRemoved(time))
            {
                Projectiles.Remove(proj);
            }
        }
    }

    public void CacheEntities()
    {
        foreach (var kvp in Projectiles)
        {
            var proj = kvp.Value;
            if (_targetCache.ContainsKey(proj.TargetType))
                continue;

            switch (proj.TargetType)
            {
                case ProjectileTargetType.Player:
                    _targetCache.Add(proj.TargetType,
                        World.GetAllPlayersWithin(Position.X, Position.Y, PROJECTILE_DAMAGE_RANGE)
                            .Select(x => (Character)x));
                    break;
                case ProjectileTargetType.Enemy:
                    _targetCache.Add(proj.TargetType,
                        World.GetEnemiesWithin(Position.X, Position.Y, PROJECTILE_DAMAGE_RANGE));
                    break;
                case ProjectileTargetType.All:
                    _targetCache.Add(proj.TargetType,
                        World.GetAllPlayersWithin(Position.X, Position.Y, PROJECTILE_DAMAGE_RANGE)
                            .Concat(World.GetEnemiesWithin(Position.X, Position.Y, PROJECTILE_DAMAGE_RANGE)));
                    break;
            }
        }
    }

    public void EmptyEntityCache()
    {
        _targetCache.Clear();
    }

    public void CheckProjectileHit(int projectileId, int targetId, int elapsedLifetimeMs, WorldPosData targetPos)
    {
        Projectiles.Update();
        if (!Projectiles.TryGetValue(projectileId, out var proj))
        {
            Console.WriteLine("Projectile is dead");
            return;
        }

        if (proj.Dead)
        {
            Console.WriteLine("Projectile is dead");
            return;
        }

        if (!World.Entities.TryGetValue(targetId, out var target) || target is not Character c)
            return;

        proj.CheckClientCollision(c, elapsedLifetimeMs, targetPos);
    }

    public Character GetNearestOtherEnemyByName(string name, float radius)
    {
        var query = new SearchQuery(name, new IntPoint((int)Position.X, (int)Position.Y), radius, 0);
        if (World.SearchCache.TryGetValue(query, out var result) && result.NearestEntity != null)
            return result.NearestEntity;

        var entities = World.GetEnemiesByName(name, Position.X, Position.Y, radius);
        Character nearest = null;
        var minDist = float.MaxValue;
        if (name != Name)
        {
            foreach (var en in entities)
            {
                if (en.DistSqr(this) >= minDist)
                    continue;

                minDist = en.DistSqr(this);
                nearest = en;
            }

            World.SearchCache.Add(query, new SearchQueryResult(entities, nearest));
            return nearest;
        }

        foreach (var en in entities)
        {
            if (en.Id == Id || en.DistSqr(this) >= minDist)
                continue;

            minDist = en.DistSqr(this);
            nearest = en;
        }

        World.SearchCache.Add(query, new SearchQueryResult(entities, nearest));
        return nearest;
    }

    public IEnumerable<Character> GetEnemiesWithin(float radius)
    {
        return World.GetEnemiesWithin(Position.X, Position.Y, radius);
    }

    public IEnumerable<Player> GetPlayersWithin(float maxRadius, Predicate<Player> cond = null, float minRadius = 0)
    {
        var query = new SearchQuery("Player", new IntPoint((int)Position.X, (int)Position.Y), maxRadius, minRadius);
        if (World.SearchCache.TryGetValue(query, out var result))
            return result.Entities.Select(i => i as Player);

        var players = World.GetAllPlayersWithin(Position.X, Position.Y, maxRadius, cond, minRadius);
        World.SearchCache.Add(query, new SearchQueryResult(players, null));
        return players;
    }

    public IEnumerable<Character> GetOtherEnemiesByName(string name, float radius)
    {
        var entities = World.GetEnemiesByName(name, Position.X, Position.Y, radius);
        if (name != Name)
            return entities;

        return entities.Where(x => x.Id != Id); // i think this is more performant than casting to list then removing. if not then change it.
    }

    public IEnumerable<Character> GetOtherEnemiesByName(IEnumerable<string> names, float radius)
    {
        var entities = World.GetEnemiesByName(names, Position.X, Position.Y, radius);

        return entities.Where(x => x.Id != Id); // i think this is more performant than casting to list then removing. if not then change it.
    }

    public List<Projectile> ShootProjectiles(ProjectilePath path, ushort projType = 0, int minDamage = 0, int maxDamage = 0,
        byte numShots = 1, float angle = 0, float? x = null, float? y = null, float angleInc = 0,
        bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
        Action<Character, Character> onHitEvent = null, int size = 100,
        (ConditionEffectIndex, int)[] effects = null,
        int propId = -1, float radiusSqr = SIGHT_RADIUS_SQR,
        string projName = null, int? damage = null)
    {
        x ??= Position.X;
        y ??= Position.Y;

        if (projName != null)
            projType = XmlLibrary.Id2Object(projName).ObjectType;

        if (damage != null)
        {
            minDamage = damage.Value;
            maxDamage = damage.Value;
        }

        var projectiles = new List<Projectile>();
        var firstProjId = _nextProjectileId++;
        _nextProjectileId += (ushort)(numShots - 1);
        var dmg = (short)Rand.Next(minDamage, maxDamage);
        for (var i = 0; i < numShots; i++)
        {
            var proj = new Projectile(this, firstProjId + i, RealmManager.WorldTime.TotalElapsedMs,
                angle + (angleInc * i), new Vector2(x.Value, y.Value), (int)dmg, path.Clone(),
                path.LifetimeMs, multiHit, passesCover, armorPiercing, ProjectileTargetType.Player,
                onHitEvent, effects);
            projectiles.Add(proj);
            QueueProjectile(proj);
        }

        World.BroadcastAll(plr =>
        {
            if (plr.DistSqr(this) <= Math.Max(radiusSqr, SIGHT_RADIUS_SQR))
            {
                plr.User.SendPacket(new EnemyShoot(
                    firstProjId,
                    Id,
                    projType,
                    x.Value,
                    y.Value,
                    angle.Deg2Rad(),
                    dmg,
                    numShots,
                    angleInc.Deg2Rad(),
                    path,
                    path.LifetimeMs,
                    multiHit,
                    passesCover,
                    armorPiercing,
                    size,
                    effects,
                    propId));
            }
        });

        return projectiles;
    }

    public void QueueProjectile(Projectile projectile)
    {
        Projectiles.Add(projectile);
    }

    public void AOEDamage(Vector2 pos, short damage = 10, int cooldownOffset = 0, int damageCooldown = 0,
        int activateCount = 1, int? color = null, float radius = 5)
    {
        new AOEDamager(World, damage, cooldownOffset, damageCooldown, activateCount, color, pos, radius);
    }

    public virtual Player GetAttackTarget(float radiusSqr, TargetType targetType, float minRadiusSqr = 0)
    {
        switch (targetType)
        {
            case TargetType.ClosestPlayer:
                return this.GetNearestPlayer(radiusSqr,
                    plr => plr.IsTargetable,
                    minRadiusSqr);
            case TargetType.RandomPlayerPerBehavior:
            case TargetType.RandomPlayerPerCycle:
                return GetPlayersWithin(MathF.Sqrt(radiusSqr),
                    plr => !plr.HasConditionEffect(ConditionEffectIndex.Invisible),
                    MathF.Sqrt(minRadiusSqr))?.RandomElement();
            case TargetType.FarthestPlayer:
                return this.GetFarthestPlayer(radiusSqr, plr => plr.IsTargetable, minRadiusSqr);
            default:
                return null;
        }
    }

    public void ApplyConditionEffect(ConditionEffectIndex condEffIndex, int durationMS)
    {
        _condEffects.ApplyConditionEffect(condEffIndex, durationMS);
    }

    public void RemoveConditionEffect(ConditionEffectIndex condEffIndex)
    {
        _condEffects.RemoveConditionEffect(condEffIndex);
    }

    public bool HasConditionEffect(ConditionEffectIndex condEffIndex)
    {
        return _condEffects.HasConditionEffect(condEffIndex);
    }

    public virtual bool Death(string killer = null)
    {
        Killer = killer;
        return TryLeaveWorld();
    }

    public override bool TryLeaveWorld()
    {
        using (TimedLock.Lock(_deathLock))
        {
            if (Dead)
                return false;

            Dead = true;
        }

        if (!IsPlayer)
        {
            HandleXpGain();
            HandleLoot();
            RemoveReferencesTo();
            GetBehavior()?.OnDeath(this, Killer);
        }

        LeaveWorld();
        return true;
    }

    public void HandleXpGain()
    {
        if (Spawned)
            return;

        var baseXp = (int)(Math.Ceiling(Desc.MaxHP / 10f) * Desc.XpMult);
        if (baseXp != 0)
        {
            World.BroadcastAll(player =>
            {
                if (player.DistSqr(this) < SIGHT_RADIUS_SQR * 2)
                    player.GainXP(this, baseXp);
            });
        }
    }

    public void HandleLoot()
    {
        var i = 0;
        var lootCount = _lootInfo.Count;
        if (lootCount == 0)
            return;

        foreach (var player in DamageStorage.GetAttackers())
        {
            if (player == null) continue;

            var damage = player.Damage;
            if (!_playerLootCaches.TryGetValue(player.AccountId, out var lootCache))
                continue;

            var droppedItems = new List<Item>();
            for (i = 0; i < lootCount; i++)
            {
                if (lootCache.LootRolls[i] < _lootInfo[i].DropChance)
                {
                    var item = new Item(XmlLibrary.ItemDescs[_lootInfo[i].ItemType].Root);
                    HandleItemDropped(item);
                    droppedItems.Add(item);
                }
            }

            if (droppedItems.Count > 0)
            {
                World.DropLootWithOverflow(Position.X, Position.Y, droppedItems, player);
            }
        }
    }

    public virtual void OnHitBy(Character from, DamageSource damageSource)
    {
        int damage = damageSource.GetTotalDamage();
        if (from.IsPlayer)
        {
            Player p = (Player)from;
            if (DamageStorage.RegisterDamage(p, damage))
                UpdateLootRollCache(p);
        }

        Damage(damageSource.GetTotalDamage(), from);
        if (damageSource.Effects != null)
            ApplyConditionEffects(damageSource.Effects);
        GetBehavior()?.OnHitBy(this, from, damageSource);
    }

    public void HitBy(Character from, DamageSource damageSource, bool ignoreInvincible = false)
    {
        if (from.Dead || Dead)
            return;

        if (!CanBeDamaged(ignoreInvincible))
            return;

        using (TimedLock.Lock(damageSource.Hit))
            damageSource.Hit.Add(Id);

        OnHitBy(from, damageSource);
    }

    public void DamageWithText(int dmg, int color = 0xFF0000, int size = 24, Character from = null,
        string prefix = "", (ConditionEffectIndex, int)[] effects = null, bool ignoreInvincible = false)
    {
        if (!CanBeDamaged(ignoreInvincible))
            return;

        if (from != null)
            HitBy(from, new IndirectDamage(dmg), ignoreInvincible);
        else
        {
            Damage(dmg);
            if (effects != null)
                ApplyConditionEffects(effects);
        }

        var player =
            from is Player ? from as Player :
            IsPlayer ? (Player)this : null; //so that it works with either enemies or players

        if (player != null)
            player.SendNotif(prefix + "-" + dmg, color, size, Id);
    }

    public int Damage(int damage, Character from = null)
    {
        using (TimedLock.Lock(_dmgLock))
        {
            int msAfterDmg = MS - damage;
            int hpDmg = msAfterDmg < 0 ? Math.Abs(msAfterDmg) : 0;
            var dmgDealt = hpDmg > HP ? HP : hpDmg;

            MS = Math.Max(0, msAfterDmg);
            HP = Math.Max(0, HP - hpDmg);

            OnDamagedBy?.Invoke(this, from, dmgDealt);

            if (from is Player plr)
                plr.DamageDealt(this, dmgDealt);

            if (healthLock != null && healthLock.IsLockActive(this))
                return dmgDealt;

            if (HP <= 0)
            {
                if (this is Enemy e && from is Player p)
                    p.OnKillInvoke(e);

                Death(from != null ? from.Name : "Unknown");
            }

            return dmgDealt;
        }
    }

    public void ApplyConditionEffects(IEnumerable<ConditionEffectDesc> effects)
    {
        ApplyConditionEffects(effects.Select(i => (i.Effect, i.DurationMS)));
    }

    public void ApplyConditionEffects(IEnumerable<(ConditionEffectIndex, int)> effects)
    {
        foreach (var eff in effects)
            ApplyConditionEffect(eff.Item1, eff.Item2);
    }

    public virtual void Hit(Character hit, DamageSource damageSource)
    {
        if (!_hitEntities.Contains(hit))
        {
            _hitEntities.Add(hit);
        }

        GetBehavior()?.OnHitEntity(this, hit, damageSource);
    }

    public void RemoveReferencesTo()
    {
        foreach (var player in DamageStorage.GetAttackers())
            player?.RemoveReferenceTo(this);
    }

    public void Say(string text)
    {
        World.Taunt(this, text);
    }

    private void HandleItemDropped(Item item)
    {
        HandleEquipmentDrop(item);
    }

    private void HandleEquipmentDrop(Item item)
    {
        if (item.SlotType == 10) // Make sure this is equippable
            return;

        // Do stuff here
    }

    public void PlayerTextReceived(Player plr, string text)
    {
        OnPlayerText?.Invoke(this, plr, text);
    }

    /// <summary>
    /// Apply a health lock to this character, preventing it from dying.
    /// </summary>
    /// <param name="healthLock">Health lock.</param>
    public void ApplyHealthLock(HealthLock healthLock)
    {
        this.healthLock = healthLock;
    }

    /// <summary>
    /// Release a health lock from this character, allowing it to die.
    /// </summary>
    public void ReleaseHealthLock()
    {
        healthLock?.ReleaseLock(this);
    }

    public override void Dispose()
    {
        base.Dispose();
        _nextProjectileId = 0;
        Projectiles.Clear();
        StateResources.ClearResources();
        DamageStorage.Clear();
        _lootInfo.Clear();
        _playerLootCaches.Clear();
        _targetCache.Clear();
        _hitEntities.Clear();
        Killer = null;
    }
}