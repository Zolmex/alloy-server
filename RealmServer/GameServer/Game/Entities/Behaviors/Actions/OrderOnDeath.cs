namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record OrderOnDeath : BehaviorScript
    {
        private readonly float _range;
        private readonly string _children;
        private readonly string _targetState;

        public OrderOnDeath(float range, string children, string targetState)
        {
            _range = range;
            _children = children;
            _targetState = targetState;
        }

        public override void Start(Character host)
        {
            host.DeathEvent += OnDeath;
        }

        private void OnDeath(Entity en)
        {
            foreach (var i in (en as Character).GetOtherEnemiesByName(_children, _range))
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