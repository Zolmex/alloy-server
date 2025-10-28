#region

using Common.ProjectilePaths;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Utilities;
using static GameServer.Game.Entities.Behaviors.BehaviorScript;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public class DashDemo : EntityBehavior
    {
        public override void RegisterStates()
        {
            StateManager.RegisterState(DemoState.Tick, TestTick);
        }

        public override void RegisterBehaviors()
        {
            Dash = new Dash(dashSpeed: 8, dashRange: 4, numDashes: 3, cooldownMs: 200, cycleCooldownMs: 2000, ease: Ease.InCubic, damage: 10);
            DashShoot = new Shoot(32f, new LinePath(15f), 3, 15, projName: "Red Star", targetType: TargetType.ClosestPlayer, cooldownMS: 100, lifetimeMs: 500, damage: 5);
        }

        public Dash Dash { get; set; }
        public Shoot DashShoot { get; set; }

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
                var tickState = Dash.Tick(owner, time);
                if (tickState == BehaviorTickState.BehaviorActivate)
                {
                    var dashInfo = owner.ResolveResource<DashInfo>(Dash);
                    owner.World.Taunt(owner, $"I'm out here dashing at {dashInfo.DashAngle.Rad2Deg()} degrees man I got tiles under my feet im a freak man");
                    owner.ShootProjectiles(new LinePath(10, 1000).ToPath(), projName: "Green Star", damage: 10, angle: owner.GetAngleBetween(owner.GetNearestPlayer(252)));
                }
                else if (tickState == BehaviorTickState.BehaviorActive)
                {
                    DashShoot.Tick(owner, time);
                }
            }
        }

        public enum DemoState
        {
            Tick
        }
    }
}