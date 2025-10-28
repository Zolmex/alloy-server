namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record RemoveObjectOnDeath : BehaviorScript
    {
        private readonly string _objName;
        private readonly int _range;

        public RemoveObjectOnDeath(string objName, int range)
        {
            _objName = objName;
            _range = range;
        }

        public override void Start(Character host)
        {
            host.DeathEvent += OnDeath;
        }

        public void OnDeath(Entity host)
        {
            foreach (var match in host.World.GetEntitiesWithin(host.Position.X, host.Position.Y, _range,
                         en => en.Desc.ObjectId == _objName))
            {
                match.TryLeaveWorld();
            }
        }
    }
}