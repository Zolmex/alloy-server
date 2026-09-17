using System;
using System.Numerics;
using System.Xml.Linq;
using Arch.Core;
using Common.Game;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class StayAwayFromInfo {
    public bool FirstTick;
    public int FollowTimer;
    public Entity Target;
    public bool Following => Target != Entity.Null;
}

public record StayAwayFrom : BehaviorScript {
    private readonly float _acquireRadiusSqr;
    private readonly int _cooldownMS;
    private readonly int _cooldownOffsetMS;
    private readonly float _distanceFromTarget;
    private readonly int _followTimeMs;
    private readonly float _speed;
    private readonly string _target;
    private readonly TargetType _targetType;

    public StayAwayFrom(float speed = 1f, float distFromTarget = 2f, float acquireRange = 10f, int cooldownMS = 1000,
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
        var stayAwayFromInfo = host.BehavController.Resources.ResolveResource<StayAwayFromInfo>(this);
        stayAwayFromInfo.FollowTimer = _cooldownOffsetMS == 0 ? _cooldownMS : _cooldownOffsetMS;
        stayAwayFromInfo.FirstTick = true;
        stayAwayFromInfo.Target = Entity.Null;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var stayAwayFromInfo = host.BehavController.Resources.ResolveResource<StayAwayFromInfo>(this);
        if (_cooldownMS >= 0) {
            stayAwayFromInfo.FollowTimer -= time.ElapsedMsDelta;
            if (stayAwayFromInfo.FollowTimer <= 0) {
                stayAwayFromInfo.Target = host.World.GetAttackTarget(host.Position.Pos, _acquireRadiusSqr, _targetType, _target);
                stayAwayFromInfo.FirstTick = true;

                stayAwayFromInfo.FollowTimer = stayAwayFromInfo.Following ? _followTimeMs : _cooldownMS;

                if (!stayAwayFromInfo.Following)
                    return BehaviorTickState.BehaviorDeactivate;
            }
        }

        if (stayAwayFromInfo.Following) {
            ref var targetPos = ref host.World.Ecs.Get<Position>(stayAwayFromInfo.Target);
            var distToTarget = host.Position.DistSqr(ref targetPos);
            if (distToTarget == 0f || distToTarget > _distanceFromTarget)
                return BehaviorTickState.BehaviorFailed;

            var angle = host.Position.GetAngleBetween(targetPos.Pos);
            var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            var speed = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            dist *= -speed;
            var newX = host.Position.Pos.X + dist.X;
            var newY = host.Position.Pos.Y + dist.Y;
            host.Position.Move(newX, newY);
            return BehaviorTickState.BehaviorActive;
        }

        return BehaviorTickState.OnCooldown;
    }
}