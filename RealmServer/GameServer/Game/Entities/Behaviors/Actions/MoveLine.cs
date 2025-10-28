#region

using Common;
using Common.Utilities;
using System;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class MoveLineInfo
    {
        public float DistLeft;
    }

    public record MoveLine : BehaviorScript
    {
        private readonly float _angle;
        private readonly float _distance;
        private readonly float _speed;

        public MoveLine(float speed, float angle = 0, float distance = 0)
        {
            _speed = speed;
            _angle = angle.Deg2Rad();
            _distance = distance;
        }

        public override void Start(Character host)
        {
            var state = host.ResolveResource<MoveLineInfo>(this);
            state.DistLeft = _distance;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<MoveLineInfo>(this);
            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return BehaviorTickState.BehaviorFailed;

            var vect = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
            vect += host.Position.ToVec2();
            if (state.DistLeft > 0)
            {
                var moveDist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
                state.DistLeft -= moveDist;
            }

            host.MoveTowards(time, vect, _speed);

            return BehaviorTickState.BehaviorActive;
        }
    }
}