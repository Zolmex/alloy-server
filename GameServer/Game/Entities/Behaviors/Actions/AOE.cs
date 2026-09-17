using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Arch.Core;
using Common;
using Common.Game;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Utilities;
using World = GameServer.Game.Worlds.World;

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

    public override void Start(ref EntityContext host) {
        var aoeInfo = host.BehavController.Resources.ResolveResource<AOEInfo>(this);
        aoeInfo.CooldownLeft = _cooldownOffset;
        aoeInfo.AngleOffset = 0f;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var aoeInfo = host.BehavController.Resources.ResolveResource<AOEInfo>(this);
        if (aoeInfo.CooldownLeft > 0) {
            aoeInfo.CooldownLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        aoeInfo.AoeDamagerList = aoeInfo.AoeDamagerList.Where(x => x.IsActive).ToList();

        //if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
        //    return;

        var startAngle = _fixedAngle;
        var throwDist = _range;
        ref var hostPos = ref host.Position;
        if (_targetType != TargetType.FixedAngle) {
            var attackTarget = host.World.GetAttackTarget(hostPos.Pos, _rangeSqr, _targetType);
            if (attackTarget == Entity.Null)
                return BehaviorTickState.BehaviorFailed;

            ref var attackTargetPos = ref host.World.Ecs.Get<Position>(attackTarget);
            startAngle = (float)Math.Atan2(attackTargetPos.Pos.Y - hostPos.Pos.Y,
                attackTargetPos.Pos.X - hostPos.Pos.X);
            throwDist = MathF.Min(_range, hostPos.GetDistanceBetween(ref attackTargetPos));
        }

        aoeInfo.AngleOffset += _rotateAngle;
        startAngle += _angleOffsetDefault;
        startAngle += aoeInfo.AngleOffset;

        // TODO: predictive code

        var aoeX = hostPos.Pos.X + MathF.Cos(startAngle) * throwDist;
        var aoeY = hostPos.Pos.Y + MathF.Sin(startAngle) * throwDist;
        foreach (var plrId in host.World.Map.GetPlayersWithin(hostPos.Pos.X, hostPos.Pos.Y, 32f)) {
            var user = host.World.Users[plrId];
            user.SendPacket(new
                ShowEffect(
                    (byte)ShowEffectIndex.Throw,
                    (EntityId)host.Entity,
                    _color,
                    _throwTime,
                    new WorldPosData(aoeX, aoeY),
                    new WorldPosData()));
        }

        var dmg = (short)Random.Shared.Next(_minDamage, _maxDamage);
        aoeInfo.AoeDamagerList.Add(new AOEDamager(host.Entity, host.World, dmg, _throwTime, _damageCooldown, _activateCount,
            _damageColor, new Vector2(aoeX, aoeY), _radius, _effects));
        aoeInfo.CooldownLeft = _cooldownMS;
        return BehaviorTickState.BehaviorActive;
    }
}

public class AOEDamager {
    private Entity _host;
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

    public AOEDamager(Entity host, World world, short damage, int cooldown, int damageCooldown, int activateCount, int? color,
        Vector2 pos, float radius,
        (ConditionEffectIndex, int)[] effects = null) {
        _host = host;
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
        foreach (var plr in World.Map.GetPlayersWithin(Pos.X, Pos.Y, Radius)) {
            World.DamageSystem.Damage(new DamageRecord(_host, plr, Damage));
        }

        if (Color.HasValue)
            foreach (var user in World.Map.GetUsersWithin(Pos.X, Pos.Y, 32f))
                user.SendPacket(new ShowEffect(
                    (byte)ShowEffectIndex.Nova,
                    EntityId.Null,
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