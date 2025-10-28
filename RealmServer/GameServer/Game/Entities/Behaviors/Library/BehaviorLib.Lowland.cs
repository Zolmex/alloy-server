#region

using Common;
using Common.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;
using GameServer.Game.Entities.Loot;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public partial class BehaviorLib
    {
        [CharacterBehavior("Hobbit Mage")]
        public static State HobbitMage =>
            new(
                new CharacterLoot(
                    // new TierLoot(2, ItemType.Weapon, 0.3),
                    // new TierLoot(2, ItemType.Armor, 0.3),
                    // new TierLoot(1, ItemType.Ring, 0.11),
                    // new TierLoot(1, ItemType.Ability, 0.39),
                    new ItemLoot("Health Potion", 0.02f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new State("idle",
                    new EntityWithinTransition("ring1", radius: 12)
                ),
                new State("ring1",
                    new Shoot(1, new LinePath(5f), targeted: false, projName: "Dark Blue Magic", damage: 10,
                        lifetimeMs: 340, fixedAngle: 0, count: 15, shootAngle: 24, cooldownMS: 1200),
                    new TimedTransition(400, "ring2")
                ),
                new State("ring2",
                    new Shoot(1, new LinePath(5f), targeted: false, projName: "Blue Magic", damage: 10,
                        lifetimeMs: 340, fixedAngle: 8, count: 15, shootAngle: 24, cooldownMS: 1200),
                    new TimedTransition(400, "ring3")
                ),
                new State("ring3",
                    new Shoot(1, new LinePath(5f), targeted: false, projName: "Cyan Magic", damage: 10,
                        lifetimeMs: 340, fixedAngle: 16, count: 15, shootAngle: 24, cooldownMS: 1200),
                    new TimedTransition(400, "idle")
                ),
                new Follow(4.91f, 6),
                new Wander(2.94f),
                new Spawn("Hobbit Archer", 0, 0, maxSpawnsPerReset: 4, cooldownMs: 12000),
                new Spawn("Hobbit Rogue", 0, 0, maxSpawnsPerReset: 3, cooldownMs: 6000)
            );

        [CharacterBehavior("Hobbit Archer")]
        public static State HobbitArcher =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(10, new LinePath(6f), targeted: true, projName: "Green Arrow", damage: 8,
                    lifetimeMs: 2000, cooldownMS: 1000),
                new State("run1",
                    new Protect(6.85f, "Hobbit Mage", 12, 10,
                        1),
                    new Wander(2.94f),
                    new TimedTransition(400, "run2")
                ),
                new State("run2",
                    new StayAwayFrom(5.14f, 4),
                    new Wander(2.94f),
                    new TimedTransition(600, "run3")
                ),
                new State("run3",
                    new Protect(6.29f, "Hobbit Archer", 16, 2,
                        2),
                    new Wander(2.94f),
                    new TimedTransition(400, "run1")
                )
            );

        [CharacterBehavior("Hobbit Rogue")]
        public static State HobbitRogue =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(3, new LinePath(6f), targeted: true, projName: "Blade", damage: 13,
                    lifetimeMs: 800, cooldownMS: 1000),
                new Protect(7.38f, "Hobbit Mage", 15, 9,
                    2.5f),
                new Follow(5.46f, 1),
                new Wander(2.94f)
            );

        [CharacterBehavior("Undead Hobbit Mage")]
        public static State UndeadHobbitMage =>
            new(
                new CharacterLoot(
                    // new TierLoot(3, ItemType.Weapon, 0.3),
                    // new TierLoot(3, ItemType.Armor, 0.3),
                    // new TierLoot(1, ItemType.Ring, 0.12),
                    // new TierLoot(1, ItemType.Ability, 0.39),
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Shoot(10, new LinePath(6f), targeted: true, projName: "Salmon Missile", damage: 20,
                    lifetimeMs: 1100, cooldownMS: 1000),
                new State("idle",
                    new EntityWithinTransition("ring1", radius: 12)
                ),
                new State("ring1",
                    new Shoot(1, new LinePath(5f), targeted: false, projName: "Yellow Missile", damage: 13,
                        lifetimeMs: 340, fixedAngle: 0, count: 15, shootAngle: 24, cooldownMS: 1200),
                    new TimedTransition(400, "ring2")
                ),
                new State("ring2",
                    new Shoot(1, new LinePath(5f), targeted: false, projName: "Fire Missile", damage: 13,
                        lifetimeMs: 340, fixedAngle: 8, count: 15, shootAngle: 24, cooldownMS: 1200),
                    new TimedTransition(400, "ring3")
                ),
                new State("ring3",
                    new Shoot(1, new LinePath(5f), targeted: false, projName: "Violet Missile", damage: 13,
                        lifetimeMs: 340, fixedAngle: 16, count: 15, shootAngle: 24, cooldownMS: 1200),
                    new TimedTransition(400, "idle")
                ),
                new Follow(4.91f, 6),
                new Wander(2.94f),
                new Spawn("Undead Hobbit Archer", 0, 0, maxSpawnsPerReset: 4, cooldownMs: 12000),
                new Spawn("Undead Hobbit Rogue", 0, 0, maxSpawnsPerReset: 3, cooldownMs: 6000)
            );

        [CharacterBehavior("Undead Hobbit Archer")]
        public static State UndeadHobbitArcher =>
            new(
                new CharacterLoot(
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Shoot(10, new LinePath(6f), targeted: true, projName: "Green Arrow", damage: 12,
                    lifetimeMs: 2000, cooldownMS: 1000),
                new State("run1",
                    new Protect(6.85f, "Undead Hobbit Mage", 12, 10,
                        1),
                    new Wander(2.94f),
                    new TimedTransition(400, "run2")
                ),
                new State("run2",
                    new StayAwayFrom(5.14f, 4),
                    new Wander(2.94f),
                    new TimedTransition(600, "run3")
                ),
                new State("run3",
                    new Protect(6.29f, "Undead Hobbit Archer", 16, 2,
                        2),
                    new Wander(2.94f),
                    new TimedTransition(400, "run1")
                )
            );

        [CharacterBehavior("Undead Hobbit Rogue")]
        public static State UndeadHobbitRogue =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(3, new LinePath(6f), targeted: true, projName: "Blade", damage: 16,
                    lifetimeMs: 800, cooldownMS: 1000),
                new Protect(7.38f, "Undead Hobbit Mage", 15, 9,
                    2.5f),
                new Follow(5.46f, 1),
                new Wander(2.94f)
            );

        [CharacterBehavior("Sumo Master")]
        public static State SumoMaster =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.05f),
                    new ItemLoot("Magic Potion", 0.05f)
                ),
                new State("sleeping1",
                    new SetAltTexture(0),
                    new TimedTransition(1000, "sleeping2"),
                    new HpLessTransition(0.99f, "hurt")
                ),
                new State("sleeping2",
                    new SetAltTexture(3),
                    new TimedTransition(1000, "sleeping1"),
                    new HpLessTransition(0.99f, "hurt")
                ),
                new State("hurt",
                    new SetAltTexture(2),
                    new Spawn("Lil Sumo", 0, 0, 200),
                    new TimedTransition(1000, "awake")
                ),
                new State("awake",
                    new SetAltTexture(1),
                    new Shoot(3, new LinePath(7f), targeted: true, projName: "Red BigBullet", damage: 15,
                        lifetimeMs: 500, cooldownMS: 250, size: 120),
                    new Follow(1.015f, 1),
                    new Wander(1.015f),
                    new HpLessTransition(0.5f, "rage")
                ),
                new State("rage",
                    new SetAltTexture(4),
                    new Taunt("Engaging Super-Mode!!!"),
                    new Follow(4.07f, 1),
                    new Wander(4.07f),
                    new State("shoot",
                        new Shoot(8, new LinePath(10f), targeted: true, projName: "Gold Bullet", damage: 25,
                            lifetimeMs: 1000, cooldownMS: 150, size: 150, multiHit: true, passesCover: true),
                        new TimedTransition(700, "rest")
                    ),
                    new State("rest",
                        new TimedTransition(400, "shoot")
                    )
                )
            );

        [CharacterBehavior("Lil Sumo")]
        public static State LilSumo =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.02f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new Shoot(8, new LinePath(5f), targeted: true, projName: "Red Bullet", damage: 10,
                    lifetimeMs: 3000, cooldownMS: 1000, size: 50),
                new Orbit(2.94f, 2, target: "Sumo Master"),
                new Wander(2.94f)
            );

        [CharacterBehavior("Elf Wizard")]
        public static State ElfWizard =>
            new(
                new CharacterLoot(
                    // new TierLoot(2, ItemType.Weapon, 0.36),
                    // new TierLoot(2, ItemType.Armor, 0.36),
                    // new TierLoot(1, ItemType.Ring, 0.11),
                    // new TierLoot(1, ItemType.Ability, 0.39),
                    new ItemLoot("Health Potion", 0.02f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new State("idle",
                    new Wander(2.94f),
                    new EntityWithinTransition("move1", radius: 11)
                ),
                new State("move1",
                    new Shoot(10, new LinePath(7f), targeted: true, projName: "Blue Bolt", damage: 5,
                        lifetimeMs: 2000, count: 3, shootAngle: 14, predictive: 0.3f, cooldownMS: 1000, size: 90),
                    new BackAndForth(5.18f),
                    new TimedTransition(2000, "move2")
                ),
                new State("move2",
                    new Shoot(10, new LinePath(7f), targeted: true, projName: "Blue Bolt", damage: 5,
                        lifetimeMs: 2000, count: 3, shootAngle: 10, predictive: 0.5f, cooldownMS: 1000, size: 90),
                    new Follow(4.07f, acquireRange: 10.5f, distFromTarget: 3),
                    new Wander(2.94f),
                    new TimedTransition(2000, "move3")
                ),
                new State("move3",
                    new StayAwayFrom(4.07f, 5),
                    new Wander(2.94f),
                    new TimedTransition(2000, "idle")
                ),
                new Spawn("Elf Archer", 0, 0, maxSpawnsPerReset: 2, cooldownMs: 15000),
                new Spawn("Elf Swordsman", 0, 0, maxSpawnsPerReset: 4, cooldownMs: 7000),
                new Spawn("Elf Mage", 0, 0, maxSpawnsPerReset: 1, cooldownMs: 8000)
            );

        [CharacterBehavior("Elf Archer")]
        public static State ElfArcher =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(10, new LinePath(6.5f), targeted: true, projName: "Green Arrow", damage: 9,
                    lifetimeMs: 2000, predictive: 1, cooldownMS: 1000),
                new Orbit(3.47f, 3, speedVariance: 0.1f, radiusVariance: 0.5f),
                new Protect(7.38f, "Elf Wizard", 30, 10,
                    1),
                new Wander(2.94f)
            );

        [CharacterBehavior("Elf Swordsman")]
        public static State ElfSwordsman =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(10, new LinePath(10f), targeted: true, projName: "Blade", damage: 13,
                    lifetimeMs: 400, predictive: 1, cooldownMS: 1000),
                new Protect(7.38f, "Elf Wizard", 15, 10,
                    5),
                new Buzz(6.29f, 1),
                new Orbit(4.07f, 3, speedVariance: 0.1f, radiusVariance: 0.5f),
                new Wander(2.94f)
            );

        [CharacterBehavior("Elf Mage")]
        public static State ElfMage =>
            new(
                new CharacterLoot(
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Cyan Magic", damage: 4,
                    lifetimeMs: 1500, cooldownMS: 300),
                new Orbit(3.47f, 3),
                new Protect(7.38f, "Elf Wizard", 30, 10,
                    1),
                new Wander(2.94f)
            );

        [CharacterBehavior("Goblin Rogue")]
        public static State GoblinRogue =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new State("protect",
                    new Protect(5.18f, "Goblin Mage", 12, 1.5f,
                        1.5f),
                    new TimedTransition(1200, "scatter")
                ),
                new State("scatter",
                    new Orbit(5.18f, 7, target: "Goblin Mage", radiusVariance: 1),
                    new TimedTransition(2400, "protect")
                ),
                new Shoot(3, new LinePath(6f), targeted: true, projName: "Blade", damage: 10,
                    lifetimeMs: 800, cooldownMS: 1000),
                new State("help",
                    new Protect(5.18f, "Goblin Mage", 12, 6,
                        3),
                    new Follow(5.18f, acquireRange: 10.5f, distFromTarget: 1.5f),
                    new EntityNotWithinTransition("Goblin Mage", 15, targetStates: "protect")
                )
            );

        [CharacterBehavior("Goblin Warrior")]
        public static State GoblinWarrior =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new State("protect",
                    new Protect(5.18f, "Goblin Mage", 12, 1.5f,
                        1.5f),
                    new TimedTransition(1200, "scatter")
                ),
                new State("scatter",
                    new Orbit(5.18f, 7, target: "Goblin Mage", radiusVariance: 1),
                    new TimedTransition(2400, "protect")
                ),
                new Shoot(3, new LinePath(6f), targeted: true, projName: "Blade", damage: 12,
                    lifetimeMs: 600, cooldownMS: 1000),
                new State("help",
                    new Protect(5.18f, "Goblin Mage", 12, 6,
                        3),
                    new Follow(5.18f, acquireRange: 10.5f, distFromTarget: 1.5f),
                    new EntityNotWithinTransition("Goblin Mage", 15, targetStates: "protect")
                ),
                new DropPortalOnDeath("Pirate Cave Portal", 0.01f)
            );

        [CharacterBehavior("Goblin Mage")]
        public static State GoblinMage =>
            new(
                new CharacterLoot(
                    // new TierLoot(3, ItemType.Weapon, 0.3),
                    // new TierLoot(3, ItemType.Armor, 0.3),
                    // new TierLoot(1, ItemType.Ring, 0.09),
                    // new TierLoot(1, ItemType.Ability, 0.38),
                    new ItemLoot("Health Potion", 0.02f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new State("unharmed",
                    new Shoot(8, new LinePath(3.5f), targeted: true, projName: "Cyan Magic", damage: 14,
                        lifetimeMs: 3000, predictive: 0.35f, cooldownMS: 1000, size: 90),
                    new Shoot(8, new LinePath(7.5f), targeted: true, projName: "Dark Blue Magic", damage: 16,
                        lifetimeMs: 1600, predictive: 0.35f, cooldownMS: 1300, size: 130),
                    new Follow(3.47f, acquireRange: 10.5f, distFromTarget: 4),
                    new Wander(2.94f),
                    new HpLessTransition(0.65f, "activate_horde")
                ),
                new State("activate_horde",
                    new Shoot(8, new LinePath(3.5f), targeted: true, projName: "Cyan Magic", damage: 14,
                        lifetimeMs: 3000, predictive: 0.25f, cooldownMS: 1000, size: 90),
                    new Shoot(8, new LinePath(7.5f), targeted: true, projName: "Dark Blue Magic", damage: 16,
                        lifetimeMs: 1600, predictive: 0.25f, cooldownMS: 1000, size: 130),
                    new Flash(0x484848, 0.6f, 5000),
                    new Order(12, "Goblin Rogue", "help"),
                    new Order(12, "Goblin Warrior", "help"),
                    new StayAwayFrom(3.47f, 6)
                ),
                new Spawn("Goblin Rogue", 0, 0, maxSpawnsPerReset: 7, cooldownMs: 12000),
                new Spawn("Goblin Warrior", 0, 0, maxSpawnsPerReset: 7, cooldownMs: 12000)
            );

        [CharacterBehavior("Easily Enraged Bunny")]
        public static State EasilyEnragedBunny =>
            new(
                new Follow(4.635f, acquireRange: 9.5f, distFromTarget: 1),
                new TransformOnDeath("Enraged Bunny")
            );

        [CharacterBehavior("Enraged Bunny")]
        public static State EnragedBunny =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.01f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new Shoot(9, new LinePath(7f), targeted: true, projName: "Fire Missile", damage: 12,
                    lifetimeMs: 1600, predictive: 0.5f, cooldownMS: 400, size: 90),
                new State("red",
                    new Flash(0xff0000, 1.5f, 1),
                    new TimedTransition(1600, "yellow")
                ),
                new State("yellow",
                    new Flash(0xffff33, 1.5f, 1),
                    new TimedTransition(1600, "orange")
                ),
                new State("orange",
                    new Flash(0xff9900, 1.5f, 1),
                    new TimedTransition(1600, "red")
                ),
                new Follow(5.435f, acquireRange: 9, distFromTarget: 2.5f),
                new Wander(5.435f)
            );

        [CharacterBehavior("Forest Nymph")]
        public static State ForestNymph =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new State("circle",
                    new Shoot(4, new LinePath(5.5f), targeted: true, projName: "Purple Boomerang", damage: 14,
                        lifetimeMs: 2900, count: 1, predictive: 0.1f, cooldownMS: 900, size: 70),
                    new Follow(5.735f, acquireRange: 11, distFromTarget: 3.5f, cooldownOffsetMS: 1000,
                        cooldownMS: 5000),
                    new Orbit(7.965f, 3.5f, 12),
                    new Wander(4.585f),
                    new TimedTransition(4000, "dart_away")
                ),
                new State("dart_away",
                    new Shoot(9, new LinePath(5f), targeted: true, projName: "Blue Boomerang", damage: 10,
                        lifetimeMs: 2900, count: 6, fixedAngle: 20, shootAngle: 60, cooldownMS: 1400, size: 70),
                    new Wander(2.94f),
                    new TimedTransition(3600, "circle")
                ),
                new DropPortalOnDeath("Pirate Cave Portal", 0.01f)
            );

        [CharacterBehavior("Sandsman King")]
        public static State SandsmanKing =>
            new(
                new CharacterLoot(
                    // new TierLoot(3, ItemType.Weapon, 0.3),
                    // new TierLoot(3, ItemType.Armor, 0.3),
                    // new TierLoot(1, ItemType.Ring, 0.11),
                    // new TierLoot(1, ItemType.Ability, 0.39),
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(10, new LinePath(7f), targeted: true, projName: "Blade", damage: 15,
                    lifetimeMs: 1200, cooldownMS: 10000),
                new Follow(4.07f, 4),
                new Wander(2.94f),
                new Spawn("Sandsman Archer", 0, 0, maxSpawnsPerReset: 2, cooldownMs: 10000),
                new Spawn("Sandsman Sorcerer", 0, 0, maxSpawnsPerReset: 3, cooldownMs: 8000)
            );

        [CharacterBehavior("Sandsman Sorcerer")]
        public static State SandsmanSorcerer =>
            new(
                new CharacterLoot(
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Shoot(10, new LinePath(0.8f), targeted: true, projName: "Purple Mystic Shot", damage: 13,
                    lifetimeMs: 10000, cooldownMS: 5000),
                new Shoot(5, new LinePath(8f), targeted: true, projName: "Dark Blue Magic", damage: 17,
                    lifetimeMs: 300, cooldownMS: 400),
                new Protect(7.38f, "Sandsman King", 15, 6,
                    5),
                new Wander(2.94f)
            );

        [CharacterBehavior("Sandsman Archer")]
        public static State SandsmanArcher =>
            new(
                new CharacterLoot(
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Shoot(10, new LinePath(8.5f), targeted: true, projName: "Green Arrow", damage: 8,
                    lifetimeMs: 1400, predictive: 0.5f, cooldownMS: 1000),
                new Orbit(4.15f, 3.25f, 15, "Sandsman King", radiusVariance: 0.5f),
                new Wander(2.94f)
            );

        [CharacterBehavior("Giant Crab")]
        public static State GiantCrab =>
            new(
                new CharacterLoot(
                    // new TierLoot(2, ItemType.Weapon, 0.14),
                    // new TierLoot(2, ItemType.Armor, 0.19),
                    // new TierLoot(1, ItemType.Ring, 0.05),
                    // new TierLoot(1, ItemType.Ability, 0.28),
                    new ItemLoot("Health Potion", 0.02f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new State("idle",
                    new Wander(4.07f),
                    new EntityWithinTransition("scuttle", radius: 11)
                ),
                new State("scuttle",
                    new Shoot(9, new LinePath(2f), targeted: true, projName: "Beam", damage: 1,
                        lifetimeMs: 200, cooldownMS: 1000, size: 60),
                    new Shoot(9, new LinePath(4f), targeted: true, projName: "Beam", damage: 4,
                        lifetimeMs: 400, cooldownMS: 1000),
                    new Shoot(9, new LinePath(6f), targeted: true, projName: "Beam", damage: 7,
                        lifetimeMs: 600, cooldownMS: 1000, size: 140),
                    new Shoot(9, new LinePath(8f), targeted: true, projName: "Beam", damage: 11,
                        lifetimeMs: 800, cooldownMS: 1000, size: 180),
                    new State("move1",
                        new Follow(6.29f, acquireRange: 10.6f, distFromTarget: 2),
                        new Wander(4.07f),
                        new TimedTransition(400, "pause1")
                    ),
                    new State("pause1",
                        new TimedTransition(200, "move1")
                    ),
                    new TimedTransition(4700, "tri-spit")
                ),
                new State("tri-spit",
                    new Shoot(9, new LinePath(7f), targeted: true, projName: "Blue Bolt", damage: 10,
                        lifetimeMs: 1800, predictive: 0.5f, coolDownOffset: 1200, cooldownMS: 90000, size: 90),
                    new Shoot(9, new LinePath(7f), targeted: true, projName: "Blue Bolt", damage: 10,
                        lifetimeMs: 1800, predictive: 0.5f, coolDownOffset: 1800, cooldownMS: 90000, size: 90),
                    new Shoot(9, new LinePath(7f), targeted: true, projName: "Blue Bolt", damage: 10,
                        lifetimeMs: 1800, predictive: 0.5f, coolDownOffset: 2400, cooldownMS: 90000, size: 90),
                    new State("move2",
                        new Follow(6.29f, acquireRange: 10.6f, distFromTarget: 2),
                        new Wander(4.07f),
                        new TimedTransition(400, "pause2")
                    ),
                    new State("pause2",
                        new TimedTransition(200, "move2")
                    ),
                    new TimedTransition(3200, "idle")
                ),
                new DropPortalOnDeath("Pirate Cave Portal", 0.01f)
            );

        [CharacterBehavior("Sand Devil")]
        public static State SandDevil =>
            new(
                new State("wander",
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Dark Gray Spinner", damage: 10,
                        lifetimeMs: 1500, predictive: 0.3f, cooldownMS: 700, size: 68, effects: (ConditionEffectIndex.Confused, 1000)),
                    new Follow(4.63f, acquireRange: 10, distFromTarget: 2.2f),
                    new Wander(4.63f),
                    new TimedTransition(3000, "circle")
                ),
                new State("circle",
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Dark Gray Spinner", damage: 10,
                        lifetimeMs: 1500, predictive: 0.3f, coolDownOffset: 1000, cooldownMS: 1000, size: 68, effects: (ConditionEffectIndex.Confused, 1000)),
                    new Orbit(4.63f, 2, 9),
                    new TimedTransition(3100, "wander")
                ),
                new DropPortalOnDeath("Pirate Cave Portal", 0.01f)
            );
    }
}