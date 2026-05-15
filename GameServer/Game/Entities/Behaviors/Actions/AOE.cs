using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common;
using Common.Game;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class AOEInfo {
    public float AngleOffset;
    public List<AOEDamager> AoeDamagerList = new();
    public int CooldownLeft;
}

public record AOE : BehaviorScript {
    private readonly int _activateCount;
    private readonly float _angleOffsetDefault;
    private readonly int _color;
    private readonly int _cooldownMS;
    private readonly int _cooldownOffset;
    private readonly int _damageColor;
    private readonly int _damageCooldown;
    private readonly (ConditionEffectIndex, int)[] _effects;
    private readonly float _fixedAngle;
    private readonly int _maxDamage;
    private readonly int _minDamage;
    private readonly float _radius;
    private readonly float _range;
    private readonly float _rangeSqr;
    private readonly float _rotateAngle;
    private readonly TargetType _targetType;
    private readonly int _throwTime;

    public AOE(float radius, int damage, int cooldownMs, float range = 12f, int cooldownOffset = 0,
        int color = 0xFF0000, TargetType targetType = TargetType.ClosestPlayer,
        float fixedAngle = 0, float angleOffset = 0, int activateCount = 1, int throwTime = 1500,
        int damageCooldown = 1000, int damageColor = 0xFF0000, float rotateAngle = 0f,
        (ConditionEffectIndex, int)[] effects = null) {
        _radius = radius;
        _minDamage = damage;
        _maxDamage = damage;
        _range = range;
        _rangeSqr = range * range;
        _cooldownMS = cooldownMs;
        _cooldownOffset = cooldownOffset;
        _color = color;
        _targetType = targetType;
        _fixedAngle = fixedAngle.Deg2Rad();
        _angleOffsetDefault = angleOffset.Deg2Rad();
        _activateCount = activateCount;
        _throwTime = throwTime;
        _damageCooldown = damageCooldown;
        _damageColor = damageColor;
        _rotateAngle = rotateAngle.Deg2Rad();
        _effects = effects;
    }

    public override void Start(ref EntityView host) {
        var aoeInfo = host.Behavior.Resources.ResolveResource<AOEInfo>(this);
        aoeInfo.CooldownLeft = _cooldownOffset;
        aoeInfo.AngleOffset = 0f;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var aoeInfo = host.Behavior.Resources.ResolveResource<AOEInfo>(this);
        if (aoeInfo.CooldownLeft > 0) {
            aoeInfo.CooldownLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        aoeInfo.AoeDamagerList = aoeInfo.AoeDamagerList.Where(x => x.IsActive).ToList();

        //if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
        //    return;

        var startAngle = _fixedAngle;
        var throwDist = _range;
        if (_targetType != TargetType.FixedAngle) {
            var attackTargetId = host.World.GetAttackTarget(host.Stats.Pos, _rangeSqr, _targetType);
            if (attackTargetId == 0)
                return BehaviorTickState.BehaviorFailed;

            ref var attackTarget = ref host.World.EntityStats.Get(attackTargetId);
            startAngle = (float)Math.Atan2(attackTarget.Pos.Y - host.Stats.Pos.Y,
                attackTarget.Pos.X - host.Stats.Pos.X);
            throwDist = MathF.Min(_range, host.Stats.GetDistanceBetween(ref attackTarget));
        }

        aoeInfo.AngleOffset += _rotateAngle;
        startAngle += _angleOffsetDefault;
        startAngle += aoeInfo.AngleOffset;

        // TODO: predictive code

        var aoeX = host.Stats.Pos.X + MathF.Cos(startAngle) * throwDist;
        var aoeY = host.Stats.Pos.Y + MathF.Sin(startAngle) * throwDist;
        foreach (var plrId in host.World.Map.GetPlayersWithin(host.Stats.Pos.X, host.Stats.Pos.Y, 32f)) {
            var user = host.World.PlayerToUser[plrId];
            user.SendPacket(new
                ShowEffect(
                    (byte)ShowEffectIndex.Throw,
                    host.Id,
                    _color,
                    _throwTime,
                    new WorldPosData(aoeX, aoeY),
                    new WorldPosData()));
        }

        var dmg = (short)Random.Shared.Next(_minDamage, _maxDamage);
        aoeInfo.AoeDamagerList.Add(new AOEDamager(host.Id, host.World, dmg, _throwTime, _damageCooldown, _activateCount,
            _damageColor, new Vector2(aoeX, aoeY), _radius, _effects));
        aoeInfo.CooldownLeft = _cooldownMS;
        return BehaviorTickState.BehaviorActive;
    }
}

public class AOEDamager {
    private int _hostId;
    private int _activateCount;
    public int ActivateCount;
    public int? Color;
    public int CooldownMS;
    public short Damage;
    public (ConditionEffectIndex, int)[] Effects;
    public bool IsActive = true;
    public Vector2 Pos;
    public float Radius;
    public World World;

    public AOEDamager(int hostId, World world, short damage, int cooldown, int damageCooldown, int activateCount, int? color,
        Vector2 pos, float radius,
        (ConditionEffectIndex, int)[] effects = null) {
        _hostId = hostId;
        World = world;
        Damage = damage;
        CooldownMS = damageCooldown;
        ActivateCount = activateCount;
        Color = color;
        Pos = pos;
        Radius = radius;
        Effects = effects;

        world.AddTimedAction(cooldown, AOEActivate);
    }

    public void AOEActivate(World world) {
        foreach (var plrId in World.Map.GetPlayersWithin(Pos.X, Pos.Y, Radius)) {
            ref var plr = ref World.EntityCombat.Get(plrId);
            plr.DamageWithText(_hostId, Damage);
        }

        if (Color.HasValue)
            foreach (var user in World.Map.GetUsersWithin(Pos.X, Pos.Y, 32f))
                user.SendPacket(new ShowEffect(
                    (byte)ShowEffectIndex.Nova,
                    -1,
                    Color.Value,
                    Radius,
                    new WorldPosData(Pos.X, Pos.Y),
                    new WorldPosData()));

        if (++_activateCount < ActivateCount) {
            IsActive = false;
            World.AddTimedAction(CooldownMS, AOEActivate);
        }
    }
}