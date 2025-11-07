#region

using Common;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions;

public class AOEInfo
{
    public List<AOEDamager> AoeDamagerList = new();
    public int CooldownLeft;
    public float AngleOffset;
}

public record AOE : BehaviorScript
{
    private readonly float _range;
    private readonly float _rangeSqr;
    private readonly float _radius;
    private readonly int _minDamage;
    private readonly int _maxDamage;
    private readonly float _fixedAngle;
    private readonly float _angleOffsetDefault;
    private readonly int _cooldownMS;
    private readonly int _cooldownOffset;
    private readonly int _color;
    private readonly TargetType _targetType;
    private readonly int _activateCount;
    private readonly int _throwTime;
    private readonly int _damageCooldown;
    private readonly int _damageColor;
    private readonly float _rotateAngle;
    private readonly (ConditionEffectIndex, int)[] _effects;

    public AOE(float radius, int damage, int cooldownMs, float range = 12f, int cooldownOffset = 0, int color = 0xFF0000, TargetType targetType = TargetType.ClosestPlayer,
        float fixedAngle = 0, float angleOffset = 0, int activateCount = 1, int throwTime = 1500, int damageCooldown = 1000, int damageColor = 0xFF0000, float rotateAngle = 0f,
        (ConditionEffectIndex, int)[] effects = null)
    {
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

    public override void Start(Character host)
    {
        var aoeInfo = host.ResolveResource<AOEInfo>(this);
        aoeInfo.CooldownLeft = _cooldownOffset;
        aoeInfo.AngleOffset = 0f;
    }

    public override BehaviorTickState Tick(Character host, RealmTime time)
    {
        var aoeInfo = host.ResolveResource<AOEInfo>(this);
        if (aoeInfo.CooldownLeft > 0)
        {
            aoeInfo.CooldownLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        aoeInfo.AoeDamagerList = aoeInfo.AoeDamagerList.Where(x => x.IsActive).ToList();

        //if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
        //    return;

        var startAngle = _fixedAngle;
        var throwDist = _range;
        if (_targetType != TargetType.FixedAngle)
        {
            var attackTarget = host.GetAttackTarget(_rangeSqr, _targetType);
            if (attackTarget == null)
                return BehaviorTickState.BehaviorFailed;

            startAngle = (float)Math.Atan2(attackTarget.Position.Y - host.Position.Y, attackTarget.Position.X - host.Position.X);
            throwDist = MathF.Min(_range, host.GetDistanceBetweenF(attackTarget));
        }

        aoeInfo.AngleOffset += _rotateAngle;
        startAngle += _angleOffsetDefault;
        startAngle += aoeInfo.AngleOffset;

        // TODO: predictive code

        var aoeX = host.Position.X + (MathF.Cos(startAngle) * throwDist);
        var aoeY = host.Position.Y + (MathF.Sin(startAngle) * throwDist);
        foreach (var plr in host.World.GetAllPlayersWithin(host.Position.X, host.Position.Y, 32f))
        {

            plr.User.SendPacket(new
            ShowEffect(
                (byte)ShowEffectIndex.Throw,
                host.Id,
                _color,
                _throwTime,
                new WorldPosData(aoeX, aoeY),
                new WorldPosData()));
        }

        var dmg = (short)host.Rand.Next(_minDamage, _maxDamage);
        aoeInfo.AoeDamagerList.Add(new AOEDamager(host.World, dmg, _throwTime, _damageCooldown, _activateCount, _damageColor, new Vector2(aoeX, aoeY), _radius, _effects));
        aoeInfo.CooldownLeft = _cooldownMS;
        return BehaviorTickState.BehaviorActive;
    }
}

public class AOEDamager
{
    public World World;
    public short Damage;
    public int CooldownMS;
    public int ActivateCount;
    private int _activateCount;
    public int? Color;
    public Vector2 Pos;
    public float Radius;
    public (ConditionEffectIndex, int)[] Effects;
    public bool IsActive = true;

    public AOEDamager(World world, short damage, int cooldown, int damageCooldown, int activateCount, int? color, Vector2 pos, float radius,
        (ConditionEffectIndex, int)[] effects = null)
    {
        World = world;
        Damage = damage;
        CooldownMS = damageCooldown;
        ActivateCount = activateCount;
        Color = color;
        Pos = pos;
        Radius = radius;
        Effects = effects;

        RealmManager.AddTimedAction(cooldown, AOEActivate);
    }

    public void AOEActivate()
    {
        foreach (var plr in World.GetAllPlayersWithin(Pos.X, Pos.Y, Radius))
            plr.DamageWithText(Damage, effects: Effects);

        if (Color.HasValue)
            foreach (var plr in World.GetAllPlayersWithin(Pos.X, Pos.Y, 32f))
            {
                plr.User.SendPacket(new ShowEffect(
                    (byte)ShowEffectIndex.Nova,
                    -1,
                    Color.Value,
                    Radius,
                    new WorldPosData(Pos.X, Pos.Y),
                    new WorldPosData()));
            }

        if (++_activateCount < ActivateCount)
        {
            IsActive = false;
            RealmManager.AddTimedAction(CooldownMS, AOEActivate);
        }
    }
}