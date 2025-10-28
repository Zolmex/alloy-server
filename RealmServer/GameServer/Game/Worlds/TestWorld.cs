namespace GameServer.Game.Worlds
{
    public class TestWorld : World
    {
        public TestWorld(string name) : base(name, 0) { }

        public override void Initialize()
        {
            InitializeEntities();

            _startTime = RealmManager.WorldTime.TotalElapsedMs;
            _lastActiveTime = RealmManager.WorldTime.TotalElapsedMs;
            Initialized = true;
        }
    }
}