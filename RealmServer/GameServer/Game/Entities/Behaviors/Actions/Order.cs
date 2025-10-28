namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record Order : BehaviorScript
    {
        private readonly float _range;
        private readonly string _children;
        private readonly string _targetState;

        public Order(float range, string children, string targetState)
        {
            _range = range;
            _children = children;
            _targetState = targetState;
        }

        public override void Start(Character host)
        {
            foreach (var i in host.GetOtherEnemiesByName(_children, _range))
            {
                if (i.Behavior != null)
                { // Patpot's behavior system
                    // Need to find a way to pass the target state enum into this behavior
                }
                else
                { // Traditional behaviors
                    i.ClassicBehavior?.TransitionTo(_targetState, RealmManager.WorldTime);
                }
            }
        }
    }
}