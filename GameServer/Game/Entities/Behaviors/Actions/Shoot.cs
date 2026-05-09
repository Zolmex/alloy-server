using System;
using System.Collections.Generic;
using Common;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class ShootInfo {
    public float AngleOffset;
    public int CooldownLeft;
}

public record Shoot : BehaviorScript {
    private const int PREDICT_NUM_TICKS = 10;

    public static readonly HashSet<ushort> CustomProjectileOwners = new();

    private static readonly Logger _log = new(typeof(Shoot));
    private static readonly ProjectileProps _bladeProps = new("Blade", 1000, 10, 0);
    private static readonly ProjectilePath _bladePath = new LinePath(3).ToPath();

    private readonly float _angleOffsetDefault;
    private readonly bool _armorPiercing;
    private readonly int _cooldownMs;
    private readonly int _cooldownOffsetMs;
    private readonly byte _count;
    private readonly (ConditionEffectIndex, int)[] _effects;
    private readonly float _fixedAngle;
    private readonly int _lifetimeMs;
    private readonly int _maxDamage;
    private readonly float _maxRadiusSqr;
    private readonly int _minDamage;

    private readonly float _minRadiusSqr;
    private readonly bool _multiHit;
    private readonly bool _passesCover;
    private readonly float _predictive;
    private readonly ushort _projType;
    private readonly float _rotateAngle;
    private readonly float _shootAngle;
    private readonly int _size;
    private readonly TargetType _targetType;
    private readonly float _xOffset;
    private readonly float _yOffset;
    private ProjectilePath _path;

    private byte _projectilePropsId = 255; // 255 means we need to cache the custom projectile

    public Shoot(float maxRadius, byte count = 1, float shootAngle = 0f, byte projectilePropsId = 0,
        float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
        float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0, bool targeted = false,
        float xOffset = 0f, float yOffset = 0f, float minRadius = 0) {
        _projectilePropsId = projectilePropsId;
        _maxRadiusSqr = maxRadius * maxRadius;
        _minRadiusSqr = minRadius * minRadius;
        _count = count;
        _shootAngle = shootAngle.Deg2Rad();
        _fixedAngle = fixedAngle.Deg2Rad();
        _rotateAngle = rotateAngle.Deg2Rad();
        _angleOffsetDefault = angleOffset.Deg2Rad();
        _xOffset = xOffset;
        _yOffset = yOffset;
        _predictive = predictive;
        _cooldownOffsetMs = coolDownOffset;
        _cooldownMs = cooldownMS;
        _targetType = targeted ? TargetType.ClosestPlayer : TargetType.FixedAngle;
    }

    public Shoot(float maxRadius, ProjectilePathSegment path, byte count = 1, float shootAngle = 0f,
        ushort projType = 0, float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
        float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0, bool targeted = false,
        string projName = "",
        int lifetimeMs = 1000, int minDamage = -1, int maxDamage = -1, int damage = -1, float xOffset = 0f,
        float yOffset = 0f, int size = 100, bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
        float minRadius = 0,
        params (ConditionEffectIndex, int)[] effects) {
        _maxRadiusSqr = maxRadius * maxRadius;
        _minRadiusSqr = minRadius * minRadius;
        _count = count;
        _shootAngle = shootAngle.Deg2Rad();
        if (string.IsNullOrEmpty(projName)) {
            _projType = projType;
        }
        else {
            var projXml = XmlLibrary.Id2Object(projName);
            if (projXml == null) {
                projXml = XmlLibrary.Id2Object("Blade");
                _log.Warn($"Projectile {projName} not found. Using Blade instead.");
            }

            _projType = projXml.ObjectType;
        }

        _lifetimeMs = lifetimeMs;
        if (damage == -1) {
            _minDamage = minDamage;
            _maxDamage = maxDamage;
        }
        else {
            _minDamage = damage;
            _maxDamage = damage;
        }

        _fixedAngle = fixedAngle.Deg2Rad();
        _rotateAngle = rotateAngle.Deg2Rad();
        _angleOffsetDefault = angleOffset.Deg2Rad();
        _xOffset = xOffset;
        _yOffset = yOffset;
        _predictive = predictive;
        _cooldownOffsetMs = coolDownOffset;
        _cooldownMs = cooldownMS;
        _targetType = targeted ? TargetType.ClosestPlayer : TargetType.FixedAngle;
        path.LifetimeMs = lifetimeMs;
        _path = path.ToPath();
        _size = size;
        _multiHit = multiHit;
        _passesCover = passesCover;
        _armorPiercing = armorPiercing;
        _effects = effects;
    }

