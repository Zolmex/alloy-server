#region

using Common.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;
using static GameServer.Game.Entities.Behaviors.BehaviorScript;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class MultiProjectilePathDemo : EntityBehavior
    {
        private Shoot LineIntoCircle;
        private ProjectilePath _path;

        public override void RegisterBehaviors()
        {
            // _path = new ProjectilePath(500,
            //         new LinePath(5f, 0f))
            //     .Then(500, new LinePath(5f, 90))
            //     .Then(500, new LinePath(5f, 180))
            //     .Then(1000, new CirclePath(1, 1f));
            //.Then(500, new WavyPath(10));
            _path = new CombinedPath(
                    null,
                    new WavyPath(6, lifetimeMs: 5000, mods: PathSegmentModifier.Boomerang),
                    new DeceleratePath(1, lifetimeMs: 5000))
                .ToPath();

            LineIntoCircle = new Shoot(12, new ShootConfig()
            {
                CooldownMs = 50,
                Count = 1,
                TargetType = TargetType.ClosestPlayer,
                FixedAngle = 0,
                ProjName = "Red Star",
                Damage = 25
            }, _path);
            base.RegisterBehaviors();
        }

        public override void RegisterStates()
        {
            StateManager.RegisterState(MultiProjectilePathState.Shoot, ShootTick);
            base.RegisterStates();
        }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, MultiProjectilePathState.Shoot);
            base.Initialize(owner);
        }

        public void ShootTick(RealmTime time, Character owner, StateTick state)
        {
            LineIntoCircle.Tick(owner, time);
        }

        public enum MultiProjectilePathState
        {
            Shoot
        }
    }
}