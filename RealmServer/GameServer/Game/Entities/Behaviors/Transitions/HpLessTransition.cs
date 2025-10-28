namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class HpLessTransition : BehaviorTransition
    {
        private readonly float _threshold;

        public HpLessTransition(float threshold, string targetState, TransitionType transitionType = TransitionType.Random)
            : base(transitionType)
        {
            RegisterTargetStates(targetState);
            _threshold = threshold;
        }

        public override string Tick(Character host, RealmTime time)
        {
            var transition = (float)host.HP / host.MaxHP < _threshold;
            return transition ? GetTargetState() : null;
        }
    }
}