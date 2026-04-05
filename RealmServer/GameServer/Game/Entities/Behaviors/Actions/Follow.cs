#region

using Common.Utilities;
using System;
using System.Numerics;
using System.Xml.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions;

public class FollowInfo
{
    public bool FirstTick;
    public int FollowTimer;
    public int TargetId;
    public bool Following => TargetId != -1;
}

public record Follow : BehaviorScript
{
    private readonly float _acquireRadiusSqr;
    private readonly int _cooldownMS;
    private readonly int _cooldownOffsetMS;
    private readonly float _distanceFromTarget;
    private readonly int _followTimeMs;
    private readonly float _speed;
    private readonly string _target;
    private readonly TargetType _targetType;

    public Follow(float speed = 1f, float distFromTarget = 2f, float acquireRange = 10f, int cooldownMS = 1000, int cooldownOffsetMS = 0, int followTimeMS = 1000, TargetType targetType = TargetType.ClosestPlayer,
        string target = "player")
    {
        _speed = speed;
        _distanceFromTarget = distFromTarget * distFromTarget;
        _acquireRadiusSqr = acquireRange * acquireRange;
        _cooldownMS = cooldownMS;
        _cooldownOffsetMS = cooldownOffsetMS;
        _followTimeMs = followTimeMS;
        _targetType = targetType;
        _target = target;
    }

    public Follow(XElement xml)
    {
        _speed = xml.GetAttribute("speed", 1f);
        var distFromTarget = xml.GetAttribute("distFromTarget", 2f);
        _distanceFromTarget = distFromTarget * distFromTarget;
        var acquireRadius = xml.GetAttribute("acquireRadius", 10f);
        _acquireRadiusSqr = acquireRadius * acquireRadius;
        _cooldownMS = xml.GetAttribute("cooldownMS", 1000);
        _cooldownOffsetMS = xml.GetAttribute<int>("cooldownOffsetMS");
        _followTimeMs = xml.GetAttribute("followTimeMS", 1000);
        _targetType = Enum.Parse<TargetType>(xml.GetAttribute("targetType", "ClosestPlayer"));
        _target = xml.GetAttribute("target", "player");
    }

    public override void Start(CharacterEntity host)
    {
        var followInfo = host.ResolveResource<FollowInfo>(this);
        followInfo.FollowTimer = _cooldownOffsetMS == 0 ? _cooldownMS : _cooldownOffsetMS;
        followInfo.FirstTick = true;
        followInfo.TargetId = -1;
    }

    public override BehaviorTickState Tick(CharacterEntity host, RealmTime time)
    {
        var followInfo = host.ResolveResource<FollowInfo>(this);
        if (_cooldownMS >= 0)
        {
            followInfo.FollowTimer -= time.ElapsedMsDelta;
            if (followInfo.FollowTimer <= 0)
            {
                followInfo.TargetId = FindTarget(host, _targetType, _acquireRadiusSqr, _target);
                followInfo.FirstTick = true;

                followInfo.FollowTimer = followInfo.Following ? _followTimeMs : _cooldownMS;

                if (!followInfo.Following)
                    return BehaviorTickState.BehaviorDeactivate;
            }
        }

        if (followInfo.Following)
        {
            if (!host.World.Entities.TryGetValue(followInfo.TargetId, out var target))
            {
                followInfo.TargetId = FindTarget(host, _targetType, _acquireRadiusSqr, _target);
                return BehaviorTickState.BehaviorFailed;
            }

            var distToTarget = host.DistSqr(target);
            if (distToTarget == 0f || distToTarget < _distanceFromTarget)
                return BehaviorTickState.BehaviorFailed;

            var angle = host.GetAngleBetween(target);
            var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            var speed = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            dist *= speed;
            host.MoveRelative(dist);

            if (followInfo.FirstTick)
            {
                followInfo.FirstTick = false;
                return BehaviorTickState.BehaviorActivate;
            }

            return BehaviorTickState.BehaviorActive;
        }

        return BehaviorTickState.OnCooldown;
    }

    public static int FindTarget(CharacterEntity host, TargetType targetType, float acquireRadiusSqr, string target = "player")
    {
        switch (targetType)
        {
            case TargetType.ClosestPlayer:
                return host.GetNearestPlayer(acquireRadiusSqr)?.Id ?? -1;
            case TargetType.RandomPlayerPerBehavior:
                return host.GetPlayersWithin(acquireRadiusSqr).RandomElement()?.Id ?? -1;
            case TargetType.Entity:
                return host.GetNearestOtherEnemyByName(target, acquireRadiusSqr)?.Id ?? -1;
            case TargetType.FarthestPlayer:
                return host.GetFarthestPlayer(acquireRadiusSqr)?.Id ?? -1;
        }

        return -1;
    }
}