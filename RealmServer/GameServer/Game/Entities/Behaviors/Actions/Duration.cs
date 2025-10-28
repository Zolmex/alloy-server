namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class DurationInfo
    {
        public int TimeLeft;
    }

    public record Duration : BehaviorScript
    {
        private readonly BehaviorScript _behavior;
        private readonly int _duration;

        public Duration(BehaviorScript behavior, int duration)
        {
            _behavior = behavior;
            _duration = duration;
        }

        public override void Start(Character host)
        {
            var state = host.ResolveResource<DurationInfo>(this);
            state.TimeLeft = _duration;
            _behavior.Start(host);
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<DurationInfo>(this);

            if (state.TimeLeft <= 0)
            {
                return BehaviorTickState.BehaviorFailed;
            }

            _behavior.Tick(host, time);
            state.TimeLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.BehaviorActive;
        }
    }
}