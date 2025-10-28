#region

using Common;
using Common.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public partial class BehaviorLib
    {
        [CharacterBehavior("Skull Shrine")]
        public static State SkullShrine =>
            new(
                new State("Spawn",
                    new HpLessTransition(0.95F, "p1")

                    //p1 - p1 : makes the circle around boss
                ),
                new State("p1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 4000),
                    new Spawn("SkullShrineAnchor", 0, 0, 10000000, minSpawnCount: 1, maxSpawnCount: 1),
                    new TimedTransition(4000, "p2")

                    //p2 - p2-1 : spawns Skulls that orbit the boss 
                ),
                new State("p2",
                    new Shoot(path: new DeceleratePath(4), count: 5, shootAngle: 10, projName: "Purple Magic", lifetimeMs: 2000, damage: 100, cooldownMS: 1000, maxRadius: 30, targeted: true, size: 100, multiHit: true),
                    new TossObject("SkullINNER", angle: 90, cooldownMS: 100000, minRange: 1, maxRange: 1),
                    new TossObject("SkullINNER", angle: 270, cooldownMS: 100000, minRange: 1, maxRange: 1),
                    new TossObject("SkullINNER", angle: 360, cooldownMS: 100000, minRange: 1, maxRange: 1),
                    new TossObject("SkullINNER", angle: 180, cooldownMS: 100000, minRange: 1, maxRange: 1),
                    new TimedTransition(2000, "p2-1")

                    //Boss main Attacks, transition at 50% hp, 
                ),
                new State("p2-1",
                    new Shoot(path: new DeceleratePath(4), count: 5, shootAngle: 10, projName: "Purple Magic", lifetimeMs: 2000, damage: 100, cooldownMS: 1000, maxRadius: 30, targeted: true, size: 100, multiHit: true),
                    new Shoot(path: new LinePath(6), count: 4, shootAngle: 90, fixedAngle: 0, projName: "Red Fire", lifetimeMs: 3000, damage: 100, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(6), count: 4, shootAngle: 90, fixedAngle: 10, projName: "Red Fire", lifetimeMs: 3000, damage: 100, coolDownOffset: 200, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(6), count: 4, shootAngle: 90, fixedAngle: 20, projName: "Red Fire", lifetimeMs: 3000, damage: 100, coolDownOffset: 400, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(6), count: 4, shootAngle: 90, fixedAngle: 30, projName: "Red Fire", lifetimeMs: 3000, damage: 100, coolDownOffset: 600, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(6), count: 4, shootAngle: 90, fixedAngle: -10, projName: "Red Fire", lifetimeMs: 3000, damage: 100, coolDownOffset: 200, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(6), count: 4, shootAngle: 90, fixedAngle: -20, projName: "Red Fire", lifetimeMs: 3000, damage: 100, coolDownOffset: 400, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(6), count: 4, shootAngle: 90, fixedAngle: -30, projName: "Red Fire", lifetimeMs: 3000, damage: 100, coolDownOffset: 600, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new HpLessTransition(0.50F, "p3")

                    //Summons groups of red skulls from different directions, spawns stationary skulls 
                ),
                new State("p3",
                    new TossObject("SkullINNER2", angle: 45, cooldownMS: 100000, minRange: 6, maxRange: 6),
                    new TossObject("SkullINNER2", angle: 135, cooldownMS: 100000, minRange: 6, maxRange: 6),
                    new TossObject("SkullINNER2", angle: 225, cooldownMS: 100000, minRange: 6, maxRange: 6),
                    new TossObject("SkullINNER2", angle: 315, cooldownMS: 100000, minRange: 6, maxRange: 6),
                    new TossObject("SkullTOP", cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 225),
                    new TossObject("SkullTOP", cooldownMS: 10000, minRange: 10.5F, maxRange: 10.5F, angle: 225),
                    new TossObject("SkullTOP", cooldownMS: 10000, minRange: 11, maxRange: 11, angle: 220),
                    new TossObject("SkullTOP", cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 230),
                    new TossObject("SkullTOP", cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 220),
                    new TossObject("SkullTOP", cooldownMS: 10000, minRange: 10.5F, maxRange: 10.5F, angle: 230),
                    new TossObject("SkullTOP", cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 235),
                    new TossObject("SkullBOT", cooldownOffsetMS: 4000, cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 45),
                    new TossObject("SkullBOT", cooldownOffsetMS: 4000, cooldownMS: 10000, minRange: 10.5F, maxRange: 10.5F, angle: 45),
                    new TossObject("SkullBOT", cooldownOffsetMS: 4000, cooldownMS: 10000, minRange: 11, maxRange: 11, angle: 40),
                    new TossObject("SkullBOT", cooldownOffsetMS: 4000, cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 50),
                    new TossObject("SkullBOT", cooldownOffsetMS: 4000, cooldownMS: 10000, minRange: 10.5F, maxRange: 10.5F, angle: 35),
                    new TossObject("SkullBOT", cooldownOffsetMS: 4000, cooldownMS: 10000, minRange: 11, maxRange: 11, angle: 30),
                    new TossObject("SkullBOT", cooldownOffsetMS: 4000, cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 40),
                    new TossObject("SkullLEFT", cooldownOffsetMS: 7000, cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 180),
                    new TossObject("SkullLEFT", cooldownOffsetMS: 7000, cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 170),
                    new TossObject("SkullLEFT", cooldownOffsetMS: 7000, cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 175),
                    new TossObject("SkullLEFT", cooldownOffsetMS: 7000, cooldownMS: 10000, minRange: 10.5F, maxRange: 10.5F, angle: 185),
                    new TossObject("SkullLEFT", cooldownOffsetMS: 7000, cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 183),
                    new TossObject("SkullLEFT", cooldownOffsetMS: 7000, cooldownMS: 10000, minRange: 10, maxRange: 10, angle: 178),
                    new TossObject("SkullLEFT", cooldownOffsetMS: 7000, cooldownMS: 10000, minRange: 10.5F, maxRange: 10.5F, angle: 176),
                    new TossObject("SkullRIGHT", cooldownOffsetMS: 5000, cooldownMS: 12000, minRange: 10, maxRange: 10, angle: 3),
                    new TossObject("SkullRIGHT", cooldownOffsetMS: 5000, cooldownMS: 12000, minRange: 10.5F, maxRange: 10.5F, angle: 359),
                    new TossObject("SkullRIGHT", cooldownOffsetMS: 5000, cooldownMS: 12000, minRange: 10, maxRange: 10, angle: 355),
                    new TossObject("SkullRIGHT", cooldownOffsetMS: 5000, cooldownMS: 12000, minRange: 10.5F, maxRange: 10.5F, angle: 5),
                    new TossObject("SkullRIGHT", cooldownOffsetMS: 5000, cooldownMS: 12000, minRange: 11, maxRange: 11, angle: 8),
                    new Shoot(path: new LinePath(7), count: 6, shootAngle: 60, rotateAngle: 15, targeted: false, projName: "Purple Magic", maxRadius: 20, lifetimeMs: 4000, cooldownMS: 3000, damage: 100, size: 100),
                    new Shoot(path: new LinePath(7), count: 6, shootAngle: 50, rotateAngle: 20, targeted: false, projName: "Purple Magic", maxRadius: 20, lifetimeMs: 4000, coolDownOffset: 200, cooldownMS: 3000, damage: 100, size: 100),
                    new Shoot(path: new LinePath(7), count: 6, shootAngle: 70, rotateAngle: 25, targeted: false, projName: "Purple Magic", maxRadius: 20, lifetimeMs: 4000, coolDownOffset: 200, cooldownMS: 3000, damage: 100, size: 100)
                )
            );


        [CharacterBehavior("SkullINNER")]
        public static State SkullINNER =>
            new(
                new State("orbit",
                    new Orbit(2, 1, 10, "Skull Shrine", 0, 0),
                    new Shoot(path: new WavyPath(5), count: 3, shootAngle: 8, projName: "Blue Magic", lifetimeMs: 2000, damage: 100, cooldownMS: 2000, maxRadius: 10, targeted: true, size: 100)
                )
            );


        [CharacterBehavior("SkullTOP")]
        public static State SkullTOP =>
            new(
                new State("CHARGEEE",
                    new MoveTo(1, 1, 6, true),
                    new Shoot(path: new AcceleratePath(7), count: 2, shootAngle: 5, cooldownMS: 2000, projName: "Red Fire", lifetimeMs: 2000, damage: 100, maxRadius: 40, targeted: true, size: 100),
                    new Suicide(4000)
                )
            );

        [CharacterBehavior("SkullBOT")]
        public static State SkullBOT =>
            new(
                new State("CHARGEEE",
                    new MoveTo(-1, -1, 6, true),
                    new Shoot(path: new AcceleratePath(7), count: 2, shootAngle: 5, cooldownMS: 2000, projName: "Red Fire", lifetimeMs: 2000, damage: 100, maxRadius: 40, targeted: true, size: 100),
                    new Suicide(4000)
                )
            );

        [CharacterBehavior("SkullLEFT")]
        public static State SkullLEFT =>
            new(
                new State("CHARGEEE",
                    new MoveTo(1, 0, 6, true),
                    new Shoot(path: new AcceleratePath(7), count: 2, shootAngle: 5, cooldownMS: 2000, projName: "Red Fire", lifetimeMs: 2000, damage: 100, maxRadius: 40, targeted: true, size: 100),
                    new Suicide(4000)
                )
            );

        [CharacterBehavior("SkullRIGHT")]
        public static State SkullRIGHT =>
            new(
                new State("CHARGEEE",
                    new MoveTo(-1, 0, 6, true),
                    new Shoot(path: new AcceleratePath(7), count: 2, shootAngle: 5, cooldownMS: 2000, projName: "Red Fire", lifetimeMs: 2000, damage: 100, maxRadius: 40, targeted: true, size: 100),
                    new Suicide(4000)
                )
            );

        [CharacterBehavior("SkullINNER2")]
        public static State SkullINNER2 =>
            new(
                new State("SHOOOT",
                    new Shoot(path: new LinePath(6), count: 2, shootAngle: 25, cooldownMS: 2500, projName: "Blue Magic", lifetimeMs: 2000, damage: 100, maxRadius: 20, targeted: true, size: 100),
                    new Shoot(path: new LinePath(6), count: 2, shootAngle: 15, cooldownMS: 2500, coolDownOffset: 200, projName: "Blue Magic", lifetimeMs: 2000, damage: 100, maxRadius: 20, targeted: true, size: 100),
                    new Shoot(path: new LinePath(6), count: 1, cooldownMS: 2500, coolDownOffset: 300, projName: "Blue Magic", lifetimeMs: 2000, damage: 100, maxRadius: 20, targeted: true, size: 100),
                    new Shoot(path: new CirclePath(0.4F, 1), count: 1, projName: "White Bullet", lifetimeMs: 3000, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 270, size: 30, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(0.4F, 1), count: 1, projName: "White Bullet", lifetimeMs: 3000, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 270, size: 30, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(0.4F, 1), count: 1, projName: "White Bullet", lifetimeMs: 3000, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 270, size: 30, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(0.4F, 1), count: 1, projName: "White Bullet", lifetimeMs: 3000, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 270, size: 30, armorPiercing: true, multiHit: true),
                    new EntitiesNotWithinTransition(30, "GG", "Skull Shrine")
                ),
                new State("GG",
                    new Suicide(500)
                )
            );


        [CharacterBehavior("SkullShrineAnchor")]
        public static State SkullShrineAnchor =>
            new(
                new State("SHOOOT",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Shoot(path: new LinePath(3.7F), count: 1, projName: "White Bullet", lifetimeMs: 2490, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 0, size: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new LinePath(3.7F), count: 1, projName: "White Bullet", lifetimeMs: 2490, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 90, size: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new LinePath(3.7F), count: 1, projName: "White Bullet", lifetimeMs: 2490, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 180, size: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new LinePath(3.7F), count: 1, projName: "White Bullet", lifetimeMs: 2490, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 270, size: 50, armorPiercing: true, multiHit: true),
                    new TimedTransition(2500, "Shoot")
                ),
                new State("Shoot",
                    new Shoot(path: new CirclePath(0.1F, 9), count: 1, projName: "White Bullet", lifetimeMs: 2525, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 0, size: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(0.1F, 9), count: 1, projName: "White Bullet", lifetimeMs: 2525, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 90, size: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(0.1F, 9), count: 1, projName: "White Bullet", lifetimeMs: 2525, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 180, size: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(0.1F, 9), count: 1, projName: "White Bullet", lifetimeMs: 2525, damage: 100, cooldownMS: 500, maxRadius: 40, targeted: false, fixedAngle: 270, size: 50, armorPiercing: true, multiHit: true),
                    new EntityNotWithinTransition("GG", "Skull Shrine", 20)
                ),
                new State("GG",
                    new Suicide(500)
                )
            );
    }
}