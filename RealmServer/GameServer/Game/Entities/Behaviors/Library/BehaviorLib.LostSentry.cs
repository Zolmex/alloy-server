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
        [CharacterBehavior("LH Lost Sentry")]
        public static State LHLostSentry =>
            new(
                new State("START",
                    new TossObject("SENTRY ANCHOR1", minRange: 12, maxRange: 12, angle: 60, cooldownMS: 1000000),
                    new TossObject("SENTRY ANCHOR2", minRange: 9, maxRange: 9, angle: 1, cooldownMS: 1000000),
                    new TossObject("SENTRY ANCHOR3", minRange: 13, maxRange: 13, angle: 250, cooldownMS: 1000000),
                    new TossObject("SENTRY ANCHOR4", minRange: 6, maxRange: 6, angle: 160, cooldownMS: 1000000),
                    new TossObject("SENTRY ANCHOR5", minRange: 15, maxRange: 15, angle: 330, cooldownMS: 1000000),
                    new HpLessTransition(0.99F, "FLASH1")
                ),
                new State("FLASH1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new Flash(0xF73626, 0.5F, 4),
                    new Spawn("LH Spectral Sentry", cooldownMs: 1000000, minSpawnCount: 1, maxSpawnCount: 1),
                    new TimedTransition(2000, "SHAPES")
                ),
                new State("FLASH2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new Flash(0xF73626, 0.5F, 4),
                    new TimedTransition(2000, "SHAPES")
                ),
                new State("SHAPES",
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, yOffset: -0.3F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, yOffset: 0.3F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 200, fixedAngle: 0, yOffset: -0.8F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 200, fixedAngle: 0, yOffset: 0.8F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 400, fixedAngle: 0, yOffset: -0.8F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 400, fixedAngle: 0, yOffset: 0.8F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 540, fixedAngle: 0, yOffset: -0.3F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 540, fixedAngle: 0, yOffset: 0.3F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, xOffset: -0.3F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, xOffset: 0.3F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 200, fixedAngle: 90, xOffset: -0.8F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 200, fixedAngle: 90, xOffset: 0.8F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 400, fixedAngle: 90, xOffset: -0.8F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 400, fixedAngle: 90, xOffset: 0.8F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 540, fixedAngle: 90, xOffset: -0.3F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 540, fixedAngle: 90, xOffset: 0.3F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 45, xOffset: 0.45F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 45, yOffset: 0.45F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 350, fixedAngle: 45, xOffset: 0.9F, yOffset: -0.4F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 350, fixedAngle: 45, xOffset: -0.4F, yOffset: 0.9F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 650, fixedAngle: 45, xOffset: 0.9F, yOffset: -0.4F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 650, fixedAngle: 45, xOffset: -0.4F, yOffset: 0.9F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 880, fixedAngle: 45, xOffset: 0.45F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 880, fixedAngle: 45, yOffset: 0.45F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 135, xOffset: -0.45F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 135, yOffset: 0.45F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 350, fixedAngle: 135, xOffset: -0.9F, yOffset: -0.4F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 350, fixedAngle: 135, xOffset: 0.4F, yOffset: 0.9F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 650, fixedAngle: 135, xOffset: -0.9F, yOffset: -0.4F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 650, fixedAngle: 135, xOffset: 0.4F, yOffset: 0.9F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 880, fixedAngle: 135, xOffset: -0.45F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new LinePath(2), count: 2, projName: "LH Red", maxRadius: 20, cooldownMS: 6000, lifetimeMs: 80000, damage: 100, targeted: false, shootAngle: 180, coolDownOffset: 880, fixedAngle: 135, yOffset: 0.45F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.ArmorBroken, 2000)),
                    new Shoot(path: new CirclePath(-0.05F, 15), projName: "LH Yellow", maxRadius: 20, cooldownMS: 2500, coolDownOffset: 1000, lifetimeMs: 3000, damage: 100, targeted: false, fixedAngle: 0, xOffset: -15, multiHit: true, effects: (ConditionEffectIndex.Paralyzed, 1000)),
                    new Shoot(path: new CirclePath(-0.08F, 10), projName: "LH Yellow", maxRadius: 20, cooldownMS: 3000, coolDownOffset: 1500, lifetimeMs: 3000, damage: 100, targeted: false, fixedAngle: 90, yOffset: -10, multiHit: true, effects: (ConditionEffectIndex.Paralyzed, 1000)),
                    new Shoot(path: new CirclePath(0.03F, 15), projName: "LH Yellow", maxRadius: 20, cooldownMS: 4500, coolDownOffset: 500, lifetimeMs: 3000, damage: 100, targeted: false, fixedAngle: 180, xOffset: 15, multiHit: true, effects: (ConditionEffectIndex.Paralyzed, 1000)),
                    new Shoot(path: new CirclePath(-0.06F, 20), projName: "LH Yellow", maxRadius: 20, cooldownMS: 2000, coolDownOffset: 1000, lifetimeMs: 3000, damage: 100, targeted: false, fixedAngle: 270, yOffset: 20, multiHit: true, effects: (ConditionEffectIndex.Paralyzed, 1000)),
                    new Shoot(path: new CirclePath(0.05F, 15), projName: "LH Yellow", maxRadius: 20, cooldownMS: 2500, coolDownOffset: 200, lifetimeMs: 3000, damage: 100, targeted: false, fixedAngle: 0, xOffset: -15, multiHit: true, effects: (ConditionEffectIndex.Paralyzed, 1000)),
                    new Shoot(path: new CirclePath(0.08F, 10), projName: "LH Yellow", maxRadius: 20, cooldownMS: 3000, coolDownOffset: 500, lifetimeMs: 3000, damage: 100, targeted: false, fixedAngle: 90, yOffset: -10, multiHit: true, effects: (ConditionEffectIndex.Paralyzed, 1000)),
                    new Shoot(path: new CirclePath(-0.03F, 15), projName: "LH Yellow", maxRadius: 20, cooldownMS: 4000, coolDownOffset: 1000, lifetimeMs: 3000, damage: 100, targeted: false, fixedAngle: 180, xOffset: 15, multiHit: true, effects: (ConditionEffectIndex.Paralyzed, 1000)),
                    new Shoot(path: new CirclePath(0.06F, 20), projName: "LH Yellow", maxRadius: 20, cooldownMS: 3000, coolDownOffset: 2000, lifetimeMs: 3000, damage: 100, targeted: false, fixedAngle: 270, yOffset: 20, multiHit: true, effects: (ConditionEffectIndex.Paralyzed, 1000)),
                    new TimedTransition(10000, "SHAPES2"),
                    new HpLessTransition(0.50F, "SPOOOOKY BOII")
                ),
                new State("SHAPES2",
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2020, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2220, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2220, yOffset: 0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2220, yOffset: -0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, yOffset: 1F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, yOffset: -1F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, yOffset: 0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, yOffset: -0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4020, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4220, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4220, xOffset: 0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4220, xOffset: -0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, xOffset: 1F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, xOffset: -1F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, xOffset: 0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, xOffset: -0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new AmplitudePath(3, 0.6F, 1.8F), count: 4, projName: "LH Gray", maxRadius: 20, lifetimeMs: 3420, cooldownMS: 130, damage: 100, targeted: false, shootAngle: 90, fixedAngle: 45, rotateAngle: 0.9F, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(3, 0.6F, 1.8F), count: 4, projName: "LH Gray", maxRadius: 20, lifetimeMs: 3420, cooldownMS: 130, damage: 100, targeted: false, shootAngle: 90, fixedAngle: 135, rotateAngle: 0.9F, multiHit: true, armorPiercing: true),
                    new AOE(3, range: 9, damage: 100, cooldownMs: 1000, color: 0xF73626, fixedAngle: 270, rotateAngle: 20),
                    new AOE(3, range: 9, damage: 100, cooldownMs: 1000, color: 0xF73625, fixedAngle: 45, rotateAngle: -20),
                    new AOE(5, range: 3, damage: 100, cooldownOffset: 1000, cooldownMs: 3000, color: 0xA8251A, fixedAngle: 90, rotateAngle: 90),
                    new TimedTransition(15000, "SHAPES3"),
                    new HpLessTransition(0.50F, "SPOOOOKY BOII")
                ),
                new State("SHAPES3",
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2020, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2220, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2220, yOffset: 0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2220, yOffset: -0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, yOffset: 1F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, yOffset: -1F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, yOffset: 0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, yOffset: -0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 0, coolDownOffset: 2420, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4020, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4220, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4220, xOffset: 0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4220, xOffset: -0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, xOffset: 1F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, xOffset: -1F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, xOffset: 0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, xOffset: -0.5F, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(3), count: 2, projName: "LH Blue", maxRadius: 20, cooldownMS: 3000, lifetimeMs: 20000, damage: 100, targeted: false, shootAngle: 180, fixedAngle: 90, coolDownOffset: 4420, armorPiercing: true, multiHit: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new AmplitudePath(3, 0.6F, 1.8F), count: 4, projName: "LH Gray", maxRadius: 20, lifetimeMs: 3420, cooldownMS: 130, damage: 100, targeted: false, shootAngle: 90, fixedAngle: 135, rotateAngle: -0.9F, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(3, 0.6F, 1.8F), count: 4, projName: "LH Gray", maxRadius: 20, lifetimeMs: 3420, cooldownMS: 130, damage: 100, targeted: false, shootAngle: 90, fixedAngle: 45, rotateAngle: -0.9F, multiHit: true, armorPiercing: true),
                    new AOE(3, range: 9, damage: 100, cooldownMs: 1000, color: 0xF73626, fixedAngle: 90, rotateAngle: 20),
                    new AOE(3, range: 9, damage: 100, cooldownMs: 1000, color: 0xF73625, fixedAngle: 45, rotateAngle: -20),
                    new AOE(5, range: 3, damage: 100, cooldownOffset: 1000, cooldownMs: 3000, color: 0xA8251A, fixedAngle: 225, rotateAngle: 90),
                    new TimedTransition(15000, "FLASH2"),
                    new HpLessTransition(0.50F, "SPOOOOKY BOII")
                ),
                new State("SPOOOOKY BOII",
                    new TossObject("LH SENTRY TRANSITION", cooldownMS: 100000000, maxRange: 0.1F, minRange: 0.1F, angle: 1),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TimedTransition(2000, "WAIT")
                ),
                new State("WAIT",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 0, rotateAngle: 15, projName: "LH Blue", lifetimeMs: 3000, damage: 100, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 10, rotateAngle: 15, projName: "LH Blue", lifetimeMs: 3000, damage: 100, coolDownOffset: 200, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 20, rotateAngle: 15, projName: "LH Blue", lifetimeMs: 3000, damage: 100, coolDownOffset: 400, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: 30, rotateAngle: 15, projName: "LH Blue", lifetimeMs: 3000, damage: 100, coolDownOffset: 600, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -10, rotateAngle: 15, projName: "LH Blue", lifetimeMs: 3000, damage: 100, coolDownOffset: 200, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -20, rotateAngle: 15, projName: "LH Blue", lifetimeMs: 3000, damage: 100, coolDownOffset: 400, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new Shoot(path: new LinePath(5), count: 4, shootAngle: 90, fixedAngle: -30, rotateAngle: 15, projName: "LH Blue", lifetimeMs: 3000, damage: 100, coolDownOffset: 600, cooldownMS: 2500, maxRadius: 40, targeted: false, size: 100, multiHit: true, armorPiercing: true, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new EntityNotWithinTransition("NOO", "LH Spectral Sentry", 20)
                ),
                new State("NOO",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Flash(0xF73626, 1F, 3),
                    new SetAltTexture(3),
                    new TimedTransition(2000, "DIE")
                ),
                new State("DIE",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new Shoot(path: new CirclePath(0.05F, 11), projName: "LH Gray", maxRadius: 30, cooldownMS: 500, lifetimeMs: 5200, damage: 100, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new CirclePath(0.05F, 11), projName: "LH Gray", maxRadius: 30, cooldownMS: 500, lifetimeMs: 5200, damage: 100, targeted: false, multiHit: true, armorPiercing: true, fixedAngle: 180),
                    new Shoot(path: new CirclePath(0.05F, 11), projName: "LH Gray", maxRadius: 30, cooldownMS: 500, lifetimeMs: 5200, damage: 100, targeted: false, multiHit: true, armorPiercing: true, fixedAngle: 90),
                    new Shoot(path: new CirclePath(0.05F, 11), projName: "LH Gray", maxRadius: 30, cooldownMS: 500, lifetimeMs: 5200, damage: 100, targeted: false, multiHit: true, armorPiercing: true, fixedAngle: 270),
                    new Shoot(path: new AmplitudePath(0.5F, 8F, 0.51F), count: 2, projName: "LH Gray", maxRadius: 20, lifetimeMs: 5000, cooldownMS: 200, damage: 100, targeted: false, shootAngle: 90, fixedAngle: 90, rotateAngle: 3, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(0.5F, 8F, 0.51F), count: 2, projName: "LH Gray", maxRadius: 20, lifetimeMs: 5000, cooldownMS: 200, damage: 100, targeted: false, shootAngle: 90, fixedAngle: 180, rotateAngle: 3, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(0.5F, 8F, 0.51F), count: 2, projName: "LH Gray", maxRadius: 20, lifetimeMs: 5000, cooldownMS: 200, damage: 100, targeted: false, shootAngle: 90, fixedAngle: 270, rotateAngle: 3, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(0.5F, 8F, 0.51F), count: 2, projName: "LH Gray", maxRadius: 20, lifetimeMs: 5000, cooldownMS: 200, damage: 100, targeted: false, shootAngle: 90, fixedAngle: 360, rotateAngle: 3, multiHit: true, armorPiercing: true),
                    new Shoot(path: new BoomerangPath(3), count: 9, projName: "LH Red", maxRadius: 20, cooldownMS: 2000, lifetimeMs: 7000, damage: 100, targeted: false, shootAngle: 20, fixedAngle: 315, rotateAngle: 20, armorPiercing: true, multiHit: true),
                    new Shoot(path: new BoomerangPath(3), count: 9, projName: "LH Red", maxRadius: 20, cooldownMS: 2000, lifetimeMs: 7000, damage: 100, targeted: false, shootAngle: 20, fixedAngle: 135, rotateAngle: 20, armorPiercing: true, multiHit: true)
                )
            );


        [CharacterBehavior("SENTRY ANCHOR1")]
        public static State SENTRYANCHOR1 =>
            new (
                new State("GG",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("GGG", "LH Lost Sentry", 30)
                ),
                new State("GGG",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("SENTRY ANCHOR2")]
        public static State SENTRYANCHOR2 =>
            new(
                new State("GG",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("GGG", "LH Lost Sentry", 30)
                ),
                new State("GGG",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("SENTRY ANCHOR3")]
        public static State SENTRYANCHOR3 =>
            new(
                new State("GG",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("GGG", "LH Lost Sentry", 30)
                ),
                new State("GGG",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("SENTRY ANCHOR4")]
        public static State SENTRYANCHOR4 =>
            new(
                new State("GG",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("GGG", "LH Lost Sentry", 30)
                ),
                new State("GGG",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("SENTRY ANCHOR5")]
        public static State SENTRYANCHOR5 =>
            new(
                new State("GG",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("GGG", "LH Lost Sentry", 30)
                ),
                new State("GGG",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("LH SENTRY TRANSITION")]
        public static State LHSENTRYTRANSITION =>
            new(
                new State("GG",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("GGG", "LH Spectral Sentry", 30)
                ),
                new State("GGG",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("LH Spectral Sentry")]
        public static State LHSpectralSentry =>
            new(
                new State("START",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(3, 3, 10, "LH Lost Sentry", 0, 0, true),
                    new TimedTransition(8000, "JUMP1"),
                    new EntityWithinTransition("DIEEE", "LH SENTRY TRANSITION", 30)
                ),
                new State("JUMP1",
                    new SetAltTexture(1),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(9, 2, 30, "SENTRY ANCHOR1", 0, 0, true),
                    new TimedTransition(1600, "JUMP1-1")
                ),
                new State("JUMP1-1",
                    new SetAltTexture(0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(3, 2, 30, "SENTRY ANCHOR1", 0, 0, true),
                    new Shoot(path: new DeceleratePath(4), count: 8, projName: "LH Spectral Green1", lifetimeMs: 4000, cooldownMS: 2000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(4, 1, 1, lifetimeMs: 4000), count: 8, projName: "LH Spectral Green2", lifetimeMs: 8000, cooldownMS: 2000, coolDownOffset: 1000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new TimedTransition(8000, "JUMP2"),
                    new EntityWithinTransition("DIEEE", "LH SENTRY TRANSITION", 30)
                ),
                new State("JUMP2",
                    new SetAltTexture(1),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(9, 2, 30, "SENTRY ANCHOR4", 0, 0, true),
                    new TimedTransition(2000, "JUMP2-1")
                ),
                new State("JUMP2-1",
                    new SetAltTexture(0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(3, 2, 30, "SENTRY ANCHOR4", 0, 0, true),
                    new Shoot(path: new DeceleratePath(4), count: 8, projName: "LH Spectral Green1", lifetimeMs: 4000, cooldownMS: 2000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(4, 1, 1, lifetimeMs: 4000), count: 8, projName: "LH Spectral Green2", lifetimeMs: 8000, cooldownMS: 2000, coolDownOffset: 1000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new TimedTransition(8000, "JUMP3"),
                    new EntityWithinTransition("DIEEE", "LH SENTRY TRANSITION", 30)
                ),
                new State("JUMP3",
                    new SetAltTexture(1),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(9, 2, 30, "SENTRY ANCHOR3", 0, 0, true),
                    new TimedTransition(2000, "JUMP3-1")
                ),
                new State("JUMP3-1",
                    new SetAltTexture(0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(3, 2, 30, "SENTRY ANCHOR3", 0, 0, true),
                    new Shoot(path: new DeceleratePath(4), count: 8, projName: "LH Spectral Green1", lifetimeMs: 4000, cooldownMS: 2000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(4, 1, 1, lifetimeMs: 4000), count: 8, projName: "LH Spectral Green2", lifetimeMs: 8000, cooldownMS: 2000, coolDownOffset: 1000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new TimedTransition(8000, "JUMP4"),
                    new EntityWithinTransition("DIEEE", "LH SENTRY TRANSITION", 30)
                ),
                new State("JUMP4",
                    new SetAltTexture(1),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(9, 2, 30, "SENTRY ANCHOR2", 0, 0, true),
                    new TimedTransition(2000, "JUMP4-1")
                ),
                new State("JUMP4-1",
                    new SetAltTexture(0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(3, 2, 30, "SENTRY ANCHOR2", 0, 0, true),
                    new Shoot(path: new DeceleratePath(4), count: 8, projName: "LH Spectral Green1", lifetimeMs: 4000, cooldownMS: 2000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(4, 1, 1, lifetimeMs: 4000), count: 8, projName: "LH Spectral Green2", lifetimeMs: 8000, cooldownMS: 2000, coolDownOffset: 1000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new TimedTransition(8000, "JUMP5"),
                    new EntityWithinTransition("DIEEE", "LH SENTRY TRANSITION", 30)
                ),
                new State("JUMP5",
                    new SetAltTexture(1),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(9, 2, 30, "SENTRY ANCHOR5", 0, 0, true),
                    new TimedTransition(1500, "JUMP5-1")
                ),
                new State("JUMP5-1",
                    new SetAltTexture(0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(3, 2, 30, "SENTRY ANCHOR5", 0, 0, true),
                    new Shoot(path: new DeceleratePath(4), count: 8, projName: "LH Spectral Green1", lifetimeMs: 4000, cooldownMS: 2000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(4, 1, 1, lifetimeMs: 4000), count: 8, projName: "LH Spectral Green2", lifetimeMs: 8000, cooldownMS: 2000, coolDownOffset: 1000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new TimedTransition(8000, "JUMP6"),
                    new EntityWithinTransition("DIEEE", "LH SENTRY TRANSITION", 30)
                ),
                new State("JUMP6",
                    new SetAltTexture(1),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(9, 2, 30, "LH Lost Sentry", 0, 0, true),
                    new TimedTransition(2000, "JUMP6-1")
                ),
                new State("JUMP6-1",
                    new SetAltTexture(0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit(3, 2, 30, "LH Lost Sentry", 0, 0, true),
                    new Shoot(path: new DeceleratePath(4), count: 8, projName: "LH Spectral Green1", lifetimeMs: 4000, cooldownMS: 2000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(4, 1, 1, lifetimeMs: 4000), count: 8, projName: "LH Spectral Green2", lifetimeMs: 8000, cooldownMS: 2000, coolDownOffset: 1000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new TimedTransition(8000, "JUMP1"),
                    new EntityWithinTransition("DIEEE", "LH SENTRY TRANSITION", 30)
                ),
                new State("DIEEE",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 1000),
                    new Protect(5, "LH Lost Sentry", 20, 8),
                    new Wander(2, 1, 3),
                    new Follow(5, 1, 10),
                    new Shoot(path: new DeceleratePath(4), count: 12, projName: "LH Spectral Green1", lifetimeMs: 4000, cooldownMS: 2000, coolDownOffset: 500, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new AmplitudePath(4, 1, 1, lifetimeMs: 4000), count: 12, projName: "LH Spectral Green2", lifetimeMs: 8000, cooldownMS: 2000, coolDownOffset: 1000, maxRadius: 20, damage: 100, shootAngle: 45, targeted: false, multiHit: true, armorPiercing: true),
                    new Shoot(path: new DeceleratePath(4), count: 6, projName: "LH Spectral Green2", lifetimeMs: 4000, cooldownMS: 1500, coolDownOffset: 500, maxRadius: 20, damage: 100, shootAngle: 5, targeted: true, multiHit: true, armorPiercing: true)
                )
            );
    }
}