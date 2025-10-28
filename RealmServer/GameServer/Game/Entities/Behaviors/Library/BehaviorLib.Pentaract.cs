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
        [CharacterBehavior("Pentaract")]
        public static State Pentaract =>
            new(
                new State("start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new TossObject("Pentaract Tower1", minRange: 15, maxRange: 15, angle: 270, cooldownMS: 100000),
                    new TossObject("Pentaract Tower2", minRange: 15, maxRange: 15, angle: 198, cooldownMS: 100000),
                    new TossObject("Pentaract Tower3", minRange: 15, maxRange: 15, angle: 342, cooldownMS: 100000),
                    new TossObject("Pentaract Tower4", minRange: 15, maxRange: 15, angle: 126, cooldownMS: 100000),
                    new TossObject("Pentaract Tower5", minRange: 15, maxRange: 15, angle: 54, cooldownMS: 100000),
                    new TimedTransition(2000, "WAIT")
                ),
                new State("WAIT",
                    new EntityNotWithinTransition("Ultra", "PentaractTransition1", 30)
                ),
                new State("Ultra",
                    new TossObject("Pentaract Tower Ultra", cooldownMS: 10000, minRange: 0.2F, maxRange: 0.2F, angle: 270),
                    new TimedTransition(500, "Shoot")
                ),
                new State("Shoot",
                    new Shoot(path: new CirclePath(0.1F, 15), maxRadius: 50, count: 1, projName: "White Bullet", cooldownMS: 100, lifetimeMs: 2500, targeted: false, damage: 100, fixedAngle: 360, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(-0.1F, 15), maxRadius: 50, count: 1, projName: "White Bullet", cooldownMS: 100, lifetimeMs: 2500, targeted: false, damage: 100, fixedAngle: 360, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(0.1F, 15), maxRadius: 50, count: 1, projName: "White Bullet", cooldownMS: 100, lifetimeMs: 2500, targeted: false, damage: 100, fixedAngle: 180, armorPiercing: true, multiHit: true),
                    new Shoot(path: new CirclePath(-0.1F, 15), maxRadius: 50, count: 1, projName: "White Bullet", cooldownMS: 100, lifetimeMs: 2500, targeted: false, damage: 100, fixedAngle: 180, armorPiercing: true, multiHit: true),
                    new EntityNotWithinTransition("GG", "Pentaract Tower Ultra", 30)
                ),
                new State("GG",
                    new EntitiesNotWithinTransition(20, "GGG", ["Pentaract Tower1", "Pentaract Tower2", "Pentaract Tower3", "Pentaract Tower4", "Pentaract Tower5"])
                ),
                new State("GGG",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("Pentaract Tower1")]
        public static State PentaractTower1 =>
            new(
                new State("Start",
                    new TossObject("PentaractTransition1", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new TimedTransition(100, "Shoot1")
                ),
                new State("Shoot1",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 0, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Spawn("Pentaract Eye1", cooldownMs: 3000, minSpawnCount: 2, maxSpawnCount: 2, maxDensity: 10, densityRadius: 10),
                    new HpLessTransition(0.50F, "Transition")
                ),
                new State("Transition",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TossObject("PentaractTransition2", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new EntityNotWithinTransition("WAIT", "PentaractTransition1", 40)
                ),
                new State("WAIT",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TimedTransition(300, "Shoot2")
                ),
                new State("Shoot2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 72, projName: "Red Fire", damage: 50, targeted: false, lifetimeMs: 3130, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 35.5F, projName: "White Bullet", damage: 50, targeted: false, lifetimeMs: 1900, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new EntityNotWithinTransition("Wait2", "Pentaract Tower Ultra", 30)
                ),
                new State("Wait2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new Flash(0xF73626, 0.5F, 4),
                    new TimedTransition(2000, "Shoot3")
                ),
                new State("Shoot3",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: 20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot4")
                ),
                new State("Shoot4",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: -20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot3")
                )
            );

        [CharacterBehavior("Pentaract Tower2")]
        public static State PentaractTower2 =>
            new(
                new State("Start",
                    new TossObject("PentaractTransition1", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new TimedTransition(100, "Shoot1")
                ),
                new State("Shoot1",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 0, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Spawn("Pentaract Eye2", cooldownMs: 3000, minSpawnCount: 2, maxSpawnCount: 2, maxDensity: 10, densityRadius: 10),
                    new HpLessTransition(0.50F, "Transition")
                ),
                new State("Transition",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TossObject("PentaractTransition2", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new EntityNotWithinTransition("WAIT", "PentaractTransition1", 40)
                ),
                new State("WAIT",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TimedTransition(300, "Shoot2")
                ),
                new State("Shoot2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 360, projName: "Red Fire", damage: 50, targeted: false, lifetimeMs: 3100, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 323.8F, projName: "White Bullet", damage: 50, targeted: false, lifetimeMs: 1920, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new EntityNotWithinTransition("Wait2", "Pentaract Tower Ultra", 30)
                ),
                new State("Wait2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new Flash(0xF73626, 0.5F, 4),
                    new TimedTransition(2000, "Shoot3")
                ),
                new State("Shoot3",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: 20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot4")
                ),
                new State("Shoot4",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: -20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot3")
                )
            );

        [CharacterBehavior("Pentaract Tower3")]
        public static State PentaractTower3 =>
            new(
                new State("Start",
                    new TossObject("PentaractTransition1", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new TimedTransition(100, "Shoot1")
                ),
                new State("Shoot1",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 0, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Spawn("Pentaract Eye3", cooldownMs: 3000, minSpawnCount: 2, maxSpawnCount: 2, maxDensity: 10, densityRadius: 10),
                    new HpLessTransition(0.50F, "Transition")
                ),
                new State("Transition",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TossObject("PentaractTransition2", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new EntityNotWithinTransition("WAIT", "PentaractTransition1", 40)
                ),
                new State("WAIT",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TimedTransition(300, "Shoot2")
                ),
                new State("Shoot2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 144, projName: "Red Fire", damage: 50, targeted: false, lifetimeMs: 3100, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 108, projName: "White Bullet", damage: 50, targeted: false, lifetimeMs: 1920, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new EntityNotWithinTransition("Wait2", "Pentaract Tower Ultra", 30)
                ),
                new State("Wait2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new Flash(0xF73626, 0.5F, 4),
                    new TimedTransition(2000, "Shoot3")
                ),
                new State("Shoot3",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: 20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot4")
                ),
                new State("Shoot4",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: -20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot3")
                )
            );

        [CharacterBehavior("Pentaract Tower4")]
        public static State PentaractTower4 =>
            new(
                new State("Start",
                    new TossObject("PentaractTransition1", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new TimedTransition(100, "Shoot1")
                ),
                new State("Shoot1",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 0, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Spawn("Pentaract Eye4", cooldownMs: 3000, minSpawnCount: 2, maxSpawnCount: 2, maxDensity: 10, densityRadius: 10),
                    new HpLessTransition(0.50F, "Transition")
                ),
                new State("Transition",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TossObject("PentaractTransition2", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new EntityNotWithinTransition("WAIT", "PentaractTransition1", 40)
                ),
                new State("WAIT",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TimedTransition(300, "Shoot2")
                ),
                new State("Shoot2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 288, projName: "Red Fire", damage: 50, targeted: false, lifetimeMs: 3130, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 252, projName: "White Bullet", damage: 50, targeted: false, lifetimeMs: 1920, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new EntityNotWithinTransition("Wait2", "Pentaract Tower Ultra", 30)
                ),
                new State("Wait2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new Flash(0xF73626, 0.5F, 4),
                    new TimedTransition(2000, "Shoot3")
                ),
                new State("Shoot3",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: 20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot4")
                ),
                new State("Shoot4",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: -20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot3")
                )
            );

        [CharacterBehavior("Pentaract Tower5")]
        public static State PentaractTower5 =>
            new(
                new State("Start",
                    new TossObject("PentaractTransition1", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new TimedTransition(100, "Shoot1")
                ),
                new State("Shoot1",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 0, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -10, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 200, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -20, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 400, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -30, rotateAngle: 15, projName: "Silver Shield", lifetimeMs: 2000, damage: 100, coolDownOffset: 600, cooldownMS: 1500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true),
                    new Spawn("Pentaract Eye5", cooldownMs: 3000, minSpawnCount: 2, maxSpawnCount: 2, maxDensity: 10, densityRadius: 10),
                    new HpLessTransition(0.50F, "Transition")
                ),
                new State("Transition",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TossObject("PentaractTransition2", cooldownMS: 1000000, minRange: 0.1F, maxRange: 0.1F, angle: 360),
                    new EntityNotWithinTransition("WAIT", "PentaractTransition1", 40)
                ),
                new State("WAIT",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TimedTransition(300, "Shoot2")
                ),
                new State("Shoot2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 216.1F, projName: "Red Fire", damage: 50, targeted: false, lifetimeMs: 3100, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new Shoot(path: new LinePath(9), count: 1, fixedAngle: 180, projName: "White Bullet", damage: 50, targeted: false, lifetimeMs: 1920, cooldownMS: 100, maxRadius: 50, armorPiercing: true, multiHit: true),
                    new EntityNotWithinTransition("Wait2", "Pentaract Tower Ultra", 30)
                ),
                new State("Wait2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new Flash(0xF73626, 0.5F, 4),
                    new TimedTransition(2000, "Shoot3")
                ),
                new State("Shoot3",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: 20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot4")
                ),
                new State("Shoot4",
                    new AOE(range: 10, radius: 4, damage: 50, cooldownMs: 3000),
                    new Shoot(path: new LinePath(4), count: 4, projName: "Silver Shield", shootAngle: 90, rotateAngle: -20, targeted: false, damage: 50, maxRadius: 15, lifetimeMs: 2000, cooldownMS: 200, multiHit: true),
                    new Shoot(path: new AmplitudePath(4, 0.3F, 1F), count: 4, projName: "Black Missile", shootAngle: 5, targeted: true, damage: 50, maxRadius: 15, lifetimeMs: 3000, cooldownMS: 1500, coolDownOffset: 500, multiHit: true, armorPiercing: true),
                    new TimedTransition(3000, "Shoot3")
                )
            );

        [CharacterBehavior("PentaractTransition1")]
        public static State PentaractTransition1 =>
            new(
                new State("Wait",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityWithinTransition("Die", "PentaractTransition2", 2)
                ),
                new State("Die",
                    new Suicide(200)
                )
            );

        [CharacterBehavior("PentaractTransition2")]
        public static State PentaractTransition2 =>
            new(
                new State("Die",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Suicide(500)
                )
            );

        [CharacterBehavior("Pentaract Tower Ultra")]
        public static State PentaractTowerUltra =>
            new(
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Flash(0xF73626, 0.5F, 4),
                    new TimedTransition(2000, "Shoot")
                ),
                new State("Shoot",
                    new Shoot(path: new AcceleratePath(5), count: 8, maxRadius: 20, projName: "Black Missile", damage: 50, shootAngle: 10, cooldownMS: 2000, lifetimeMs: 3000, targeted: true, multiHit: true, armorPiercing: true),
                    new Shoot(path: new DeceleratePath(4), count: 4, maxRadius: 20, projName: "Black Missile", damage: 50, shootAngle: 5, cooldownMS: 2000, coolDownOffset: 1000, lifetimeMs: 3000, targeted: true, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(5, 0.5F, 0.5F, lifetimeMs: 5), count: 10, maxRadius: 20, projName: "White Bolt", damage: 50, shootAngle: 36, cooldownMS: 3000, lifetimeMs: 3000, rotateAngle: 25, targeted: false, multiHit: true, size: 120),
                    new Spawn("Pentaract Eye Ultra", cooldownMs: 5000, minSpawnCount: 4, maxSpawnCount: 6, maxDensity: 20, densityRadius: 10)
                )
            );

        [CharacterBehavior("Pentaract Eye Ultra")]
        public static State PentaractEyeUltra =>
            new(
                new State("Orbit1",
                    new Orbit(3, 3, 10, "Pentaract Tower Ultra", 0, 0),
                    new Shoot(path: new LinePath(6), count: 1, maxRadius: 10, projName: "White Blast", cooldownMS: 4000, coolDownOffset: 1000, lifetimeMs: 2000, targeted: true, damage: 50, size: 50),
                    new TimedTransition(6000, "Orbit2"),
                    new EntityNotWithinTransition("GG", "Pentaract Tower Ultra", 30)
                ),
                new State("Orbit2",
                    new Orbit(5, 8, 10, "Pentaract Tower Ultra", 0, 0),
                    new Shoot(path: new LinePath(6), count: 1, maxRadius: 10, projName: "White Blast", cooldownMS: 4000, coolDownOffset: 1000, lifetimeMs: 2000, targeted: true, damage: 50, size: 50),
                    new TimedTransition(6000, "Orbit1"),
                    new EntityNotWithinTransition("GG", "Pentaract Tower Ultra", 30)
                ),
                new State("GG",
                    new Suicide(10)
                )
            );

        [CharacterBehavior("Pentaract Eye1")]
        public static State PentaractEye1 =>
            new(
                new State("Protect",
                    new Wander(2, 1, 4),
                    new Protect(8, "Pentaract Tower1", 20, 10),
                    new Follow(6, 0.5F, 10),
                    new Shoot(path: new DeceleratePath(7), count: 1, maxRadius: 10, projName: "White Bolt", damage: 50, cooldownMS: 3000, lifetimeMs: 2000, targeted: true),
                    new EntityNotWithinTransition("GG", "Pentaract Tower1", 5)
                ),
                new State("GG",
                    new Suicide(500)
                )
            );

        [CharacterBehavior("Pentaract Eye2")]
        public static State PentaractEye2 =>
            new(
                new State("Protect",
                    new Wander(2, 1, 4),
                    new Protect(8, "Pentaract Tower2", 20, 10),
                    new Follow(6, 0.5F, 10),
                    new Shoot(path: new DeceleratePath(7), count: 1, maxRadius: 10, projName: "White Bolt", damage: 50, cooldownMS: 3000, lifetimeMs: 2000, targeted: true),
                    new EntityNotWithinTransition("GG", "Pentaract Tower2", 5)
                ),
                new State("GG",
                    new Suicide(500)
                )
            );

        [CharacterBehavior("Pentaract Eye3")]
        public static State PentaractEye3 =>
            new(
                new State("Protect",
                    new Wander(2, 1, 4),
                    new Protect(8, "Pentaract Tower3", 20, 10),
                    new Follow(6, 0.5F, 10),
                    new Shoot(path: new DeceleratePath(7), count: 1, maxRadius: 10, projName: "White Bolt", damage: 50, cooldownMS: 3000, lifetimeMs: 2000, targeted: true),
                    new EntityNotWithinTransition("GG", "Pentaract Tower3", 5)
                ),
                new State("GG",
                    new Suicide(500)
                )
            );

        [CharacterBehavior("Pentaract Eye4")]
        public static State PentaractEye4 =>
            new(
                new State("Protect",
                    new Wander(2, 1, 4),
                    new Protect(8, "Pentaract Tower4", 20, 10),
                    new Follow(6, 0.5F, 10),
                    new Shoot(path: new DeceleratePath(7), count: 1, maxRadius: 10, projName: "White Bolt", damage: 50, cooldownMS: 3000, lifetimeMs: 2000, targeted: true),
                    new EntityNotWithinTransition("GG", "Pentaract Tower4", 5)
                ),
                new State("GG",
                    new Suicide(500)
                )
            );

        [CharacterBehavior("Pentaract Eye5")]
        public static State PentaractEye5 =>
            new(
                new State("Protect",
                    new Wander(2, 1, 4),
                    new Protect(8, "Pentaract Tower5", 20, 10),
                    new Follow(6, 0.5F, 10),
                    new Shoot(path: new DeceleratePath(7), count: 1, maxRadius: 10, projName: "White Bolt", damage: 50, cooldownMS: 3000, lifetimeMs: 2000, targeted: true),
                    new EntityNotWithinTransition("GG", "Pentaract Tower5", 5)
                ),
                new State("GG",
                    new Suicide(500)
                )
            );
    }
}