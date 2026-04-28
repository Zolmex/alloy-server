#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.Projectiles.ProjectilePaths;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using GameServerOld.Game.Entities.Types;

#endregion

namespace GameServerOld.Game.Entities.DamageSources.Types;

public class Projectile : DamageSource, IIdentifiable {
    public enum ProjectileTargetType {
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
    public readonly HashSet<int> Hit;
    public readonly CharacterEntity Owner;
    public readonly ProjectilePath Path;
    public readonly Vector2 StartPosition;
    private readonly long startTime;
    public readonly ProjectileTargetType TargetType;
    public bool ArmorPiercing;
    public int Damage;
    public bool Dead;
    public long Lifetime;
    public bool MultiHit;
    public bool PassesCover;

    public Vector2 Position;

    public long Time;

    public Projectile(CharacterEntity owner, int id, long time, float angle, Vector2 startPos, int damage,
        ProjectilePath path, ProjectileTargetType targetType,
        Action<CharacterEntity, CharacterEntity> onHitEvent = null) {
        Owner = owner;
        Id = id;
        Angle = angle.Deg2Rad();
        StartPosition = startPos;
        Position = startPos;
        Damage = damage;
        Hit = new HashSet<int>();
        TargetType = targetType;
        startTime = time;

        Path = path;

        _onHitEvent = onHitEvent;
    }


    public int Id { get; set; }

    public void SetProps(ProjectileProps props) {
        Lifetime = startTime + props.LifetimeMS;
        MultiHit = props.MultiHit;
        PassesCover = props.PassesCover;
        ArmorPiercing = props.ArmorPiercing;
        Effects = props.Effects;

        var pathInfo = new ProjectileInfo
            { StartPos = StartPosition, ShootAngle = Angle, LifetimeMs = props.LifetimeMS, ProjId = Id };
        Path.SetInfo(pathInfo);
    }

    public void SetProps(ProjectileDesc desc) {
        Lifetime = startTime + desc.LifetimeMS;
        MultiHit = desc.MultiHit;
        PassesCover = desc.PassesCover;
        ArmorPiercing = desc.ArmorPiercing;
        Effects = desc.Effects.Select(i => (i.Effect, i.DurationMS)).ToArray();

        var pathInfo = new ProjectileInfo
            { StartPos = StartPosition, ShootAngle = Angle, LifetimeMs = desc.LifetimeMS, ProjId = Id };
        Path.SetInfo(pathInfo);
    }

    public bool CanHit(Entity en) {
        // if (en.HasConditionEffect(ConditionEffectIndex.Invincible) || en.HasConditionEffect(ConditionEffectIndex.Stasis))
        //    return false;

        return !Hit.Add(en.Id);
    }

    public Vector2 PositionAt(long totalElapsedMs) {
        Vector2 moveVec;
        if (totalElapsedMs > Lifetime) // Projectile lifetime grace period
            moveVec = Path.PositionAt((int)(Lifetime - startTime));
        else
            moveVec = Path.PositionAt((int)(totalElapsedMs - startTime));

        return Path.Info.StartPos + moveVec;
    }

    // Server-side projectile collision
    public void CheckCollisions(IEnumerable<CharacterEntity> entitiesInRadius, long serverTime) {
        foreach (var entity in entitiesInRadius) {
            if (Hit.Contains(entity.Id))
                continue;

            if (CheckProjectileInHitRange(entity.Position.X, entity.Position.Y, PROJECTILE_RADIUS_PLAYER) &&
                CheckServerEntityHit(entity, serverTime))
                HitEntity(entity);
        }
    }

    // EnemyHit packet
    public void CheckClientCollision(CharacterEntity entity, long elapsedLifetimeMs, WorldPosData targetPos) {
        var elapsed = startTime + elapsedLifetimeMs;
        Position = PositionAt(elapsed);

        if (Hit.Contains(entity.Id))
            return;

        if (entity.DistSqr(targetPos.X, targetPos.Y) > 16) { // 16 => 4 tiles radius
            Console.WriteLine("Failed enemyhit: entity position check");
            return;
        }

        if (CheckProjectileInHitRange(targetPos.X, targetPos.Y, SERVER_LENIENCY_RADIUS))
            HitEntity(entity);
        else
            Console.WriteLine(
                $"projectile {Id} pos ({Position.X}, {Position.Y}, startPos: {Path.Info.StartPos} angle: {Angle.Rad2Deg()}) target pos ({targetPos.X}, {targetPos.Y})");
    }