    public Shoot(float maxRadius, ProjectilePath path, byte count = 1, float shootAngle = 0f,
        ushort projType = 0, float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
        float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0, bool targeted = false,
        string projName = "", int minDamage = -1, int maxDamage = -1, int damage = -1, float xOffset = 0f,
        float yOffset = 0f, int size = 100, bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
        float minRadius = 0,
        params (ConditionEffectIndex, int)[] effects) {
        _maxRadiusSqr = maxRadius * maxRadius;
        _minRadiusSqr = minRadius * minRadius;
        _count = count;
        _shootAngle = shootAngle.Deg2Rad();
        if (string.IsNullOrEmpty(projName)) {
            _projType = projType;
        }
        else {
            var projXml = XmlLibrary.Id2Object(projName);
            if (projXml == null) {
                projXml = XmlLibrary.Id2Object("Blade");
                _log.Warn($"Projectile {projName} not found. Using Blade instead.");
            }

            _projType = projXml.ObjectType;
        }

        if (damage == -1) {
            _minDamage = minDamage;
            _maxDamage = maxDamage;
        }
        else {
            _minDamage = damage;
            _maxDamage = damage;
        }

        _fixedAngle = fixedAngle.Deg2Rad();
        _rotateAngle = rotateAngle.Deg2Rad();
        _angleOffsetDefault = angleOffset.Deg2Rad();
        _xOffset = xOffset;
        _yOffset = yOffset;
        _predictive = predictive;
        _cooldownOffsetMs = coolDownOffset;
        _cooldownMs = cooldownMS;
        _targetType = targeted ? TargetType.ClosestPlayer : TargetType.FixedAngle;
        _path = path;
        _lifetimeMs = path.LifetimeMs;
        _size = size;
        _multiHit = multiHit;
        _passesCover = passesCover;
        _armorPiercing = armorPiercing;
        _effects = effects;
    }

    public Shoot(float maxRadius, ProjectilePathSegment path, byte count = 1, float shootAngle = 0f,
        ushort projType = 0, float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
        float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0,
        TargetType targetType = TargetType.ClosestPlayer, string projName = "",
        int lifetimeMs = 1000, int minDamage = -1, int maxDamage = -1, int damage = -1, float xOffset = 0f,
        float yOffset = 0f, int size = 100, bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
        float minRadius = 0,
        params (ConditionEffectIndex, int)[] effects) {
        _maxRadiusSqr = maxRadius * maxRadius;
        _minRadiusSqr = minRadius * minRadius;
        _count = count;
        _shootAngle = shootAngle.Deg2Rad();
        if (string.IsNullOrEmpty(projName))
            _projType = projType;
        else
            _projType = XmlLibrary.Id2Object(projName).ObjectType;
        _lifetimeMs = lifetimeMs;
        if (damage == -1) {
            _minDamage = minDamage;
            _maxDamage = maxDamage;
        }
        else {
            _minDamage = damage;
            _maxDamage = damage;
        }

        _fixedAngle = fixedAngle.Deg2Rad();
        _rotateAngle = rotateAngle.Deg2Rad();
        _angleOffsetDefault = angleOffset.Deg2Rad();
        _xOffset = xOffset;
        _yOffset = yOffset;
        _predictive = predictive;
        _cooldownOffsetMs = coolDownOffset;
        _cooldownMs = cooldownMS;
        _targetType = targetType;
        path.LifetimeMs = lifetimeMs;
        _path = path.ToPath();
        _size = size;
        _multiHit = multiHit;
        _passesCover = passesCover;
        _armorPiercing = armorPiercing;
        _effects = effects;
    }

