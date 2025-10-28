namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class TimedTransitionInfo
    {
        public int TimeLeft;
    }

    /// <summary>
    /// A class for transitioning to a given state after a set amount of time.
    /// </summary>
    public class TimedTransition : BehaviorTransition
    {
        private readonly int _timeDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTransition"/> class.
        /// </summary>
        /// <param name="time">Time in milliseconds after which to perform the transition.</param>
        /// <param name="targetState">Target state to be transitioned to.</param>
        public TimedTransition(int time, string targetState)
        {
            RegisterTargetStates(targetState);
            _timeDefault = time;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTransition"/> class.
        /// </summary>
        /// <param name="time">Time in milliseconds after which to perform the transition.</param>
        /// <param name="transitionType">How the transition should decide which state to transition to.</param>
        /// <param name="targetStates">Target states that can be transitioned to. If multiple are set, it will pick a random state.</param>
        public TimedTransition(int time, TransitionType transitionType = TransitionType.Random, params string[] targetStates)
            : base(transitionType)
        {
            RegisterTargetStates(targetStates);
            _timeDefault = time;
        }

        public override void Start(Character host)
        {
            base.Start(host);
            var state = host.ResolveResource<TimedTransitionInfo>(this);
            state.TimeLeft = _timeDefault;
        }

        /// <inheritdoc/>
        public override string Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<TimedTransitionInfo>(this);
            state.TimeLeft -= time.ElapsedMsDelta;

            if (state.TimeLeft <= 0)
            {
                state.TimeLeft = _timeDefault;
                return GetTargetState();
            }

            return null;
        }
    }
}