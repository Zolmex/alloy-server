#region

using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public class MoveDemo : EntityBehavior
    {
        private Move Move;

        public override void RegisterStates()
        {
            StateManager.RegisterState(DemoState.Tick, TestTick);
        }

        public override void RegisterBehaviors()
        {
            Move = new Move(4, 2, 1000, 4f);
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
                Move.Start(owner);
            }
            else if (state == StateTick.Tick)
            {
                var tickState = Move.Tick(owner, time);
                if (tickState == BehaviorScript.BehaviorTickState.BehaviorActivate)
                {
                    owner.World.Taunt(owner, "Started Moving!");
                }
                else if (tickState == BehaviorScript.BehaviorTickState.BehaviorDeactivate)
                {
                    owner.World.Taunt(owner, "Stopped Moving!");
                }
            }
        }

        public enum DemoState
        {
            Tick
        }
    }
}