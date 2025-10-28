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
        [CharacterBehavior("Oryx Stone Guardian Right")]
        public static State OryxStoneGuardianRight =>
            new(
                new CharacterLoot(
                    new ItemLoot("Ancient Stone Sword", 0.003f, 0.03f),
                    // LootTemplates.BasicDrop(threshold: 0.005f),
                    new ItemLoot("Potion of Defense", 1f, 0.0001f)
                ),
                // new ScaleHP2(23),
                new OrderOnDeath(30, "Oryx Stone Guardian Left", "6"),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new SetAltTexture(1),
                    new EntityWithinTransition("Chose", radius: 10)
                ),
                new State("Chose",
                    new SetAltTexture(0),
                    new TimedTransition(100, TransitionType.Random, "1", "2", "3", "4", "5")
                ),
                new State("1",
                    new State("1.1",
                        new Orbit((0.6f * 5.55f) + 0.74f, 5, 20, "Oryx Guardian TaskMaster"),
                        new TimedTransition(1200, "1.2")
                    ),
                    new State("1.2",
                        new SetAltTexture(4),
                        new Orbit((.4f * 5.55f) + 0.74f, 5, 20, "Oryx Guardian TaskMaster"),
                        new Shoot(20, 2, 360 / 2f, fixedAngle: 180, rotateAngle: 30, cooldownMS: 200),
                        new TimedTransition(4000, TransitionType.Random, "Chose")
                    )
                ),
                new State("2",
                    new State("2.1",
                        new Protect((2 * 5.55f) + 0.74f, "Oryx Guardian TaskMaster", 20),
                        new TimedTransition(1000, "2.2")
                    ),
                    new State("2.2",
                        new MoveLine((2 * 5.55f) + 0.74f, 0),
                        new TimedTransition(1000, "2.3")
                    ),
                    new State("2.3",
                        new Sequence(
                            new Shoot(10, 1, fixedAngle: 290, coolDownOffset: 0, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 280, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 270, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 260, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 250, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 240, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 230, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 220, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 210, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 200, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 190, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 180, coolDownOffset: 100, cooldownMS: 100)
                        ),
                        new Sequence(
                            new Shoot(10, 1, fixedAngle: 70, coolDownOffset: 0, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 80, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 90, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 100, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 110, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 120, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 130, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 140, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 150, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 160, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 170, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 180, coolDownOffset: 100, cooldownMS: 100)
                        ),
                        new TimedTransition(4000, TransitionType.Random, "Chose")
                    )
                ),
                new State("3",
                    new State("3.1",
                        new Protect((2 * 5.55f) + 0.74f, "Oryx Guardian TaskMaster", 20),
                        new TimedTransition(1500, "3.2")
                    ),
                    new State("3.2",
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 270, cooldownMS: 1000),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 300, cooldownMS: 1000),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 330, cooldownMS: 1000),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 0, cooldownMS: 1000),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 90, cooldownMS: 1000),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 60, cooldownMS: 1000),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 30, cooldownMS: 1000),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 270, cooldownMS: 1700),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 300, cooldownMS: 1700),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 0, cooldownMS: 1700),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 90, cooldownMS: 1700),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 60, cooldownMS: 1700),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 30, cooldownMS: 1700),
                        new Shoot(0, 1, projectileIndex: 1, fixedAngle: 330, cooldownMS: 1700),
                        new TimedTransition(4000, TransitionType.Random, "Chose")
                    )
                ),
                new State("4",
                    new Charge((2 * 5.55f) + 0.74f, 10f),
                    new Shoot(20, 5, 10, 0, cooldownMS: 1500, targeted: true),
                    new TimedTransition(4000, TransitionType.Random, "Chose")
                ),
                new State("5",
                    new Wander((0.4f * 5.55f) + 0.74f),
                    new Shoot(10, 10, 360 / 10f, 2, 36, cooldownMS: 1000),
                    new TimedTransition(4000, TransitionType.Random, "Chose")
                ),
                new State("6",
                    new State("6.1",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new ReturnToSpawn((2 * 5.55f) + 0.74f, 1),
                        new TimedTransition(1000, "6.2")
                    ),
                    new State("6.2",
                        new SetAltTexture(1),
                        new HealSelf(1000, 2400),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(3000, "6.3")
                    ),
                    new State("6.3",
                        new TossObject("Oryx Guardian Sword", 8, 90, 100000),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new EntityWithinTransition("6.4", "Oryx Guardian Sword", 30)
                    ),
                    new State("6.4",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new SetAltTexture(1),
                        new EntityNotWithinTransition("6.5", "Oryx Guardian Sword", 30)
                    ),
                    new State("6.5",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(1400, "Chose")
                    )
                )
            );

        [CharacterBehavior("Oryx Stone Guardian Left")]
        public static State OryxStoneGuardianLeft =>
            new(
                new CharacterLoot(
                    // LootTemplates.BasicDrop(threshold: 0.005f),
                    new ItemLoot("Potion of Defense", 1f, 0.0001f)
                ),
                // new ScaleHP2(23),
                new OrderOnDeath(30, "Oryx Stone Guardian Right", "6"),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new SetAltTexture(1),
                    new EntityWithinTransition("Chose", radius: 10)
                ),
                new State("Chose",
                    new SetAltTexture(0),
                    new TimedTransition(100, TransitionType.Random, "1", "2", "3", "4", "5")
                ),
                new State("1",
                    new State("1.1",
                        new Orbit((.6f * 5.55f) + 0.74f, 5, 20, "Oryx Guardian TaskMaster"),
                        new TimedTransition(1200, "1.2")
                    ),
                    new State("1.2",
                        new SetAltTexture(4, -1),
                        new Orbit((.4f * 5.55f) + 0.74f, 5, 20, "Oryx Guardian TaskMaster"),
                        new Shoot(20, 2, 360 / 2f, fixedAngle: 180, rotateAngle: 30, cooldownMS: 200),
                        new TimedTransition(4000, TransitionType.Random, "Chose")
                    )
                ),
                new State("2",
                    new State("2.1",
                        new Protect((2 * 5.55f) + 0.74f, "Oryx Guardian TaskMaster", 20),
                        new TimedTransition(1000, "2.2")
                    ),
                    new State("2.2",
                        new MoveLine((2 * 5.55f) + 0.74f, 180),
                        new TimedTransition(1000, "2.3")
                    ),
                    new State("2.3",
                        new Sequence(
                            new Shoot(10, 1, fixedAngle: 250, coolDownOffset: 0, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 260, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 270, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 280, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 290, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 300, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 310, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 320, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 330, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 340, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 350, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 0, coolDownOffset: 100, cooldownMS: 100)
                        ),
                        new Sequence(
                            new Shoot(10, 1, fixedAngle: 110, coolDownOffset: 0, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 100, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 90, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 80, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 70, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 60, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 50, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 40, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 30, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 20, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 10, coolDownOffset: 100, cooldownMS: 100),
                            new Shoot(10, 1, fixedAngle: 0, coolDownOffset: 100, cooldownMS: 100)
                        ),
                        new TimedTransition(4000, TransitionType.Random, "Chose")
                    )
                ),
                new State("3",
                    new State("3.1",
                        new Protect((2 * 5.55f) + 0.74f, "Oryx Guardian TaskMaster", 20),
                        new TimedTransition(1500, "3.2")
                    ),
                    new State("3.2",
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 270, cooldownMS: 1000),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 240, cooldownMS: 1000),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 210, cooldownMS: 1000),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 180, cooldownMS: 1000),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 150, cooldownMS: 1000),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 120, cooldownMS: 1000),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 90, cooldownMS: 1000),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 270, cooldownMS: 1700),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 240, cooldownMS: 1700),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 210, cooldownMS: 1700),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 180, cooldownMS: 1700),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 150, cooldownMS: 1700),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 120, cooldownMS: 1700),
                        new Shoot(10, 1, projectileIndex: 1, fixedAngle: 90, cooldownMS: 1700),
                        new TimedTransition(4000, TransitionType.Random, "Chose")
                    )
                ),
                new State("4",
                    new Charge((2 * 5.55f) + 0.74f, 10f),
                    new Shoot(20, 5, 10, 0, cooldownMS: 1500),
                    new TimedTransition(4000, TransitionType.Random, "Chose")
                ),
                new State("5",
                    new Wander((0.4f * 5.55f) + 0.74f),
                    new Shoot(10, 10, 360 / 10f, 2, 36, cooldownMS: 1000),
                    new TimedTransition(4000, TransitionType.Random, "Chose")
                ),
                new State("6",
                    new State("6.1",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new ReturnToSpawn((2 * 5.55f) + 0.74f, 1),
                        new TimedTransition(1000, "6.2")
                    ),
                    new State("6.2",
                        new SetAltTexture(1),
                        new HealSelf(1000, 2400),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(3000, "6.3")
                    ),
                    new State("6.3",
                        new TossObject("Oryx Guardian Sword", 8, 90, 100000),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new EntityWithinTransition("6.4", "Oryx Guardian Sword", 30)
                    ),
                    new State("6.4",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new SetAltTexture(1),
                        new EntityNotWithinTransition("6.5", "Oryx Guardian Sword", 30)
                    ),
                    new State("6.5",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(1400, "Chose")
                    )
                )
            );

        [CharacterBehavior("Oryx Guardian Sword")]
        public static State OryxGuardianSword =>
            new(
                new State("Attack",
                    new Sequence(
                        new Shoot(80, 5, 360 / 5f, fixedAngle: 72, coolDownOffset: 500, cooldownMS: 1000),
                        new Shoot(80, 5, 360 / 5f, fixedAngle: 36, coolDownOffset: 500, cooldownMS: 1000)
                    ),
                    new HpLessTransition(0.2f, "Death")
                ),
                new State("Death",
                    new AOE(5, 50, 270, range: 8),
                    new Suicide()
                )
            );

        [CharacterBehavior("Oryx Guardian TaskMaster")]
        public static State OryxGuardianTaskMaster =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new DropPortalOnDeath("Oryx Chamber Portal"),
                new State("Idle",
                    new EntitiesNotWithinTransition(100, "Death",
                        ["Oryx Stone Guardian Right", "Oryx Stone Guardian Left"])
                ),
                new State("Death",
                    new Suicide()
                )
            );

        [CharacterBehavior("Oryx's Living Floor")]
        public static State OryxsLivingFloor =>
            new(
                new State("Idle",
                    new EntityWithinTransition(radius: 20, targetState: "Toss")
                ),
                new State("Toss",
                    new TossObject("Quiet Bomb", 10, cooldownMS: 1000),
                    new EntityNotWithinTransition(radius: 20, targetStates: "Idle"),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 5,
                        count: 18,
                        shootAngle: 20,
                        fixedAngle: 0,
                        cooldownMS: 750,
                        targeted: true
                    )
                )
            );

        [CharacterBehavior("Oryx Knight")]
        public static State OryxKnight =>
            new(
                new State("waiting for u bae <3",
                    new EntityWithinTransition(radius: 10, targetState: "tim 4 rekkings")
                ),
                new State("tim 4 rekkings",
                    new Wander((0.2f * 5.55f) + 0.74f), // Adjusted speed
                    new Follow((0.6f * 5.55f) + 0.74f, acquireRange: 10, distFromTarget: 3), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 3,
                        shootAngle: 20,
                        cooldownMS: 350,
                        targeted: true // Added
                    ),
                    new TimedTransition(5000, "tim 4 singular rekt")
                ),
                new State("tim 4 singular rekt",
                    new Wander((0.2f * 5.55f) + 0.74f), // Adjusted speed
                    new Follow((0.7f * 5.55f) + 0.74f, acquireRange: 10, distFromTarget: 3), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 1,
                        cooldownMS: 500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 10,
                        count: 1,
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 10,
                        count: 1,
                        cooldownMS: 450,
                        targeted: true // Added
                    ),
                    new TimedTransition(2500, "tim 4 rekkings")
                )
            );

        [CharacterBehavior("Oryx Pet")]
        public static State OryxPet =>
            new(
                new State("swago baboon",
                    new EntityWithinTransition("anuspiddle", radius: 10)
                ),
                new State("anuspiddle",
                    new Wander((0.2f * 5.55f) + 0.74f),
                    new Follow((0.6f * 5.55f) + 0.74f, 0, 10, -1, 0),
                    new Shoot(10, 1, projectileIndex: 0, cooldownMS: 200, targeted: true)
                )
            );

        [CharacterBehavior("Oryx Insect Commander")]
        public static State OryxInsectCommander =>
            new(
                new State("lol jordan is a nub",
                    new Wander((0.2f * 5.55f) + 0.74f), // Adjusted speed
                    new Reproduce("Oryx Insect Minion", densityRadius: 10, maxDensity: 20, cooldownMs: 500),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 1,
                        cooldownMS: 900,
                        targeted: true // Added
                    )
                )
            );

        [CharacterBehavior("Oryx Insect Minion")]
        public static State OryxInsectMinion =>
            new(
                new State("its SWARMING time",
                    new Wander((0.2f * 5.55f) + 0.74f), // Adjusted speed
                    new Wander((0.4f * 5.55f) + 0.74f, distanceFromSpawn: 8), // Adjusted speed
                    new Follow((0.8f * 5.55f) + 0.74f, acquireRange: 10, distFromTarget: 1), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 5,
                        shootAngle: 72,
                        cooldownMS: 1500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 1,
                        cooldownMS: 230,
                        targeted: true // Added
                    )
                )
            );

        [CharacterBehavior("Oryx Suit of Armor")]
        public static State OryxSuitOfArmor =>
            new(
                new State("idle",
                    new EntityWithinTransition(radius: 8, targetState: "attack me pl0x")
                ),
                new State("attack me pl0x",
                    new TimedTransition(1500, "jordan is stanking")
                ),
                new State("jordan is stanking",
                    new Wander((0.2f * 5.55f) + 0.74f), // Adjusted speed
                    new Follow((0.4f * 5.55f) + 0.74f, acquireRange: 10, distFromTarget: 2), // Adjusted speed
                    new SetAltTexture(1),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 2,
                        shootAngle: 15,
                        cooldownMS: 600,
                        targeted: true // Added
                    ),
                    new HpLessTransition(0.2f, "heal")
                ),
                new State("heal",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new SetAltTexture(0),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        cooldownMS: 200,
                        targeted: true // Added
                    ),
                    new HealSelf(2000, 200),
                    new TimedTransition(1500, "jordan is stanking")
                )
            );

        [CharacterBehavior("Oryx Eye Warrior")]
        public static State OryxEyeWarrior =>
            new(
                new State("swaggin",
                    new EntityWithinTransition(radius: 10, targetState: "penispiddle")
                ),
                new State("penispiddle",
                    new Follow((0.6f * 5.55f) + 0.74f, acquireRange: 10, distFromTarget: 0), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 5,
                        shootAngle: 72,
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 10,
                        count: 1,
                        cooldownMS: 500,
                        targeted: true // Added
                    )
                )
            );

        [CharacterBehavior("Oryx Brute")]
        public static State OryxBrute =>
            new(
                new State("swaggin",
                    new EntityWithinTransition(radius: 10, targetState: "piddle")
                ),
                new State("piddle",
                    new Wander((0.2f * 5.55f) + 0.74f), // Adjusted speed
                    new Follow((0.4f * 5.55f) + 0.74f, acquireRange: 10, distFromTarget: 1), // Adjusted speed
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 10,
                        count: 5,
                        shootAngle: 72,
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new Reproduce("Oryx Eye Warrior", densityRadius: 10, maxDensity: 4, cooldownMs: 500),
                    new TimedTransition(5000, "charge")
                ),
                new State("charge",
                    new Wander((0.3f * 5.55f) + 0.74f), // Adjusted speed
                    new Follow((1.2f * 5.55f) + 0.74f, acquireRange: 10, distFromTarget: 1), // Adjusted speed
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 10,
                        count: 5,
                        shootAngle: 72,
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 10,
                        count: 5,
                        shootAngle: 72,
                        cooldownMS: 750,
                        targeted: true // Added
                    ),
                    new Reproduce("Oryx Eye Warrior", densityRadius: 10, maxDensity: 4, cooldownMs: 500),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 3,
                        shootAngle: 10,
                        cooldownMS: 300,
                        targeted: true // Added
                    ),
                    new TimedTransition(4000, "piddle")
                )
            );

        [CharacterBehavior("Quiet Bomb")]
        public static State QuietBomb =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new State("Tex1",
                        new TimedTransition(250, "Tex2")
                    ),
                    new State("Tex2",
                        new SetAltTexture(1),
                        new TimedTransition(250, "Tex3")
                    ),
                    new State("Tex3",
                        new SetAltTexture(0),
                        new TimedTransition(250, "Tex4")
                    ),
                    new State("Tex4",
                        new SetAltTexture(1),
                        new TimedTransition(250, "Explode")
                    )
                ),
                new State("Explode",
                    new SetAltTexture(0),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 0,
                        count: 18,
                        shootAngle: 20,
                        fixedAngle: 0,
                        cooldownMS: 1000
                    ),
                    new Suicide()
                )
            );
    }
}