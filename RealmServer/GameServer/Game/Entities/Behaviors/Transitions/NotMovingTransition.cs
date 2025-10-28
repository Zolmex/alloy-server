#region

using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class NotMovingTransitionInfo
    {
        public Vector2 Position;
        public int TimeLeft;
    }

    public class NotMovingTransition : BehaviorTransition
    {
        private readonly int _delay;

        public NotMovingTransition(string targetState, int delay = 250)
            : base(TransitionType.Random)
        {
            RegisterTargetStates(targetState);
            _delay = delay;
        }

        public override void Start(Character host)
        {
            var state = host.ResolveResource<NotMovingTransitionInfo>(this);
            state.Position = new Vector2(host.Position.X, host.Position.Y);
            state.TimeLeft = _delay;
        }

        public override string Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<NotMovingTransitionInfo>(this);
            if (state.TimeLeft > 0)
            {
                state.TimeLeft -= time.ElapsedMsDelta;
                return null;
            }

            if (host.Position.X == state.Position.X && host.Position.Y == state.Position.Y)
            {
                return GetTargetState();
            }

            // Re-assign the position and reset the delay
            state.Position = new Vector2(host.Position.X, host.Position.Y);
            state.TimeLeft = _delay;
            return null;
        }
    }
}