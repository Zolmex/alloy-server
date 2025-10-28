#region

using Common;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;
using GameServer.Game.Entities.Loot;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public partial class BehaviorLib
    {
        [CharacterBehavior("Oryx the Mad God 2")]
        public static State OryxTheMadGod2 =>
            new(
                new CharacterLoot(
                    new ItemLoot("Potion of Vitality", 1f, 0.29f),
                    new ItemLoot("Potion of Attack", 0.3f, 0.05f),
                    new ItemLoot("Potion of Defense", 0.3f, 0.05f),
                    new ItemLoot("Potion of Wisdom", 0.3f, 0.05f)
                    // new ItemLoot("Oryx's Arena Key", 0.01f, 0.20f)
                    // new TierLoot(10, ItemType.Weapon, 0.07f, 0.1f),
                    // new TierLoot(11, ItemType.Weapon, 0.06f, 0.1f),
                    // new TierLoot(12, ItemType.Weapon, 0.05f, 0.1f),
                    // new TierLoot(5, ItemType.Ability, 0.07f, 0.1f),
                    // new TierLoot(6, ItemType.Ability, 0.05f, 0.1f),
                    // new TierLoot(11, ItemType.Armor, 0.07f, 0.1f),
                    // new TierLoot(12, ItemType.Armor, 0.06f, 0.1f),
                    // new TierLoot(13, ItemType.Armor, 0.05f, 0.1f),
                    // new TierLoot(5, ItemType.Ring, 0.06f, 0.1f)
                ),
                // new ScaleHP2(30),
                new State("Attack",
                    new Wander((0.05f * 5.55f) + 0.74f), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 25,
                        count: 8,
                        shootAngle: 45,
                        cooldownMS: 1500,
                        coolDownOffset: 1500,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        predictive: 0.2f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 25,
                        count: 2,
                        shootAngle: 10,
                        predictive: 0.4f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        predictive: 0.6f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 5,
                        maxRadius: 25,
                        count: 2,
                        shootAngle: 10,
                        predictive: 0.8f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 6,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        predictive: 1f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Taunt(coolDownMS: 6000, text: "Puny mortals! My {HP} HP will annihilate you!"),
                    new Spawn("Henchman of Oryx", maxSpawnsPerReset: 5, cooldownMs: 5000),
                    new HpLessTransition(0.2f, "prepareRage")
                ),
                new State("prepareRage",
                    new Follow((0.1f * 5.55f) + 0.74f, acquireRange: 15, distFromTarget: 3), // Adjusted speed
                    new Taunt("Can't... keep... henchmen... alive... anymore! ARGHHH!!!"),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(
                        projectileIndex: 13,
                        maxRadius: 25,
                        fixedAngle: 0,
                        cooldownMS: 4000,
                        coolDownOffset: 4000
                    ),
                    new Shoot(
                        projectileIndex: 14,
                        maxRadius: 25,
                        fixedAngle: 30,
                        cooldownMS: 4000,
                        coolDownOffset: 4000
                    ),
                    new TimedTransition(10000, "rage")
                ),
                new State("rage",
                    new Follow((0.1f * 5.55f) + 0.74f, acquireRange: 15, distFromTarget: 3), // Adjusted speed
                    new Shoot(
                        projectileIndex: 13,
                        maxRadius: 25,
                        cooldownMS: 90000001,
                        coolDownOffset: 8000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 14,
                        maxRadius: 25,
                        cooldownMS: 90000001,
                        coolDownOffset: 8500,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 25,
                        count: 8,
                        shootAngle: 45,
                        cooldownMS: 1500,
                        coolDownOffset: 1500,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        predictive: 0.2f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 25,
                        count: 2,
                        shootAngle: 10,
                        predictive: 0.4f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        predictive: 0.6f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 5,
                        maxRadius: 25,
                        count: 2,
                        shootAngle: 10,
                        predictive: 0.8f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new Shoot(
                        projectileIndex: 6,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        predictive: 1f,
                        cooldownMS: 1000,
                        coolDownOffset: 1000,
                        targeted: true
                    ),
                    new TossObject("Monstrosity Scarab", 7, 0, 1000),
                    new Taunt(coolDownMS: 6000, text: "Puny mortals! My {HP} HP will annihilate you!")
                )
            );

        [CharacterBehavior("Oryx the Mad God 1")]
        public static State OryxTheMadGod1 =>
            new(
                new CharacterLoot(
                    // new ItemLoot("Oryx's Arena Key", 0.005f, 0.20f),
                    new ItemLoot("Potion of Attack", 0.3f, 0.05f),
                    new ItemLoot("Potion of Defense", 0.3f, 0.05f)
                    // new TierLoot(10, ItemType.Weapon, 0.07f, 0.1f),
                    // new TierLoot(11, ItemType.Weapon, 0.06f, 0.1f),
                    // new TierLoot(5, ItemType.Ability, 0.07f, 0.1f),
                    // new TierLoot(11, ItemType.Armor, 0.07f, 0.1f),
                    // new TierLoot(5, ItemType.Ring, 0.06f, 0.1f)
                ),
                new State("0",
                    new DropPortalOnDeath("Wine Cellar Portal", 100, 120),
                    new HpLessTransition(0.2f, "rage"),
                    new State("Slow",
                        new Taunt("Fools! I still have {HP} hitpoints!"),
                        new Spawn("Minion of Oryx", maxSpawnsPerReset: 5, cooldownMs: 350000),
                        new Reproduce("Minion of Oryx", densityRadius: 10, maxDensity: 5, cooldownMs: 1500),
                        new Shoot(
                            projectileIndex: 5,
                            maxRadius: 25,
                            count: 4,
                            shootAngle: 10,
                            cooldownMS: 1000,
                            targeted: true // Added
                        ),
                        new TimedTransition(20000, "Dance 1")
                    ),
                    new State("Dance 1",
                        new Flash(0xf389E13, 0.5f, 60),
                        new Taunt("BE SILENT!!!"),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new Shoot(
                            projectileIndex: 6,
                            maxRadius: 50,
                            count: 8,
                            shootAngle: 45,
                            cooldownMS: 700,
                            coolDownOffset: 200,
                            targeted: true // Added
                        ),
                        new TossObject("Ring Element", 9, 24, 320000),
                        new TossObject("Ring Element", 9, 48, 320000),
                        new TossObject("Ring Element", 9, 72, 320000),
                        new TossObject("Ring Element", 9, 96, 320000),
                        new TossObject("Ring Element", 9, 120, 320000),
                        new TossObject("Ring Element", 9, 144, 320000),
                        new TossObject("Ring Element", 9, 168, 320000),
                        new TossObject("Ring Element", 9, 192, 320000),
                        new TossObject("Ring Element", 9, 216, 320000),
                        new TossObject("Ring Element", 9, 240, 320000),
                        new TossObject("Ring Element", 9, 264, 320000),
                        new TossObject("Ring Element", 9, 288, 320000),
                        new TossObject("Ring Element", 9, 312, 320000),
                        new TossObject("Ring Element", 9, 336, 320000),
                        new TossObject("Ring Element", 9, 360, 320000),
                        new TimedTransition(25000, "artifacts")
                    ),
                    new State("artifacts",
                        new Taunt("My Artifacts will protect me!"),
                        new Flash(0xf389E13, 0.5f, 60),
                        new Shoot(
                            projectileIndex: 9,
                            maxRadius: 50,
                            count: 3,
                            shootAngle: 120,
                            cooldownMS: 1500,
                            coolDownOffset: 200,
                            targeted: true // Added
                        ),
                        new Shoot(
                            projectileIndex: 8,
                            maxRadius: 50,
                            count: 10,
                            shootAngle: 36,
                            cooldownMS: 2000,
                            coolDownOffset: 200,
                            targeted: true // Added
                        ),
                        new Shoot(
                            projectileIndex: 7,
                            maxRadius: 50,
                            count: 10,
                            shootAngle: 36,
                            cooldownMS: 500,
                            coolDownOffset: 200,
                            targeted: true // Added
                        ),
                        new TossObject("Guardian Element 1", 1, 0, 90000001,
                            1000),
                        new TossObject("Guardian Element 1", 1, 90, 90000001,
                            1000),
                        new TossObject("Guardian Element 1", 1, 180, 90000001,
                            1000),
                        new TossObject("Guardian Element 1", 1, 270, 90000001,
                            1000),
                        new TossObject("Guardian Element 2", 9, 0, 90000001,
                            1000),
                        new TossObject("Guardian Element 2", 9, 90, 90000001,
                            1000),
                        new TossObject("Guardian Element 2", 9, 180, 90000001,
                            1000),
                        new TossObject("Guardian Element 2", 9, 270, 90000001,
                            1000),
                        new TimedTransition(25000, "gaze")
                    ),
                    new State("gaze",
                        new Taunt("All who looks upon my face shall die."),
                        new Shoot(
                            projectileIndex: 1,
                            maxRadius: 7,
                            count: 2,
                            shootAngle: 10,
                            cooldownMS: 1000,
                            coolDownOffset: 800,
                            targeted: true // Added
                        ),
                        new TimedTransition(10000, "Dance 2")
                    ),
                    new State("Dance 2",
                        new Flash(0xf389E13, 0.5f, 60),
                        new Taunt("Time for more dancing!"),
                        new Shoot(
                            projectileIndex: 6,
                            maxRadius: 50,
                            count: 8,
                            shootAngle: 45,
                            cooldownMS: 700,
                            coolDownOffset: 200,
                            targeted: true // Added
                        ),
                        new TossObject("Ring Element", 9, 24, 320000),
                        new TossObject("Ring Element", 9, 48, 320000),
                        new TossObject("Ring Element", 9, 72, 320000),
                        new TossObject("Ring Element", 9, 96, 320000),
                        new TossObject("Ring Element", 9, 120, 320000),
                        new TossObject("Ring Element", 9, 144, 320000),
                        new TossObject("Ring Element", 9, 168, 320000),
                        new TossObject("Ring Element", 9, 192, 320000),
                        new TossObject("Ring Element", 9, 216, 320000),
                        new TossObject("Ring Element", 9, 240, 320000),
                        new TossObject("Ring Element", 9, 264, 320000),
                        new TossObject("Ring Element", 9, 288, 320000),
                        new TossObject("Ring Element", 9, 312, 320000),
                        new TossObject("Ring Element", 9, 336, 320000),
                        new TossObject("Ring Element", 9, 360, 320000),
                        new TimedTransition(1000, "Dance2, 1")
                    ),
                    new State("Dance2, 1",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new Shoot(
                            projectileIndex: 8,
                            maxRadius: 0,
                            count: 4,
                            shootAngle: 90,
                            fixedAngle: 0,
                            cooldownMS: 170
                        ),
                        new TimedTransition(200, "Dance2, 2")
                    ),
                    new State("Dance2, 2",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new Shoot(
                            projectileIndex: 8,
                            maxRadius: 0,
                            count: 4,
                            shootAngle: 90,
                            fixedAngle: 30,
                            cooldownMS: 170
                        ),
                        new TimedTransition(200, "Dance2, 3")
                    ),
                    new State("Dance2, 3",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new Shoot(
                            projectileIndex: 8,
                            maxRadius: 0,
                            count: 4,
                            shootAngle: 90,
                            fixedAngle: 15,
                            cooldownMS: 170
                        ),
                        new TimedTransition(200, "Dance2, 4")
                    ),
                    new State("Dance2, 4",
                        new Shoot(
                            projectileIndex: 8,
                            maxRadius: 0,
                            count: 4,
                            shootAngle: 90,
                            fixedAngle: 45,
                            cooldownMS: 170
                        ),
                        new TimedTransition(200, "Dance2, 1")
                    ),
                    new State("rage",
                        new ChangeSize(target: 10, rate: 200),
                        new Taunt("I HAVE HAD ENOUGH OF YOU!!!||ENOUGH!!!||DIE!!!", 300),
                        new Spawn("Minion of Oryx", maxSpawnsPerReset: 10, cooldownMs: 350000),
                        new Reproduce("Minion of Oryx", maxDensity: 10, cooldownMs: 1500),
                        new Shoot(
                            projectileIndex: 1,
                            maxRadius: 7,
                            count: 2,
                            shootAngle: 10,
                            cooldownMS: 1500,
                            coolDownOffset: 2000,
                            targeted: true // Added
                        ),
                        new Shoot(
                            projectileIndex: 16,
                            maxRadius: 7,
                            count: 5,
                            shootAngle: 10,
                            cooldownMS: 1500,
                            coolDownOffset: 2000,
                            targeted: true // Added
                        ),
                        new Follow((0.85f * 5.55f) + 0.74f, acquireRange: 1, distFromTarget: 0), // Adjusted speed
                        new Flash(0xFF0000, 0.5f, 9000001)
                    )
                )
            );

        [CharacterBehavior("Ring Element")]
        public static State RingElement =>
            new(
                new State("0",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 50,
                        count: 12,
                        shootAngle: 36,
                        cooldownMS: 700,
                        coolDownOffset: 200,
                        targeted: true // Added
                    ),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(20000, "Despawn")
                ),
                new State("Despawn",
                    new Suicide()
                )
            );

        [CharacterBehavior("Minion of Oryx")]
        public static State MinionOfOryx =>
            new(
                new CharacterLoot(
                    // new TierLoot(7, ItemType.Weapon, 0.2f),
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Wander((0.4f * 5.55f) + 0.74f), // Adjusted speed
                new Shoot(
                    projectileIndex: 0,
                    maxRadius: 3,
                    count: 3,
                    shootAngle: 10,
                    cooldownMS: 1000,
                    targeted: true // Added
                ),
                new Shoot(
                    projectileIndex: 1,
                    maxRadius: 3,
                    count: 3,
                    shootAngle: 10,
                    cooldownMS: 1000,
                    targeted: true // Added
                )
            );

        [CharacterBehavior("Guardian Element 1")]
        public static State GuardianElement1 =>
            new(
                new State("0",
                    new Orbit(1f, 1f, target: "Oryx the Mad God 1", radiusVariance: 0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new TimedTransition(10000, "Grow")
                ),
                new State("Grow",
                    new ChangeSize(target: 100, rate: 200),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Orbit(1f, 1f, target: "Oryx the Mad God 1", radiusVariance: 0),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 3,
                        count: 1,
                        shootAngle: 10,
                        cooldownMS: 700,
                        targeted: true // Added
                    ),
                    new TimedTransition(10000, "Despawn")
                ),
                new State("Despawn",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Orbit(1f, 1f, target: "Oryx the Mad God 1", radiusVariance: 0),
                    new ChangeSize(target: 100, rate: 100),
                    new Suicide()
                )
            );

        [CharacterBehavior("Guardian Element 2")]
        public static State GuardianElement2 =>
            new(
                new State("0",
                    new Orbit(1.3f, 9f, target: "Oryx the Mad God 1", radiusVariance: 0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 25,
                        count: 3,
                        shootAngle: 10,
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new TimedTransition(20000, "Despawn")
                ),
                new State("Despawn",
                    new Suicide()
                )
            );

        [CharacterBehavior("Henchman of Oryx")]
        public static State HenchmanOfOryx =>
            new(
                new State("Attack",
                    new Orbit((0.2f * 5.55f) + 0.74f, 2f, target: "Oryx the Mad God 2", radiusVariance: 1),
                    new Wander((0.3f * 5.55f) + 0.74f), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 15,
                        predictive: 1f,
                        cooldownMS: 2500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 10,
                        count: 3,
                        shootAngle: 10,
                        cooldownMS: 2500,
                        targeted: true // Added
                    ),
                    new Spawn("Vintner of Oryx", maxSpawnsPerReset: 1, cooldownMs: 5000),
                    new Spawn("Aberrant of Oryx", maxSpawnsPerReset: 1, cooldownMs: 5000),
                    new Spawn("Monstrosity of Oryx", maxSpawnsPerReset: 1, cooldownMs: 5000),
                    new Spawn("Abomination of Oryx", maxSpawnsPerReset: 1, cooldownMs: 5000)
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("Monstrosity of Oryx")]
        public static State MonstrosityOfOryx =>
            new(
                new State("Wait",
                    new EntityWithinTransition(radius: 15, targetState: "Attack")
                ),
                new State("Attack",
                    new TimedTransition(10000, "Wait"),
                    new Orbit((0.1f * 5.55f) + 0.74f, 6f, target: "Oryx the Mad God 2", radiusVariance: 3),
                    new Follow((0.1f * 5.55f) + 0.74f, acquireRange: 15),
                    new Wander((0.2f * 5.55f) + 0.74f), // Adjusted speed
                    new TossObject("Monstrosity Scarab", 1, 0, 10000, 1000)
                )
            );

        [CharacterBehavior("Monstrosity Scarab")]
        public static State MonstrosityScarab =>
            new(
                new State("Attack",
                    new State("Charge",
                        new Charge((1f * 5.55f) + 0.74f, 25, 1000),
                        new Wander((0.3f * 5.55f) + 0.74f), // Adjusted speed
                        new EntityWithinTransition(radius: 1, targetState: "Boom")
                    ),
                    new State("Boom",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 1,
                            count: 16,
                            shootAngle: 360 / 16f,
                            fixedAngle: 0,
                            cooldownMS: 1000
                        ),
                        new Suicide()
                    )
                )
            );

        [CharacterBehavior("Vintner of Oryx")]
        public static State VintnerOfOryx =>
            new(
                new State("Attack",
                    new Protect(6.29f, "Oryx the Mad God 2", protectionRange: 4, reprotectRange: 3),
                    new Charge(6.29f, 15, 2000),
                    new Protect(6.29f, "Henchman of Oryx"),
                    new StayAwayFrom(6.29f, 15),
                    new Wander(6.29f), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        cooldownMS: 250,
                        targeted: true // Added
                    )
                )
            );

        [CharacterBehavior("Aberrant of Oryx")]
        public static State AberrantOfOryx =>
            new(
                new Protect((0.2f * 5.55f) + 0.74f, "Oryx the Mad God 2"),
                new Wander((0.7f * 5.55f) + 0.74f), // Adjusted speed
                new State("Wait",
                    new EntityWithinTransition(radius: 15, targetState: "Attack")
                ),
                new State("Attack",
                    new TimedTransition(10000, "Wait"),
                    new State("Randomize",
                        new TimedTransition(100, "Toss1"),
                        new TimedTransition(100, "Toss2"),
                        new TimedTransition(100, "Toss3"),
                        new TimedTransition(100, "Toss4"),
                        new TimedTransition(100, "Toss5"),
                        new TimedTransition(100, "Toss6"),
                        new TimedTransition(100, "Toss7"),
                        new TimedTransition(100, "Toss8")
                    ),
                    new State("Toss1",
                        new TossObject("Aberrant Blaster", 5, 0, 40000),
                        new TimedTransition(4900, "Randomize")
                    ),
                    new State("Toss2",
                        new TossObject("Aberrant Blaster", 5, 45, 40000),
                        new TimedTransition(4900, "Randomize")
                    ),
                    new State("Toss3",
                        new TossObject("Aberrant Blaster", 5, 90, 40000),
                        new TimedTransition(4900, "Randomize")
                    ),
                    new State("Toss4",
                        new TossObject("Aberrant Blaster", 5, 135, 40000),
                        new TimedTransition(4900, "Randomize")
                    ),
                    new State("Toss5",
                        new TossObject("Aberrant Blaster", 5, 180, 40000),
                        new TimedTransition(4900, "Randomize")
                    ),
                    new State("Toss6",
                        new TossObject("Aberrant Blaster", 5, 225, 40000),
                        new TimedTransition(4900, "Randomize")
                    ),
                    new State("Toss7",
                        new TossObject("Aberrant Blaster", 5, 270, 40000),
                        new TimedTransition(4900, "Randomize")
                    ),
                    new State("Toss8",
                        new TossObject("Aberrant Blaster", 5, 315, 40000),
                        new TimedTransition(4900, "Randomize")
                    )
                )
            );

        [CharacterBehavior("Aberrant Blaster")]
        public static State AberrantBlaster =>
            new(
                new State("Wait",
                    new EntityWithinTransition(radius: 3, targetState: "Boom")
                ),
                new State("Boom",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 5,
                        shootAngle: 7,
                        targeted: true, // Added
                        cooldownMS: 1000
                    ),
                    new Suicide()
                )
            );

        [CharacterBehavior("Bile of Oryx")]
        public static State BileOfOryx =>
            new(
                new Protect(
                    (0.4f * 5.55f) + 0.74f, // Adjusted speed
                    "Oryx the Mad God 2",
                    protectionRange: 5,
                    reprotectRange: 4
                ),
                new Wander((0.5f * 5.55f) + 0.74f) // Adjusted speed
            );

        [CharacterBehavior("Abomination of Oryx")]
        public static State AbominationOfOryx =>
            new(
                new State("Shoot",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 1,
                        count: 3,
                        shootAngle: 5,
                        targeted: true, // Added
                        cooldownMS: 1000
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 1,
                        count: 5,
                        shootAngle: 5,
                        targeted: true, // Added
                        cooldownMS: 1000
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 1,
                        count: 7,
                        shootAngle: 5,
                        targeted: true, // Added
                        cooldownMS: 1000
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 1,
                        count: 5,
                        shootAngle: 5,
                        targeted: true, // Added
                        cooldownMS: 1000
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 1,
                        count: 3,
                        shootAngle: 5,
                        targeted: true, // Added
                        cooldownMS: 1000
                    ),
                    new TimedTransition(1000, "Wait")
                ),
                new State("Wait",
                    new EntityWithinTransition(radius: 2, targetState: "Shoot")
                ),
                new Charge(
                    (3f * 5.55f) + 0.74f, // Adjusted speed
                    10,
                    3000
                ),
                new Wander((0.5f * 5.55f) + 0.74f) // Adjusted speed
            );
    }
}