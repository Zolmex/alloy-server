#region

using Common.ProjectilePaths;
using Common.Utilities;
using System;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public class PannyDemo : EntityBehavior
    {
        private int _cooldown;
        private const int ShockwaveCooldown = 1000;
        private const int ShockwaveProjectileGap = 10;
        private const int ShockwaveProjectileSafeGap = 20;
        private const int ShockwaveGapMinimum = 4;
        private const int ShockwaveGapMaximum = 8;

        public override void RegisterStates()
        {
            StateManager.RegisterState(DemoState.Tick, TestTick);
        }

        public override void RegisterBehaviors()
        { }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, DemoState.Tick);
            base.Initialize(owner);
        }

        public void TestTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Tick)
            {
                _cooldown -= time.ElapsedMsDelta;
                if (_cooldown <= 0)
                {
                    _shockwave(owner);
                    _cooldown = ShockwaveCooldown;
                    return;
                }
            }
        }

        private void _shockwave(Character owner)
        {
            var rand = new Random();
            var numberOfGaps = rand.Next(ShockwaveGapMinimum, ShockwaveGapMaximum);
            float randomStartAngle = rand.Next(0, 360);
            float numProjectiles = ((360 - (ShockwaveProjectileSafeGap * numberOfGaps)) / ShockwaveProjectileGap) + numberOfGaps;
            var gapEvery = (int)Math.Floor(numProjectiles / numberOfGaps);
            var j = 0;
            var shootAngleDeg = randomStartAngle;
            for (var i = 0; i < numProjectiles; i++)
            {
                var gapAngle = ++j % gapEvery == 0;
                if (gapAngle)
                {
                    shootAngleDeg += (ShockwaveProjectileSafeGap / 2) + (ShockwaveProjectileGap / 2);
                    owner.ShootProjectiles(new ChangeSpeedPath(-15, 16, 2000, 5000, 1).ToPath(), projName: "Blue Star", damage: 5, angle: shootAngleDeg.Deg2Rad());
                    shootAngleDeg += (ShockwaveProjectileSafeGap / 2) - (ShockwaveProjectileGap / 2);
                }
                else
                {
                    shootAngleDeg += ShockwaveProjectileGap;
                    owner.ShootProjectiles(new ChangeSpeedPath(-15, 16, 2000, 5000, 1).ToPath(), projName: "Red Star", damage: 5, angle: shootAngleDeg.Deg2Rad());
                }
            }
        }


        public enum DemoState
        {
            Tick
        }
    }
}