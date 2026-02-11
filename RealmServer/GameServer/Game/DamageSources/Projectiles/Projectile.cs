#region

using Common;
using Common.ProjectilePaths;
using Common.Utilities;
using GameServer.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#endregion

namespace GameServer.Game.DamageSources.Projectiles;

public class Projectile : DamageSource, IIdentifiable
{
    public enum ProjectileTargetType
    {
        Player,
        Enemy,
        All
    }

    public const float PROJECTILE_RADIUS = 0.5f;
    public const float PROJECTILE_RADIUS_PLAYER = 0.5f;
    public const float SERVER_LENIENCY_RADIUS = PROJECTILE_RADIUS + 0.15f;

    public const int MOVEMENT_TIME_FRAME = 50;
    private static readonly Logger _log = new(typeof(Projectile));

    private readonly Action<CharacterEntity, CharacterEntity> _onHitEvent; // hit, hit by

    private readonly HashSet<(Player p, WorldPosData pos, long time)> _unconfirmedHits = new();
    public readonly float Angle;
    public readonly bool ArmorPiercing;
    public readonly HashSet<int> Hit;
    public readonly long Lifetime;
    public readonly bool MultiHit;
    public readonly CharacterEntity Owner;
    public readonly bool PassesCover;
    private readonly ProjectilePath Path;
    public readonly Vector2 StartPosition;
    private readonly long startTime;
    public readonly ProjectileTargetType TargetType;
    public int Damage;
    public bool Dead;

    public Vector2 Position;

    public long Time;

    public Projectile(CharacterEntity owner, int id, long time, float angle, Vector2 startPos, int damage,
        ProjectilePath path,
        int lifetimeMS, bool multiHit, bool passesCover, bool armorPiercing, ProjectileTargetType targetType,
        Action<CharacterEntity, CharacterEntity> onHitEvent = null, (ConditionEffectIndex, int)[] effects = null)
    {
        Owner = owner;
        Id = id;
        Lifetime = time + lifetimeMS;
        Angle = angle.Deg2Rad();
        StartPosition = startPos;
        Position = startPos;
        Damage = damage;
        Hit = new HashSet<int>();
        //LifetimeMS = lifetimeMS;
        MultiHit = multiHit;
        PassesCover = passesCover;
        ArmorPiercing = armorPiercing;
        TargetType = targetType;
        startTime = time;

        // Setup our path info
        var pathInfo = new ProjectileInfo { StartPos = startPos, ShootAngle = angle.Deg2Rad(), LifetimeMs = lifetimeMS, ProjId = id };

        Path = path;
        Path.SetInfo(pathInfo);

        Effects = effects;
        _onHitEvent = onHitEvent;
    }

    public int Id { get; set; }

    public bool CanHit(Entity en)
    {
        // if (en.HasConditionEffect(ConditionEffectIndex.Invincible) || en.HasConditionEffect(ConditionEffectIndex.Stasis))
        //    return false;

        using (TimedLock.Lock(this))
            return !Hit.Add(en.Id);
    }

    public Vector2 PositionAt(long totalElapsedMs)
    {
        Vector2 moveVec;
        if (totalElapsedMs > Lifetime) // Projectile lifetime grace period
        {
            moveVec = Path.PositionAt((int)(Lifetime - startTime));
        }
        else
        {
            moveVec = Path.PositionAt((int)(totalElapsedMs - startTime));
        }

        return Path.Info.StartPos + moveVec;
    }

    // Server-side projectile collision
    public void CheckCollisions(IEnumerable<CharacterEntity> entitiesInRadius, long serverTime)
    {
        foreach (var entity in entitiesInRadius)
        {
            using (TimedLock.Lock(this))
                if (Hit.Contains(entity.Id))
                    continue;

            if (CheckProjectileInHitRange(entity.Position.X, entity.Position.Y, PROJECTILE_RADIUS_PLAYER) &&
                CheckServerEntityHit(entity, serverTime))
                HitEntity(entity);
        }
    }

    // EnemyHit packet
    public void CheckClientCollision(CharacterEntity entity, long elapsedLifetimeMs, WorldPosData targetPos)
    {
        var elapsed = startTime + elapsedLifetimeMs;
        Position = PositionAt(elapsed);

        using (TimedLock.Lock(this))
            if (Hit.Contains(entity.Id))
                return;

        if (entity.DistSqr(targetPos.X, targetPos.Y) > 16)
        { // 16 => 4 tiles radius
            Console.WriteLine("Failed enemyhit: entity position check");
            return;
        }

        if (CheckProjectileInHitRange(targetPos.X, targetPos.Y, SERVER_LENIENCY_RADIUS))
            HitEntity(entity);
        else
            Console.WriteLine($"projectile {Id} pos ({Position.X}, {Position.Y}, startPos: {Path.Info.StartPos} angle: {Angle.Rad2Deg()}) target pos ({targetPos.X}, {targetPos.Y})");
    }

