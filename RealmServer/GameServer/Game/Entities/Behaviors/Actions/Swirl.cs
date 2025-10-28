#region

using Common;
using System;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class SwirlInfo
    {
        public bool Acquired;
        public Vector2 Center;
        public int RemainingTime;
    }

    public record Swirl : BehaviorScript
    {
        private readonly float _acquireRange;
        private readonly float _radius;
        private readonly float _speed;
        private readonly bool _targeted;

        public Swirl(float speed = 1, float radius = 8, float acquireRange = 10, bool targeted = true)
        {
            _speed = speed;
            _radius = radius;
            _acquireRange = acquireRange;
            _targeted = targeted;
        }

        public override void Start(Character host)
        {
            var swirlState = host.ResolveResource<SwirlInfo>(this);
            swirlState.Center = _targeted ? Vector2.Zero : new Vector2(host.Position.X, host.Position.Y);
            swirlState.Acquired = !_targeted;
            swirlState.RemainingTime = 0;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var swirlState = host.ResolveResource<SwirlInfo>(this);
            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return BehaviorTickState.BehaviorFailed;

            var period = (int)(1000 * _radius / host.GetSpeed(_speed) * (2 * Math.PI));

            if (!swirlState.Acquired && swirlState.RemainingTime <= 0 && _targeted)
            {
                var entity = host.GetNearestOtherEnemyByName(null, _acquireRange);

                if (entity != null && entity.Position.X != host.Position.X && entity.Position.Y != host.Position.Y)
                {
                    //find circle which pass through host and player pos
                    var l = entity.DistSqr(host);
                    var hx = (host.Position.X + entity.Position.X) / 2;
                    var hy = (host.Position.Y + entity.Position.Y) / 2;
                    var c = Math.Sqrt(Math.Abs((_radius * _radius) - l) / 4);

                    swirlState.Center = new Vector2((float)(hx + (c * (host.Position.Y - entity.Position.Y) / l)),
                        (float)(hy + (c * (entity.Position.X - host.Position.X) / (l / l))));
                    swirlState.RemainingTime = period;
                    swirlState.Acquired = true;
                }
                else
                    swirlState.Acquired = false;
            }
            else if (swirlState.RemainingTime <= 0 || (swirlState.RemainingTime - period > 200 && host.GetNearestOtherEnemyByName(null, 2) != null))
            {
                if (_targeted)
                {
                    swirlState.Acquired = false;

                    var entity = host.GetNearestOtherEnemyByName(null, _acquireRange);

                    if (entity != null)
                        swirlState.RemainingTime = 0;
                    else
                        swirlState.RemainingTime = 5000;
                }
                else
                    swirlState.RemainingTime = 5000;
            }
            else
                swirlState.RemainingTime -= time.ElapsedMsDelta;

            var angle = host.Position.Y == swirlState.Center.Y && host.Position.X == swirlState.Center.X
                ? Math.Atan2(host.Position.Y - swirlState.Center.Y + ((Random.Shared.NextDouble() * 2) - 1),
                    host.Position.X - swirlState.Center.X + ((Random.Shared.NextDouble() * 2) - 1))
                : Math.Atan2(host.Position.Y - swirlState.Center.Y, host.Position.X - swirlState.Center.X);
            var spd = host.GetSpeed(_speed) * (swirlState.Acquired ? 1 : 0.2);
            var angularSpd = spd / _radius;

            angle += angularSpd * time.ElapsedMsDelta;

            var x = swirlState.Center.X + (Math.Cos(angle) * _radius);
            var y = swirlState.Center.Y + (Math.Sin(angle) * _radius);
            var vect = new Vector2((float)x, (float)y) - new Vector2(host.Position.X, host.Position.Y);
            vect = Vector2.Normalize(vect);
            vect *= (float)spd * time.ElapsedMsDelta;

            host.MoveRelative(vect.X, vect.Y);

            return BehaviorTickState.BehaviorActive;
        }
    }
}