    public override void Setup(ObjectDesc desc) {
        if (_projectilePropsId != 255) // XML defined projectile properties
        {
            if (!desc.Projectiles.TryGetValue(_projectilePropsId, out var props)) {
                // Try to find the projectile property id
                _path = _bladePath;
                _log.Error($"(ObjId {desc.ObjectId}) projectile id {_projectilePropsId} not found. Using Blade instead.");
                return;
            }

            _path = ProjectilePathSegment.ParsePath(props.Props).ToPath();
        }
        else // Custom behavior-defined projectile properties
        {
            var objId = XmlLibrary.ObjectDescs[_projType].ObjectId;
            var props = desc.Projectiles.AddOrGet(objId, _lifetimeMs, 0, _maxDamage, _minDamage,
                _maxDamage, _effects, _multiHit, _passesCover, _armorPiercing, false, false,
                false, 0, 0, 0, _size);
            _projectilePropsId = props.ProjectileIndex;

            CustomProjectileOwners.Add(desc.ObjectType); // Mark this entity for behavior projectile
        }
    }

    public override void Start(ref EntityView host) {
        var shootInfo = host.Behavior.Resources.ResolveResource<ShootInfo>(this);
        shootInfo.CooldownLeft = _cooldownOffsetMs;
        shootInfo.AngleOffset = 0;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var shootInfo = host.Behavior.Resources.ResolveResource<ShootInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Stunned)) // TODO: Condition Effects
        //     return BehaviorTickState.BehaviorFailed;

        if (shootInfo.CooldownLeft > 0) {
            shootInfo.CooldownLeft -= time.ElapsedMsDelta;
            if (shootInfo.CooldownLeft > 0)
                return BehaviorTickState.OnCooldown;
        }

        var startAngle = _fixedAngle;
        if (_targetType != TargetType.FixedAngle) {
            var attackTargetId = host.World.GetAttackTarget(host.Stats.Pos, _maxRadiusSqr, _targetType);
            if (attackTargetId == 0)
                return BehaviorTickState.BehaviorFailed;

            ref var attackTarget = ref host.World.EntityStats.Get(attackTargetId);
            if (_predictive > 0)
                startAngle = Predict(host.Stats.Pos, ref attackTarget);
            else
                startAngle = (float)Math.Atan2(attackTarget.Pos.Y - host.Stats.Pos.Y,
                    attackTarget.Pos.X - host.Stats.Pos.X);
            startAngle -= (_count / 2f - 0.5f) * _shootAngle;
        }

        startAngle += _angleOffsetDefault;
        if (_rotateAngle != 0) {
            startAngle += shootInfo.AngleOffset;
            shootInfo.AngleOffset += _rotateAngle;
        }

        var startPos = new WorldPosData(host.Stats.Pos.X + _xOffset, host.Stats.Pos.Y + _yOffset);
        host.World.ShootProjectiles(startPos, host.Id, host.Entity.ObjectType, _projectilePropsId, startAngle.Rad2Deg(), _minDamage, _maxDamage, _count, _shootAngle.Rad2Deg(), _path, ref time);

        shootInfo.CooldownLeft = _cooldownMs;
        return BehaviorTickState.BehaviorActive;
    }

    private static float Predict(WorldPosData hostPos, ref EntityStats target) {
        var targetX = target.Pos.X + PREDICT_NUM_TICKS * (target.Pos.X - target.PrevPos.X);
        var targetY = target.Pos.Y + PREDICT_NUM_TICKS * (target.Pos.Y - target.PrevPos.Y);
        var angle = MathF.Atan2(targetY - hostPos.Y, targetX - hostPos.X);
        return angle;
    }
}