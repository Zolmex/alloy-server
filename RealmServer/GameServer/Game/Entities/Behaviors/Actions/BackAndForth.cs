#region

using Common;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class BackAndForthInfo
    {
        public float Distance;
    }

    public record BackAndForth : BehaviorScript
    {
        private readonly float _distance;
        private readonly float _speed;

        public BackAndForth(float speed, int distance = 5)
        {
            _speed = speed;
            _distance = distance;
        }

        public override void Start(Character host)
        {
            var chargeState = host.ResolveResource<BackAndForthInfo>(this);
            chargeState.Distance = _distance;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var backAndForthState = host.ResolveResource<BackAndForthInfo>(this);
            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return BehaviorTickState.BehaviorFailed;

            var moveDist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            if (backAndForthState.Distance > 0)
            {
                host.MoveRelative(moveDist, 0);
                backAndForthState.Distance -= moveDist;

                if (backAndForthState.Distance <= 0)
                    backAndForthState.Distance = -_distance;
            }
            else
            {
                host.MoveRelative(-moveDist, 0);
                backAndForthState.Distance += moveDist;

                if (backAndForthState.Distance >= 0)
                    backAndForthState.Distance = _distance;
            }

            return BehaviorTickState.BehaviorActive;
        }
    }
}