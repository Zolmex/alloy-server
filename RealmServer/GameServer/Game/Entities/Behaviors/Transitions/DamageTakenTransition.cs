namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class DamageTakenRecord
    {
        public int DamageTaken;

        public void OnEntityDamaged(Character en, Character from, int dmgTaken)
        {
            DamageTaken += dmgTaken;
        }
    }

    public class DamageTakenTransition : BehaviorTransition
    {
        private readonly int _damage;

        public DamageTakenTransition(int damage, string targetState)
            : base(TransitionType.Random)
        {
            RegisterTargetStates(targetState);
            _damage = damage;
        }

        public override void Start(Character host)
        {
            var dmgTakenInfo = host.ResolveResource<DamageTakenRecord>(this);
            host.OnDamagedBy += dmgTakenInfo.OnEntityDamaged;
        }

        public override string Tick(Character host, RealmTime time)
        {
            var dmgTakenInfo = host.ResolveResource<DamageTakenRecord>(this);
            if (dmgTakenInfo.DamageTaken >= _damage)
            {
                return GetTargetState();
            }

            return null;
        }

        public override void End(Character host, RealmTime time)
        {
            var dmgTakenInfo = host.ResolveResource<DamageTakenRecord>(this);
            host.OnDamagedBy -= dmgTakenInfo.OnEntityDamaged;
        }
    }
}