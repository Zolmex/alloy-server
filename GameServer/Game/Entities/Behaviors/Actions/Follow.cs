using System;
using System.Numerics;
using System.Xml.Linq;
using Common.Game;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Utilities;
using Entity = Arch.Core.Entity;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class FollowInfo {
    public bool FirstTick;
    public int FollowTimer;
    public Entity Target;
    public bool Following => Target != Entity.Null;
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

    public override void Start(ref EntityContext host) {
        var followInfo = host.BehavController.Resources.ResolveResource<FollowInfo>(this);
        followInfo.FollowTimer = _cooldownOffsetMS == 0 ? _cooldownMS : _cooldownOffsetMS;
        followInfo.FirstTick = true;
        followInfo.Target = Entity.Null;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var followInfo = host.BehavController.Resources.ResolveResource<FollowInfo>(this);
        if (_cooldownMS >= 0) {
            followInfo.FollowTimer -= time.ElapsedMsDelta;
            if (followInfo.FollowTimer <= 0) {
                followInfo.Target = host.World.GetAttackTarget(host.Position.Pos, _acquireRadiusSqr, _targetType, _target);
                followInfo.FirstTick = true;

                followInfo.FollowTimer = followInfo.Following ? _followTimeMs : _cooldownMS;

                if (!followInfo.Following)
                    return BehaviorTickState.BehaviorDeactivate;
            }
        }

        if (followInfo.Following) {
            ref var targetPos = ref host.World.Ecs.Get<Position>(followInfo.Target);
            var distToTarget = host.Position.DistSqr(ref targetPos);
            if (distToTarget == 0f || distToTarget < _distanceFromTarget)
                return BehaviorTickState.BehaviorFailed;

            var angle = host.Position.GetAngleBetween(targetPos.Pos);
            var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            var speed = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            dist *= speed;
            host.Position.Move((Vector2)host.Position.Pos + dist);

            if (followInfo.FirstTick) {
                followInfo.FirstTick = false;
                return BehaviorTickState.BehaviorActivate;
            }

            return BehaviorTickState.BehaviorActive;
        }

        return BehaviorTickState.OnCooldown;
    }
}