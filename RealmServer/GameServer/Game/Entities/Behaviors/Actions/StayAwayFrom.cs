#region

using Common.Utilities;
using System;
using System.Numerics;
using System.Xml.Linq;
using static GameServer.Game.Entities.Behaviors.Actions.Follow;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class StayAwayFromInfo
    {
        public int FollowTimer;
        public bool FirstTick;
        public int TargetId;
        public bool Following => TargetId != -1;
    }

    public record StayAwayFrom : BehaviorScript
    {
        private readonly float _speed;
        private readonly float _distanceFromTarget;
        private readonly float _acquireRadiusSqr;
        private readonly int _cooldownMS;
        private readonly int _cooldownOffsetMS;
        private readonly int _followTimeMs;
        private readonly TargetType _targetType;
        private readonly string _target;

        public StayAwayFrom(XElement xml)
        {
            _speed = xml.GetAttribute<float>("speed", 1f);
            _distanceFromTarget = xml.GetAttribute<float>("distFromTarget", 2f);
            _distanceFromTarget *= _distanceFromTarget;
            _acquireRadiusSqr = xml.GetAttribute<float>("acquireRange", 10f);
            _acquireRadiusSqr *= _acquireRadiusSqr;
            _cooldownMS = xml.GetAttribute<int>("cooldownMS", 1000);
            _cooldownOffsetMS = xml.GetAttribute<int>("cooldownOffsetMS", 0);
            _followTimeMs = xml.GetAttribute<int>("followTimeMS", 1000);
        }

        public StayAwayFrom(float speed = 1f, float distFromTarget = 2f, float acquireRange = 10f, int cooldownMS = 1000, int cooldownOffsetMS = 0, int followTimeMS = 1000, TargetType targetType = TargetType.ClosestPlayer,
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

        public override void Start(Character host)
        {
            var stayAwayFromInfo = host.ResolveResource<StayAwayFromInfo>(this);
            stayAwayFromInfo.FollowTimer = _cooldownOffsetMS == 0 ? _cooldownMS : _cooldownOffsetMS;
            stayAwayFromInfo.FirstTick = true;
            stayAwayFromInfo.TargetId = -1;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var stayAwayFromInfo = host.ResolveResource<StayAwayFromInfo>(this);
            if (_cooldownMS >= 0)
            {
                stayAwayFromInfo.FollowTimer -= time.ElapsedMsDelta;
                if (stayAwayFromInfo.FollowTimer <= 0)
                {
                    stayAwayFromInfo.TargetId = FindTarget(host, _targetType, _acquireRadiusSqr, _target);
                    stayAwayFromInfo.FirstTick = true;

                    stayAwayFromInfo.FollowTimer = stayAwayFromInfo.Following ? _followTimeMs : _cooldownMS;

                    if (!stayAwayFromInfo.Following)
                        return BehaviorTickState.BehaviorDeactivate;
                }
            }

            if (stayAwayFromInfo.Following)
            {
                if (!host.World.Entities.TryGetValue(stayAwayFromInfo.TargetId, out var target))
                {
                    stayAwayFromInfo.TargetId = FindTarget(host, _targetType, _acquireRadiusSqr, _target);
                    return BehaviorTickState.BehaviorFailed;
                }

                var distToTarget = host.DistSqr(target);
                if (distToTarget == 0f || distToTarget > _distanceFromTarget)
                    return BehaviorTickState.BehaviorFailed;

                var angle = host.GetAngleBetween(target);
                var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                var speed = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
                dist *= -speed;
                host.MoveRelative(dist);
                return BehaviorTickState.BehaviorActive;
            }
            else
            {
                return BehaviorTickState.OnCooldown;
            }
        }
    }
}