    public bool CheckProjectileInHitRange(float x, float y, float radius)
    {
        var xDiff = MathF.Abs(x - Position.X);
        var yDiff = MathF.Abs(y - Position.Y);
        if ((xDiff * xDiff) + (yDiff * yDiff) <= radius)
            return true;
        return false;
    }

    public bool CheckServerEntityHit(CharacterEntity entity, long serverTime)
    {
        if (Dead)
            return false;
        // if this isnt a player, we dont really care about this precision
        if (entity is not Player p)
            return true;

        var pos = p.Position;
        // check if pos.Item1 is within MOVEMENT_TIME_FRAME of serverTime
        if (p.LastMoveAck > serverTime - MOVEMENT_TIME_FRAME &&
            p.LastMoveAck < serverTime + MOVEMENT_TIME_FRAME)
        {
            if (CheckProjectileInHitRange(pos.X, pos.Y,
                    PROJECTILE_RADIUS)) // great. we thought we hit. and we have a movement packet that confirms we do
                return true;
            // okay we thought we hit, and we have a movement packet within a reasonable time frame that suggests we didn't. fine
            return false;
        }

        // okay so, we thought we hit, but we havent received an ack recently enough to confirm it, so store this last movement,
        // then in either MAX_PING_TIME ms or when we next receive a move, validate this collision.
        using (TimedLock.Lock(this))
        {
            _unconfirmedHits.Add((p, new WorldPosData(Position.X, Position.Y), serverTime));
            p.StoreUnconfirmedHit(this);
            Hit.Add(p.Id); // we dont want to store multiple unconfirmed hits from the same projectile
            return false;
        }
    }

    public bool CheckUnconfirmedHit(CharacterEntity hit, float posX, float posY)
    {
        (Player, WorldPosData pos, long) hitData;
        using (TimedLock.Lock(this))
            hitData = _unconfirmedHits.Where(p => p.p == hit).FirstOrDefault();
        var xDiff = MathF.Abs(posX - hitData.pos.X);
        var yDiff = MathF.Abs(posY - hitData.pos.Y);
        if (xDiff < PROJECTILE_RADIUS && yDiff < PROJECTILE_RADIUS)
            return true;
        return false;
    }

    public void RemoveHit(CharacterEntity notHit)
    {
        using (TimedLock.Lock(this))
            Hit.Remove(notHit.Id);
    }

    public long GetUnconfirmedHitTime(Player p)
    {
        using (TimedLock.Lock(this))
            return _unconfirmedHits.Where(player => player.p == p).FirstOrDefault().time;
    }

    public void ConfirmHit(CharacterEntity hit)
    {
        // a player has reported back that a collision did go through, hit that mf
        HitEntity(hit);
        using (TimedLock.Lock(this))
            _unconfirmedHits.RemoveWhere(p => p.p == hit);
    }

    public void TryHitEntity(CharacterEntity entity)
    {
        using (TimedLock.Lock(this))
        {
            var confirmedHits = _unconfirmedHits.RemoveWhere(p => p.p == entity);
            if (confirmedHits != 0)
            {
                HitEntity(entity);
                return;
            }

            if (Hit.Contains(entity.Id))
                return;
        }

        HitEntity(entity);
    }

    public void HitEntity(CharacterEntity entity)
    {
        _onHitEvent?.Invoke(entity, Owner);
        using (TimedLock.Lock(this))
            Hit.Add(entity.Id);
        Owner.Hit(entity, this);
        entity.HitBy(Owner, this);
    }

    public bool ShouldBeRemoved(RealmTime time)
    {
        if (!Dead)
            Dead = time.TotalElapsedMs > Lifetime + 3000; // 3 seconds to account for lag
        return Dead;
    }

    public override int GetDamage()
    {
        return Damage;
    }

    public override void SetDamage(int dmg)
    {
        Damage = dmg;
    }

    public override int GetTotalDamage()
    {
        return (int)((Damage * Bonus.ProportionalBonus) + Bonus.FlatBonus);
    }
}