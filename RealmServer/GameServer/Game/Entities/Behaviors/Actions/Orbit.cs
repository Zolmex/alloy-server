#region

using Common;
using System;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class OrbitInfo
    {
        public float FinalRadius;
        public float FinalSpeed;
        public int Direction;
        public bool FirstTick;
        public Entity Target;
    }

    public record Orbit : BehaviorScript
    {
        private readonly float _acquireRange;
        private readonly float _radius;
        private readonly float _radiusVariance;
        private readonly float _speed;
        private readonly float _speedVariance;
        private readonly bool _orbitClockwise;
        private readonly string _target;
        private readonly bool _targetPlayer;

        public Orbit(float speed, float radius, float acquireRange = 10, string target = null,
            float speedVariance = 0.0f, float radiusVariance = 0.0f, bool orbitClockwise = false, bool targetPlayer = false)
        {
            _speed = speed;
            _radius = radius;
            _radiusVariance = radiusVariance;
            _acquireRange = acquireRange;
            _speedVariance = speedVariance;
            _orbitClockwise = orbitClockwise;
            _target = target;
            _targetPlayer = targetPlayer;
        }

        public override void Start(Character host)
        {
            var orbitInfo = host.ResolveResource<OrbitInfo>(this);
            orbitInfo.Direction = _orbitClockwise ? 1 : -1;
            orbitInfo.FinalSpeed = _speed + (_speedVariance * (float)((Random.Shared.NextDouble() * 2) - 1));
            orbitInfo.FinalRadius = _radius + (_radiusVariance * (float)((Random.Shared.NextDouble() * 2) - 1));
            orbitInfo.FirstTick = true;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var orbitInfo = host.ResolveResource<OrbitInfo>(this);
            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return BehaviorTickState.BehaviorFailed;

            Entity target = null;

            if (_targetPlayer)
                target = host.GetNearestPlayer(_acquireRange * _acquireRange);
            else
                target = orbitInfo.Target ?? host.GetNearestOtherEnemyByName(_target, _acquireRange);

            orbitInfo.Target = target;

            if (target == null || target.Dead)
            {
                orbitInfo.Target = null;
                return BehaviorTickState.BehaviorFailed;
            }

            var angle = host.Position.Y == target.Position.Y && host.Position.X == target.Position.X
                ? Math.Atan2(host.Position.Y - target.Position.Y + ((Random.Shared.NextDouble() * 2) - 1), host.Position.X - target.Position.X + ((Random.Shared.NextDouble() * 2) - 1))
                : Math.Atan2(host.Position.Y - target.Position.Y, host.Position.X - target.Position.X);
            var angularSpd = orbitInfo.Direction * host.GetSpeed(orbitInfo.FinalSpeed) / orbitInfo.FinalRadius;

            angle += angularSpd * (time.ElapsedMsDelta / 1000f);

            var x = target.Position.X + (Math.Cos(angle) * orbitInfo.FinalRadius);
            var y = target.Position.Y + (Math.Sin(angle) * orbitInfo.FinalRadius);
            var vect = new Vector2((float)x, (float)y) - new Vector2(host.Position.X, host.Position.Y);
            vect = Vector2.Normalize(vect);
            vect *= host.GetSpeed(orbitInfo.FinalSpeed) * (time.ElapsedMsDelta / 1000f);

            host.MoveRelative(vect.X, vect.Y);

            if (orbitInfo.FirstTick)
            {
                orbitInfo.FirstTick = false;
                return BehaviorTickState.BehaviorActivate;
            }

            return BehaviorTickState.BehaviorActive;
        }
    }
}