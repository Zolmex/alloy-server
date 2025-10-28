#region

using Common;
using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public class AOEDemo : EntityBehavior
    {
        private AOE AOE;
        private AOE AOE2;
        private AOE AOE3;

        public override void RegisterStates()
        {
            StateManager.RegisterState(DemoState.Tick, TestTick);
        }

        public override void RegisterBehaviors()
        {
            AOE = new AOE(3f, 10, 10000, color: 0x000000, throwTime: 1000, damageColor: 0x000000);
            AOE2 = new AOE(2f, 10, 10000, cooldownOffset: 1500, color: 0xFF0000, throwTime: 2000, activateCount: 3, damageColor: 0xFF0000, damageCooldown: 200);
            AOE3 = new AOE(6f, 10, 10000, cooldownOffset: 4500, color: 0x0000FF, throwTime: 500, activateCount: 3, damageColor: 0x0000FF, damageCooldown: 1000);
        }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, DemoState.Tick);
            base.Initialize(owner);
        }

        public void TestTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
            { }
            else if (state == StateTick.Tick)
            {
                AOE.Tick(owner, time);
                AOE2.Tick(owner, time);
                AOE3.Tick(owner, time);
            }
        }

        public void AOEManualTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
            {
                owner.AOEDamage(owner.Position.ToVec2(), 50);
            }
        }

        public enum DemoState
        {
            Tick
        }
    }
}