namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class OnParentDeathInfo
    {
        public bool ParentDead;
    }

    public class OnParentDeathTransition : BehaviorTransition
    {
        public OnParentDeathTransition(string targetState)
            : base(TransitionType.Random)
        {
            RegisterTargetStates(targetState);
        }

        public override void Start(Character host)
        {
            var state = host.ResolveResource<OnParentDeathInfo>(this);
            state.ParentDead = host.Parent.Dead;
            host.Parent.DeathEvent += parent => state.ParentDead = parent.Dead;
        }

        public override string Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<OnParentDeathInfo>(this);
            return state.ParentDead ? GetTargetState() : null;
        }
    }
}