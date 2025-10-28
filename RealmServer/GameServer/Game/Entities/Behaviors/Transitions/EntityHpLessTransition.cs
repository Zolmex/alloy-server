namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class EntityHpLessTransition : BehaviorTransition
    {
        private readonly float _threshold;
        private readonly float _dist;
        private readonly string _entity;

        public EntityHpLessTransition(float dist, string entity, float threshold, string targetState)
            : base()
        {
            RegisterTargetStates(targetState);
            _threshold = threshold;
            _dist = dist;
            _entity = entity;
        }

        public override string Tick(Character host, RealmTime time)
        {
            var entity = host.World.GetNearestEnemyByName(_entity, host.Position.X, host.Position.Y, _dist);
            if (entity == null)
                return null;

            var hpPerc = (float)entity.HP / entity.MaxHP;
            var transition = hpPerc <= _threshold;
            return transition ? GetTargetState() : null;
        }
    }
}