    public bool CheckProjectileInHitRange(float x, float y, float radius) {
        var xDiff = MathF.Abs(x - Position.X);
        var yDiff = MathF.Abs(y - Position.Y);
        if (xDiff * xDiff + yDiff * yDiff <= radius)
            return true;
        return false;
    }

    public bool CheckServerEntityHit(CharacterEntity entity, long serverTime) {
        if (Dead)
            return false;
        // if this isnt a player, we dont really care about this precision
        if (entity is not Player p)
            return true;

        var pos = p.Position;
        // check if pos.Item1 is within MOVEMENT_TIME_FRAME of serverTime
        if (p.LastMoveAck > serverTime - MOVEMENT_TIME_FRAME &&
            p.LastMoveAck < serverTime + MOVEMENT_TIME_FRAME) {
            if (CheckProjectileInHitRange(pos.X, pos.Y,
                    PROJECTILE_RADIUS)) // great. we thought we hit. and we have a movement packet that confirms we do
                return true;
            // okay we thought we hit, and we have a movement packet within a reasonable time frame that suggests we didn't. fine
            return false;
        }

        // okay so, we thought we hit, but we havent received an ack recently enough to confirm it, so store this last movement,
        // then in either MAX_PING_TIME ms or when we next receive a move, validate this collision.
        _unconfirmedHits.Add((p, new WorldPosData(Position.X, Position.Y), serverTime));
        p.StoreUnconfirmedHit(this);
        Hit.Add(p.Id); // we dont want to store multiple unconfirmed hits from the same projectile
        return false;
    }

    public bool CheckUnconfirmedHit(CharacterEntity hit, float posX, float posY) {
        (Player, WorldPosData pos, long) hitData;
        hitData = _unconfirmedHits.FirstOrDefault(p => p.p == hit);
        var xDiff = MathF.Abs(posX - hitData.pos.X);
        var yDiff = MathF.Abs(posY - hitData.pos.Y);
        if (xDiff < PROJECTILE_RADIUS && yDiff < PROJECTILE_RADIUS)
            return true;
        return false;
    }

    public void RemoveHit(CharacterEntity notHit) {
        Hit.Remove(notHit.Id);
    }

    public long GetUnconfirmedHitTime(Player p) {
        return _unconfirmedHits.FirstOrDefault(player => player.p == p).time;
    }

    public void ConfirmHit(CharacterEntity hit) {
        // a player has reported back that a collision did go through, hit that mf
        HitEntity(hit);
        _unconfirmedHits.RemoveWhere(p => p.p == hit);
    }

    public void TryHitEntity(CharacterEntity entity) {
        var confirmedHits = _unconfirmedHits.RemoveWhere(p => p.p == entity);
        if (confirmedHits != 0) {
            HitEntity(entity);
            return;
        }

        if (Hit.Contains(entity.Id))
            return;

        HitEntity(entity);
    }

    public void HitEntity(CharacterEntity entity) {
        _onHitEvent?.Invoke(entity, Owner);
        Hit.Add(entity.Id);
        Owner.Hit(entity, this);
        entity.HitBy(Owner, this);
    }

    public bool ShouldBeRemoved(RealmTime time) {
        if (!Dead)
            Dead = time.TotalElapsedMs > Lifetime + 3000; // 3 seconds to account for lag
        return Dead;
    }

    public override int GetDamage() {
        return Damage;
    }

    public override void SetDamage(int dmg) {
        Damage = dmg;
    }

    public override int GetTotalDamage() {
        return (int)(Damage * Bonus.ProportionalBonus + Bonus.FlatBonus);
    }
}