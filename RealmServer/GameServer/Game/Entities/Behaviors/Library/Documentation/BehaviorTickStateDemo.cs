#region

using Common.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;
using static GameServer.Game.Entities.Behaviors.BehaviorScript;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library.Documentation
{
    public class BehaviorTickStateDemo : EntityBehavior
    {
        private Shoot Shoot;

        // Called once upon behavior loading
        public override void RegisterStates()
        {
            StateManager.RegisterState(DemoState.Tick, Tick);
        }

        // Called once upon behavior loading
        public override void RegisterBehaviors()
        {
            Shoot = new Shoot(12, damage: 10, path: new LinePath(5), count: 1, lifetimeMs: 5000, cooldownMS: 1000, targeted: false, projName: "Green Star", onHitEvent: _hitByGreenStar);
        }

        // Called every time a behavior is added to an entity
        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, DemoState.Tick);
            base.Initialize(owner);
        }

        public void Tick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Tick)
            {
                var shootState = Shoot.Tick(owner, time);
                if (shootState == BehaviorTickState.BehaviorActive)
                    owner.World.Taunt(owner, "I just shot :)");
                else if (shootState == BehaviorTickState.OnCooldown)
                    owner.World.Taunt(owner, "Peter I can't shoot");
            }
        }

        private void _hitByGreenStar(Character hit, Character hitBy)
        {
            hitBy.World.Taunt(hitBy, $"I just hit {hit.Name}!");
        }

        public enum DemoState
        {
            Tick
        }
    }
}