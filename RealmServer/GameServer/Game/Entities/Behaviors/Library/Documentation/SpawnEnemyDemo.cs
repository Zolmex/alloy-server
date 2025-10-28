#region

using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public class SpawnEnemyDemo : EntityBehavior
    {
        private Spawn Spawn;

        public override void RegisterStates()
        {
            StateManager.RegisterState(DemoState.Tick, TestTick);
        }

        public override void RegisterBehaviors()
        {
            Spawn = new Spawn("Move Demo", -3, 3, -3, maxY: 3, maxSpawnsPerReset: 20, minSpawnCount: 1, maxSpawnCount: 3);
        }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, DemoState.Tick);
            base.Initialize(owner);
        }

        public void TestTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
            {
                Spawn.Start(owner);
            }
            else if (state == StateTick.Tick)
            {
                Spawn.Tick(owner, time);
            }
        }

        public enum DemoState
        {
            Tick
        }
    }
}