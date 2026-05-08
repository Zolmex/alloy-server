using System;
using System.Numerics;
using System.Xml.Linq;
using Common.Game;
using Common.Utilities;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class FollowInfo {
    public bool FirstTick;
    public int FollowTimer;
    public int TargetId;
    public bool Following => TargetId != -1;
}

public record Follow : BehaviorScript {
    private readonly float _acquireRadiusSqr;
    private readonly int _cooldownMS;
    private readonly int _cooldownOffsetMS;
    private readonly float _distanceFromTarget;
    private readonly int _followTimeMs;
    private readonly float _speed;
    private readonly string _target;
    private readonly TargetType _targetType;

    public Follow(float speed = 1f, float distFromTarget = 2f, float acquireRange = 10f, int cooldownMS = 1000,
        int cooldownOffsetMS = 0, int followTimeMS = 1000, TargetType targetType = TargetType.ClosestPlayer,
        string target = "player") {
        _speed = speed;
        _distanceFromTarget = distFromTarget * distFromTarget;
        _acquireRadiusSqr = acquireRange * acquireRange;
        _cooldownMS = cooldownMS;
        _cooldownOffsetMS = cooldownOffsetMS;
        _followTimeMs = followTimeMS;
        _targetType = targetType;
        _target = target;
    }

    public override void Start(ref EntityView host) {
        var followInfo = host.Behavior.Resources.ResolveResource<FollowInfo>(this);
        followInfo.FollowTimer = _cooldownOffsetMS == 0 ? _cooldownMS : _cooldownOffsetMS;
        followInfo.FirstTick = true;
        followInfo.TargetId = -1;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var followInfo = host.Behavior.Resources.ResolveResource<FollowInfo>(this);
        if (_cooldownMS >= 0) {
            followInfo.FollowTimer -= time.ElapsedMsDelta;
            if (followInfo.FollowTimer <= 0) {
                followInfo.TargetId = FindTarget(host, _targetType, _acquireRadiusSqr, _target);
                followInfo.FirstTick = true;

                followInfo.FollowTimer = followInfo.Following ? _followTimeMs : _cooldownMS;

                if (!followInfo.Following)
                    return BehaviorTickState.BehaviorDeactivate;
            }
        }

        if (followInfo.Following) {
            ref var targetStats = ref host.World.EntityStats.Get(followInfo.TargetId);
            if (targetStats.Id == 0) {
                followInfo.TargetId = FindTarget(host, _targetType, _acquireRadiusSqr, _target);
                return BehaviorTickState.BehaviorFailed;
            }

            var distToTarget = host.Stats.DistSqr(ref targetStats);
            if (distToTarget == 0f || distToTarget < _distanceFromTarget)
                return BehaviorTickState.BehaviorFailed;

            var angle = host.Stats.GetAngleBetween(ref targetStats);
            var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            var speed = host.Stats.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            dist *= speed;
            host.Stats.Move(host.Stats.Pos + dist);

            if (followInfo.FirstTick) {
                followInfo.FirstTick = false;
                return BehaviorTickState.BehaviorActivate;
            }

            return BehaviorTickState.BehaviorActive;
        }

        return BehaviorTickState.OnCooldown;
    }

    public static int FindTarget(in EntityView host, TargetType targetType, float acquireRadiusSqr,
        string target = "player") {
        switch (targetType) {
            case TargetType.ClosestPlayer:
                return host.World.Map.GetNearestPlayer(host.Stats.Pos, acquireRadiusSqr);
            case TargetType.RandomPlayerPerBehavior:
                return host.World.Map.GetPlayersWithin(host.Stats.Pos, acquireRadiusSqr).RandomElement();
            case TargetType.Entity:
                return host.World.Map.GetNearestEntityByName(target, host.Stats.Pos, acquireRadiusSqr);
            case TargetType.FarthestPlayer:
                return host.World.Map.GetFarthestPlayer(host.Stats.Pos, acquireRadiusSqr);
        }
        return -1;
    }
}