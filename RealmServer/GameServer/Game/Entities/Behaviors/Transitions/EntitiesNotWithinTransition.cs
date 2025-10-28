#region

using System.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class EntitiesNotWithinTransition : BehaviorTransition
    {
        private readonly string[] _targets;
        private readonly float _radius;

        public EntitiesNotWithinTransition(float radius, string targetStates, params string[] targets)
            : base(TransitionType.Random)
        {
            RegisterTargetStates(targetStates);
            _targets = targets;
            _radius = radius;
        }

        public override string Tick(Character host, RealmTime time)
        {
            if (_targets == null)
            {
                if (!host.GetEnemiesWithin(_radius).Any())
                    return GetTargetState();
                return null;
            }

            if (!host.GetOtherEnemiesByName(_targets, _radius).Any())
            {
                return GetTargetState();
            }

            return null;
        }
    }
}