namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record Suicide : BehaviorScript
    {
        private readonly int _delay;

        public Suicide(int delay = 300)
        {
            _delay = delay;
        }

        public override void Start(Character host)
        {
            host.World.AddTimedAction(_delay, () => host.TryLeaveWorld());
        }
    }
}