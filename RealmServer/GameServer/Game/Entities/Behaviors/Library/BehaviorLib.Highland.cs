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
        [CharacterBehavior("Minotaur")]
        public static State Minotaur =>
            new(
                new CharacterLoot( // Threshold: 1%
                    // new TierLoot(5, ItemType.Weapon, 0.26),
                    // new TierLoot(6, ItemType.Weapon, 0.18),
                    // new TierLoot(7, ItemType.Weapon, 0.10),
                    // new TierLoot(5, ItemType.Armor, 0.26),
                    // new TierLoot(6, ItemType.Armor, 0.18),
                    // new TierLoot(7, ItemType.Armor, 0.10),
                    // new TierLoot(3, ItemType.Ring, 0.10),
                    // new TierLoot(3, ItemType.Ability, 0.2)
                ),
                new State("idle",
                    //new StayAbove(0.6, 160),
                    new EntityWithinTransition("charge", radius: 10)
                ),
                new State("charge",
                    new Follow(6, acquireRange: 11, distFromTarget: 1.6f),
                    new TimedTransition(200, "spam_blades")
                ),
                new State("spam_blades",
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Blade", damage: 52, lifetimeMs: 1200,
                        maxRadius: 8, count: 1, cooldownMS: 100000, coolDownOffset: 1000, multiHit: true,
                        passesCover: true),
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Blade", damage: 52, lifetimeMs: 1200,
                        maxRadius: 8, count: 2, shootAngle: 16, cooldownMS: 100000, coolDownOffset: 1200, multiHit: true,
                        passesCover: true),
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Blade", damage: 52, lifetimeMs: 1200,
                        maxRadius: 8, count: 3, predictive: 0.2f, cooldownMS: 100000, coolDownOffset: 1600, multiHit: true,
                        passesCover: true),
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Blade", damage: 52, lifetimeMs: 1200,
                        maxRadius: 8, count: 1, shootAngle: 24, cooldownMS: 100000, coolDownOffset: 2200, multiHit: true,
                        passesCover: true),
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Blade", damage: 52, lifetimeMs: 1200,
                        maxRadius: 8, count: 2, predictive: 0.2f, cooldownMS: 100000, coolDownOffset: 2800, multiHit: true,
                        passesCover: true),
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Blade", damage: 52, lifetimeMs: 1200,
                        maxRadius: 8, count: 3, shootAngle: 16, cooldownMS: 100000, coolDownOffset: 3200),
                    new Wander(4.07f),
                    new TimedTransition(4400, "blade_ring")
                ),
                new State("blade_ring",
                    new Shoot(path: new LinePath(9f), targeted: false, projName: "Blade", damage: 28, lifetimeMs: 430,
                        maxRadius: 7, fixedAngle: 0, count: 12, shootAngle: 30, cooldownMS: 800, coolDownOffset: 600,
                        multiHit: true, passesCover: true),
                    new Shoot(path: new LinePath(9f), targeted: false, projName: "Blade", damage: 28, lifetimeMs: 430,
                        maxRadius: 7, fixedAngle: 15, count: 6, shootAngle: 60, cooldownMS: 800, coolDownOffset: 1000,
                        multiHit: true, passesCover: true),
                    new Follow(4.07f, acquireRange: 10, distFromTarget: 1),
                    new Wander(4.07f),
                    new TimedTransition(3500, "pause")
                ),
                new State("pause",
                    new Wander(4.07f),
                    new TimedTransition(1000, "idle")
                )
            );

        [CharacterBehavior("Ogre King")]
        public static State OgreKing =>
            new(
                new CharacterLoot( // Threshold: 1%
                    // new TierLoot(4, ItemType.Weapon, 0.2),
                    // new TierLoot(5, ItemType.Weapon, 0.1),
                    // new TierLoot(4, ItemType.Armor, 0.2),
                    // new TierLoot(5, ItemType.Armor, 0.15),
                    // new TierLoot(6, ItemType.Armor, 0.10),
                    // new TierLoot(2, ItemType.Ring, 0.15),
                    // new TierLoot(2, ItemType.Ability, 0.18)
                ),
                new Spawn("Ogre Warrior", 0, 0, maxSpawnsPerReset: 4, cooldownMs: 12000),
                new Spawn("Ogre Mage", 0, 0, maxSpawnsPerReset: 2, cooldownMs: 16000),
                new Spawn("Ogre Wizard", 0, 0, maxSpawnsPerReset: 2, cooldownMs: 20000),
                new State("idle",
                    new Wander(2.4f),
                    new EntityWithinTransition("grenade_blade_combo", radius: 10)
                ),
                new State("grenade_blade_combo",
                    new State("grenade1",
                        new AOE(3, 60, 100000),
                        new Wander(2.4f),
                        new TimedTransition(2000, "grenade2")
                    ),
                    new State("grenade2",
                        new AOE(3, 60, 100000),
                        new Wander(3.51f),
                        new TimedTransition(3000, "slow_follow")
                    ),
                    new State("slow_follow",
                        new Shoot(path: new LinePath(10f), targeted: true, projName: "Blade", damage: 50,
                            lifetimeMs: 400, maxRadius: 13, cooldownMS: 1000),
                        new Follow(2.96f, acquireRange: 9, distFromTarget: 3.5f),
                        new Wander(2.96f),
                        new TimedTransition(4000, "grenade1")
                    ),
                    new HpLessTransition(0.45f, "furious")
                ),
                new State("furious",
                    new AOE(2.4f, 55, range: 9, cooldownMs: 1500),
                    new Wander(4.07f),
                    new TimedTransition(12000, "idle")
                )
            );

        [CharacterBehavior("Ogre Warrior")]
        public static State OgreWarrior =>
            new(
                new Shoot(path: new LinePath(10f), targeted: true, projName: "Blade", damage: 45, lifetimeMs: 400,
                    maxRadius: 3, predictive: 0.5f, cooldownMS: 1000),
                new Protect(1.2f, "Ogre King", 15, 10, 5),
                new Follow(1.4f, acquireRange: 10.5f, distFromTarget: 1.6f, cooldownOffsetMS: 2600, cooldownMS: 2200),
                new Orbit(4.07f, 6),
                new Wander(2.96f)
            );

        [CharacterBehavior("Ogre Mage")]
        public static State OgreMage =>
            new(
                new Shoot(path: new LinePath(6.7f), targeted: true, projName: "Green Magic", damage: 38,
                    lifetimeMs: 1500, maxRadius: 10, predictive: 0.3f, cooldownMS: 1000),
                new Protect(1.2f, "Ogre King", 30, 10, 1),
                new Orbit(3.5f, 6),
                new Wander(2.96f)
            );

        [CharacterBehavior("Ogre Wizard")]
        public static State OgreWizard =>
            new(
                new Shoot(path: new LinePath(5f), targeted: true, projName: "Green Bolt", damage: 40, lifetimeMs: 2200,
                    maxRadius: 10, cooldownMS: 300),
                new Protect(1.2f, "Ogre King", 30, 10, 1),
                new Orbit(3.5f, 6),
                new Wander(2.96f)
            );

        [CharacterBehavior("Lizard God")]
        public static State LizardGod =>
            new(
                new CharacterLoot( // Threshold 1%
                    // new TierLoot(5, ItemType.Weapon, 0.28),
                    // new TierLoot(6, ItemType.Weapon, 0.20),
                    // new TierLoot(7, ItemType.Weapon, 0.10),
                    // new TierLoot(5, ItemType.Armor, 0.26),
                    // new TierLoot(6, ItemType.Armor, 0.18),
                    // new TierLoot(7, ItemType.Armor, 0.10),
                    // new TierLoot(3, ItemType.Ring, 0.15),
                    // new TierLoot(3, ItemType.Ability, 0.15)
                ),
                new Spawn("Night Elf Archer", 0, 0, maxSpawnsPerReset: 4),
                new Spawn("Night Elf Warrior", 0, 0, maxSpawnsPerReset: 3),
                new Spawn("Night Elf Mage", 0, 0, maxSpawnsPerReset: 2),
                new Spawn("Night Elf Veteran", 0, 0, maxSpawnsPerReset: 2),
                new Spawn("Night Elf King", 0, 0, maxSpawnsPerReset: 1),
                new Follow(6.4f, 7),
                new Wander(2.96f),
                new State("idle",
                    new EntityWithinTransition("normal_attack", radius: 10.2f)
                ),
                new State("normal_attack",
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Green Bolt", damage: 30,
                        lifetimeMs: 1600, maxRadius: 10, count: 3, shootAngle: 3, predictive: 0.5f, cooldownMS: 1000),
                    new TimedTransition(4000, "if_cloaked")
                ),
                new State("if_cloaked",
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Green Bolt", damage: 30,
                        lifetimeMs: 1600, maxRadius: 10, count: 8, shootAngle: 45, fixedAngle: 20, cooldownMS: 1600,
                        coolDownOffset: 400),
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Green Bolt", damage: 30,
                        lifetimeMs: 1600, maxRadius: 10, count: 8, shootAngle: 45, fixedAngle: 42, cooldownMS: 1600,
                        coolDownOffset: 1200),
                    new EntityWithinTransition("normal_attack", radius: 10)
                )
            );

        [CharacterBehavior("Night Elf Archer")]
        public static State NightElfArcher =>
            new(
                new Shoot(path: new LinePath(7.5f), targeted: true, projName: "Blue Arrow", damage: 36,
                    lifetimeMs: 2000, maxRadius: 10, predictive: 1f, cooldownMS: 1000),
                new Follow(9f, 7),
                new Wander(2.96f)
            );

        [CharacterBehavior("Night Elf Warrior")]
        public static State NightElfWarrior =>
            new(
                new Shoot(path: new LinePath(10f), targeted: true, projName: "Blade", damage: 36, lifetimeMs: 800,
                    maxRadius: 3, predictive: 1f, cooldownMS: 1000),
                new Follow(9f, 1),
                new Wander(2.96f)
            );

        [CharacterBehavior("Night Elf Mage")]
        public static State NightElfMage =>
            new(
                new Shoot(path: new LinePath(6f), targeted: true, projName: "Blue Magic", damage: 40, lifetimeMs: 2000,
                    maxRadius: 10, predictive: 1f, cooldownMS: 1000, multiHit: true),
                new Follow(9f, 7),
                new Wander(2.96f)
            );

        [CharacterBehavior("Night Elf Veteran")]
        public static State NightElfVeteran =>
            new(
                new Shoot(path: new LinePath(8f), targeted: true, projName: "Gold Arrow", damage: 45, lifetimeMs: 2000,
                    maxRadius: 10, predictive: 1f, cooldownMS: 1000, size: 120, multiHit: true),
                new Follow(9f, 7),
                new Wander(2.96f)
            );

        [CharacterBehavior("Night Elf King")]
        public static State NightElfKing =>
            new(
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Green Magic", damage: 55, lifetimeMs: 1200,
                    maxRadius: 10, predictive: 1f, cooldownMS: 1000),
                new Follow(9f, 7),
                new Wander(2.96f)
            );

        [CharacterBehavior("Undead Dwarf God")]
        public static State UndeadDwarfGod =>
            new(
                new CharacterLoot(
                    // new TierLoot(5, ItemType.Weapon, 0.26),
                    // new TierLoot(6, ItemType.Weapon, 0.18),
                    // new TierLoot(7, ItemType.Weapon, 0.1),
                    // new TierLoot(5, ItemType.Armor, 0.26),
                    // new TierLoot(6, ItemType.Armor, 0.18),
                    // new TierLoot(7, ItemType.Armor, 0.1),
                    // new TierLoot(3, ItemType.Ring, 0.1),
                    // new TierLoot(3, ItemType.Ability, 0.2)
                ),
                new Spawn("Undead Dwarf Warrior", 0, 0, maxSpawnsPerReset: 3),
                new Spawn("Undead Dwarf Axebearer", 0, 0, maxSpawnsPerReset: 3),
                new Spawn("Undead Dwarf Mage", 0, 0, maxSpawnsPerReset: 3),
                new Spawn("Undead Dwarf King", 0, 0, maxSpawnsPerReset: 2),
                new Spawn("Soulless Dwarf", 0, 0, maxSpawnsPerReset: 1),
                new Follow(6.3f, 7),
                new Wander(2.96f),
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Pink Bolt", damage: 30, lifetimeMs: 1700,
                    maxRadius: 10, count: 3, shootAngle: 15, cooldownMS: 1000),
                new Shoot(path: new LinePath(6f), targeted: true, projName: "Blue Star", damage: 0, lifetimeMs: 2200,
                    maxRadius: 10, predictive: 0.5f, cooldownMS: 1200, effects: (ConditionEffectIndex.Confused, 3000))
            );

        [CharacterBehavior("Undead Dwarf Warrior")]
        public static State UndeadDwarfWarrior =>
            new(
                new Shoot(path: new LinePath(6f), targeted: true, projName: "Blade", damage: 40, lifetimeMs: 1200,
                    maxRadius: 3, cooldownMS: 1000),
                new Follow(6.3f, 1),
                new Wander(2.96f)
            );

        [CharacterBehavior("Undead Dwarf Axebearer")]
        public static State UndeadDwarfAxebearer =>
            new(
                new Shoot(path: new LinePath(12f), targeted: true, projName: "Blade", damage: 46, lifetimeMs: 400,
                    maxRadius: 3, cooldownMS: 1000),
                new Follow(6.3f, 1),
                new Wander(2.96f)
            );

        [CharacterBehavior("Undead Dwarf Mage")]
        public static State UndeadDwarfMage =>
            new(
                new State("circle_player",
                    new Shoot(path: new LinePath(5), targeted: true, projName: "Blue Bolt", damage: 32,
                        lifetimeMs: 2400, maxRadius: 8, predictive: 0.3f, cooldownMS: 1000, coolDownOffset: 500, multiHit: true),
                    new Protect(4.62f, "Undead Dwarf King", 11, 10, 3),
                    new Orbit(4.62f, 3.5f, 11),
                    new Wander(4.62f),
                    new TimedTransition(3500, "circle_king")
                ),
                new State("circle_king",
                    new Shoot(path: new LinePath(5), targeted: true, projName: "Blue Bolt", damage: 32,
                        lifetimeMs: 2400, maxRadius: 8, count: 5, shootAngle: 72, fixedAngle: 20, predictive: 0.3f,
                        cooldownMS: 1600, coolDownOffset: 500, multiHit: true),
                    new Shoot(path: new LinePath(5), targeted: true, projName: "Blue Bolt", damage: 32,
                        lifetimeMs: 2400, maxRadius: 8, count: 5, shootAngle: 72, fixedAngle: 33, predictive: 0.3f,
                        cooldownMS: 1600, coolDownOffset: 1300, multiHit: true),
                    new Orbit(7.4f, 2.5f, target: "Undead Dwarf King", acquireRange: 12, radiusVariance: 0.1f,
                        speedVariance: 0.1f),
                    new Wander(4.62f),
                    new TimedTransition(3500, "circle_player")
                )
            );

        [CharacterBehavior("Undead Dwarf King")]
        public static State UndeadDwarfKing =>
            new(
                new Shoot(path: new LinePath(10f), targeted: true, projName: "Blade", damage: 52, lifetimeMs: 600,
                    maxRadius: 3, cooldownMS: 1000),
                new Follow(5.1f, 1.4f),
                new Wander(2.96f)
            );

        [CharacterBehavior("Soulless Dwarf")]
        public static State SoullessDwarf =>
            new(
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Blue Magic", damage: 55, lifetimeMs: 1700,
                    maxRadius: 10, cooldownMS: 1000),
                new State("idle",
                    new EntityWithinTransition("run1", 10.5f)
                ),
                new State("run1",
                    new Protect(6.84f, "Undead Dwarf God", 16, 10, 1),
                    new Wander(2.96f),
                    new TimedTransition(2000, "run2")
                ),
                new State("run2",
                    new StayAwayFrom(5.1f, 4),
                    new Wander(2.96f),
                    new TimedTransition(1400, "run3")
                ),
                new State("run3",
                    new Protect(6.3f, "Undead Dwarf King", 16, 2, 2),
                    new Protect(6.3f, "Undead Dwarf Axebearer", 16, 2,
                        2),
                    new Protect(6.3f, "Undead Dwarf Warrior", 16, 2, 2),
                    new Wander(2.96f),
                    new TimedTransition(2000, "idle")
                )
            );

        [CharacterBehavior("Flayer God")]
        public static State FlayerGod =>
            new(
                new CharacterLoot( // Threshold 1%
                    // new TierLoot(5, ItemType.Weapon, 0.26),
                    // new TierLoot(6, ItemType.Weapon, 0.18),
                    // new TierLoot(7, ItemType.Weapon, 0.1),
                    // new TierLoot(5, ItemType.Armor, 0.26),
                    // new TierLoot(6, ItemType.Armor, 0.18),
                    // new TierLoot(7, ItemType.Armor, 0.1),
                    // new TierLoot(3, ItemType.Ring, 0.15),
                    // new TierLoot(3, ItemType.Ability, 0.15)
                ),
                new Spawn("Flayer", 0, 0, maxSpawnsPerReset: 2),
                new Spawn("Flayer Veteran", 0, 0, maxSpawnsPerReset: 3),
                new Spawn("Flayer God", 0, 0, maxSpawnsPerReset: 2, maxDensity: 2, cooldownMs: 60000),
                new Follow(6.3f, 7),
                new Wander(2.96f),
                new Shoot(path: new LinePath(5f), targeted: true, projName: "Green Magic", damage: 60, lifetimeMs: 2200,
                    maxRadius: 10, predictive: 0.5f, cooldownMS: 400),
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Grey Star", damage: 0, lifetimeMs: 2200,
                    maxRadius: 10, predictive: 1f, cooldownMS: 1000, effects: (ConditionEffectIndex.Quiet, 10000))
            );

        [CharacterBehavior("Flayer")]
        public static State Flayer =>
            new(
                new Shoot(path: new LinePath(5f), targeted: true, projName: "Blue Magic", damage: 40, lifetimeMs: 2700,
                    maxRadius: 10, predictive: 0.5f, cooldownMS: 1000),
                new Follow(7.4f, 7),
                new Wander(2.96f)
            );

        [CharacterBehavior("Flayer Veteran")]
        public static State FlayerVeteran =>
            new(
                new Shoot(path: new LinePath(8f), targeted: true, projName: "Purple Magic", damage: 50,
                    lifetimeMs: 1500, maxRadius: 10, predictive: 0.5f, cooldownMS: 1000),
                new Follow(7.4f, 7),
                new Wander(2.96f)
            );

        [CharacterBehavior("Flamer King")]
        public static State FlamerKing =>
            new(
                new Spawn("Flamer", 0, 0, maxSpawnsPerReset: 5, cooldownMs: 10000),
                new State("Attacking",
                    new State("Charge",
                        new Follow(4.62f, 0.1f),
                        new EntityWithinTransition("Bullet", radius: 2)
                    ),
                    new State("Bullet",
                        new Flash(0xffaa00, 0.2, 20),
                        new ChangeSize(20, 140),
                        new Shoot(path: new AmplitudePath(8f, 0.6f, 0.5f, lifetimeMs: 500), targeted: true, projName: "White Flame",
                            damage: 29, lifetimeMs: 500, maxRadius: 8, cooldownMS: 200, size: 105, multiHit: true),
                        new TimedTransition(4000, "Wait")
                    ),
                    new State("Wait",
                        new ChangeSize(-20, 80),
                        new TimedTransition(500, "Charge")
                    ),
                    new HpLessTransition(0.2f, "FlashBeforeExplode")
                ),
                new State("FlashBeforeExplode",
                    new Flash(0xff0000, 1, 1),
                    new TimedTransition(300, "Explode")
                ),
                new State("Explode",
                    new Shoot(path: new AmplitudePath(8f, 0.6f, 0.5f, lifetimeMs: 500), targeted: false, projName: "White Flame",
                        damage: 29, lifetimeMs: 500, maxRadius: 12, count: 10, shootAngle: 36, fixedAngle: 0,
                        cooldownMS: 1000, size: 105, multiHit: true),
                    new Suicide()
                )
            );

        [CharacterBehavior("Flamer")]
        public static State Flamer =>
            new(
                new CharacterLoot(
                    // new TierLoot(5, ItemType.Weapon, 0.04)
                ),
                new State("Attacking",
                    new State("Charge",
                        new Protect(4.62f, "Flamer King"),
                        new Follow(4.62f, 0.1f),
                        new EntityWithinTransition("Bullet", radius: 2)
                    ),
                    new State("Bullet",
                        new Flash(0xffaa00, 0.2, 20),
                        new ChangeSize(20, 130),
                        new Shoot(path: new AmplitudePath(8f, 0.3f, 0.5f, lifetimeMs: 500), targeted: true, projName: "White Flame",
                            damage: 29, lifetimeMs: 500, maxRadius: 8, cooldownMS: 200, multiHit: true),
                        new TimedTransition(4000, "Wait")
                    ),
                    new State("Wait",
                        new ChangeSize(-20, 70),
                        new TimedTransition(600, "Charge")
                    ),
                    new HpLessTransition(0.2f, "FlashBeforeExplode")
                ),
                new State("FlashBeforeExplode",
                    new Flash(0xff0000, 1, 1),
                    new TimedTransition(300, "Explode")
                ),
                new State("Explode",
                    new Shoot(path: new AmplitudePath(8f, 0.3f, 0.5f, lifetimeMs: 500), targeted: false, projName: "White Flame",
                        damage: 29, lifetimeMs: 500, maxRadius: 12, count: 10, shootAngle: 36, fixedAngle: 0,
                        cooldownMS: 1000, multiHit: true),
                    new Suicide()
                )
            );

        [CharacterBehavior("Dragon Egg")]
        public static State DragonEgg =>
            new(
                new TransformOnDeath("White Dragon Whelp", probability: 0.3f),
                new TransformOnDeath("Juvenile White Dragon", probability: 0.2f),
                new TransformOnDeath("Adult White Dragon", probability: 0.1f)
            );

        [CharacterBehavior("White Dragon Whelp")]
        public static State WhiteDragonWhelp =>
            new(
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Red Fire", damage: 30, lifetimeMs: 2300,
                    maxRadius: 10, count: 2, shootAngle: 20, predictive: 0.3f, cooldownMS: 750, size: 80),
                new Follow(11.84f, 2.5f, 10.5f, cooldownOffsetMS: 2200, cooldownMS: 3200),
                new Wander(5.73f)
            );

        [CharacterBehavior("Juvenile White Dragon")]
        public static State JuvenileWhiteDragon =>
            new(
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Red Fire", damage: 43, lifetimeMs: 2200,
                    maxRadius: 10, count: 2, shootAngle: 20, predictive: 0.3f, cooldownMS: 750),
                new Follow(10.73f, 2.2f, 10.5f, cooldownOffsetMS: 3000, cooldownMS: 3000),
                new Wander(4.9f)
            );

        [CharacterBehavior("Adult White Dragon")]
        public static State AdultWhiteDragon =>
            new(
                new CharacterLoot( // Threshold 1%
                    // new TierLoot(7, ItemType.Armor, 0.05),
                    new ItemLoot("Seal of the Divine", 0.015f, 0.01f),
                    new ItemLoot("White Drake Egg", 0.004f, 0.01f)
                ),
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Red Fire", damage: 70, lifetimeMs: 2200,
                    maxRadius: 10, count: 3, shootAngle: 15, predictive: 0.3f, cooldownMS: 750, size: 120),
                new Follow(8.5f, 1.8f, 10.5f, cooldownOffsetMS: 4000, cooldownMS: 2000),
                new Wander(4.9f)
            );

        [CharacterBehavior("Shield Orc Shield")]
        public static State ShieldOrcShield =>
            new(
                new CharacterLoot(
                    // new TierLoot(2, ItemType.Ring, 0.01)
                ),
                new Orbit(6.3f, 3, target: "Shield Orc Flooder"),
                new Wander(1.3f),
                new State("Attacking",
                    new State("Attack",
                        new Flash(0x000000, 10, 100),
                        new Shoot(path: new LinePath(6f), targeted: true, projName: "White Bullet", damage: 40,
                            lifetimeMs: 700, maxRadius: 10, cooldownMS: 500, size: 30, multiHit: true),
                        new HpLessTransition(0.5f, "Heal"),
                        new EntityNotWithinTransition("Shield Orc Key", 7, targetStates: "Idling")
                    ),
                    new State("Heal",
                        new HealGroup(7, "Shield Orcs", 500),
                        new TimedTransition(500, "Attack"),
                        new EntityNotWithinTransition("Shield Orc Key", 7, targetStates: "Idling")
                    )
                ),
                new State("Flash",
                    new Flash(0xff0000, 1, 1),
                    new TimedTransition(300, "Idling")
                ),
                new State("Idling")
            );

        [CharacterBehavior("Shield Orc Flooder")]
        public static State ShieldOrcFlooder =>
            new(
                new CharacterLoot(
                    // new TierLoot(4, ItemType.Ability, 0.01)
                ),
                new Wander(1.3f),
                new State("Attacking",
                    new State("Attack",
                        new Flash(0x000000, 10, 100),
                        new Shoot(path: new LinePath(6f), targeted: true, projName: "White Bullet", damage: 40,
                            lifetimeMs: 700, maxRadius: 10, cooldownMS: 500, size: 30, multiHit: true),
                        new HpLessTransition(0.5f, "Heal"),
                        new EntityNotWithinTransition("Shield Orc Key", 7, targetStates: "Idling")
                    ),
                    new State("Heal",
                        new HealGroup(7, "Shield Orcs", 500),
                        new TimedTransition(500, "Attack"),
                        new EntityNotWithinTransition("Shield Orc Key", 7, targetStates: "Idling")
                    )
                ),
                new State("Flash",
                    new Flash(0xff0000, 1, 1),
                    new TimedTransition(300, "Idling")
                ),
                new State("Idling")
            );

        [CharacterBehavior("Shield Orc Key")]
        public static State ShieldOrcKey =>
            new(
                new CharacterLoot(
                    // new TierLoot(4, ItemType.Armor, 0.01)
                ),
                new Spawn("Shield Orc Flooder", 0, 0, maxSpawnsPerReset: 1, cooldownMs: 10000),
                new Spawn("Shield Orc Shield", 0, 0, maxSpawnsPerReset: 1, cooldownMs: 10000),
                new Spawn("Shield Orc Shield", 0, 0, maxSpawnsPerReset: 1, cooldownMs: 10000),
                new State("Start",
                    new TimedTransition(500, "Attacking")
                ),
                new State("Attacking",
                    new Orbit(1, 3, target: "Shield Orc Flooder"),
                    new Order(7, "Shield Orc Flooder", "Attacking"),
                    new Order(7, "Shield Orc Shield", "Attacking"),
                    new HpLessTransition(0.5f, "FlashBeforeExplode")
                ),
                new State("FlashBeforeExplode",
                    new Order(7, "Shield Orc Flooder", "Flash"),
                    new Order(7, "Shield Orc Shield", "Flash"),
                    new Flash(0xff0000, 1, 1),
                    new TimedTransition(300, "Explode")
                ),
                new State("Explode",
                    new Shoot(path: new LinePath(3f), targeted: true, projName: "White Bullet", damage: 40,
                        lifetimeMs: 3000, maxRadius: 12, count: 10, shootAngle: 36, fixedAngle: 0, cooldownMS: 1000,
                        size: 30, multiHit: true),
                    new Suicide()
                )
            );

        [CharacterBehavior("Left Horizontal Trap")]
        public static State LeftHorizontalTrap =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                new State("weak_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread", damage: 20,
                        lifetimeMs: 1450, maxRadius: 1, fixedAngle: 0, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.Weak, 6000)),
                    new TimedTransition(2000, "blind_effect")
                ),
                new State("blind_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread 2", damage: 20,
                        lifetimeMs: 1450, maxRadius: 1, fixedAngle: 0, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.Blind, 2000)),
                    new TimedTransition(2000, "pierce_effect")
                ),
                new State("pierce_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread 3", damage: 20,
                        lifetimeMs: 1450, maxRadius: 1, fixedAngle: 0, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.ArmorBroken, 2400)),
                    new TimedTransition(2000, "weak_effect")
                ),
                new Suicide(6000)
            );

        [CharacterBehavior("Top Vertical Trap")]
        public static State TopVerticalTrap =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                new State("weak_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread", damage: 20,
                        lifetimeMs: 1450, maxRadius: 1, fixedAngle: 90, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.Weak, 6000)),
                    new TimedTransition(2000, "blind_effect")
                ),
                new State("blind_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread 2", damage: 20,
                        lifetimeMs: 1450, maxRadius: 1, fixedAngle: 90, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.Blind, 2000)),
                    new TimedTransition(2000, "pierce_effect")
                ),
                new State("pierce_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread 3", damage: 20,
                        lifetimeMs: 1450, maxRadius: 1, fixedAngle: 90, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.ArmorBroken, 2400)),
                    new TimedTransition(2000, "weak_effect")
                ),
                new Suicide(6000)
            );

        [CharacterBehavior("45-225 Diagonal Trap")]
        public static State DiagonalTrap45225 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                new State("weak_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread", damage: 20,
                        lifetimeMs: 370, maxRadius: 1, fixedAngle: 45, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.Weak, 6000)),
                    new TimedTransition(2000, "blind_effect")
                ),
                new State("blind_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread 2", damage: 20,
                        lifetimeMs: 370, maxRadius: 1, fixedAngle: 45, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.Blind, 2000)),
                    new TimedTransition(2000, "pierce_effect")
                ),
                new State("pierce_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread 3", damage: 20,
                        lifetimeMs: 370, maxRadius: 1, fixedAngle: 45, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.ArmorBroken, 2400)),
                    new TimedTransition(2000, "weak_effect")
                ),
                new Suicide(6000)
            );

        [CharacterBehavior("135-315 Diagonal Trap")]
        public static State DiagonalTrap135315 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                new State("weak_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread", damage: 20,
                        lifetimeMs: 370, maxRadius: 1, fixedAngle: 135, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.Weak, 6000)),
                    new TimedTransition(2000, "blind_effect")
                ),
                new State("blind_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread 2", damage: 20,
                        lifetimeMs: 370, maxRadius: 1, fixedAngle: 135, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.Blind, 2000)),
                    new TimedTransition(2000, "pierce_effect")
                ),
                new State("pierce_effect",
                    new Shoot(path: new LinePath(7f), targeted: false, projName: "Web Thread 3", damage: 20,
                        lifetimeMs: 370, maxRadius: 1, fixedAngle: 135, cooldownMS: 200, size: 210, multiHit: true, passesCover: true, effects: (ConditionEffectIndex.ArmorBroken, 6000)),
                    new TimedTransition(2000, "weak_effect")
                ),
                new Suicide(6000)
            );

        [CharacterBehavior("Urgle")]
        public static State Urgle =>
            new(
                new CharacterLoot(
                    // new TierLoot(5, ItemType.Weapon, 0.26),
                    // new TierLoot(6, ItemType.Weapon, 0.18),
                    // new TierLoot(7, ItemType.Weapon, 0.1),
                    // new TierLoot(5, ItemType.Armor, 0.26),
                    // new TierLoot(6, ItemType.Armor, 0.18),
                    // new TierLoot(7, ItemType.Armor, 0.1),
                    // new TierLoot(3, ItemType.Ring, 0.15),
                    // new TierLoot(3, ItemType.Ability, 0.15)
                ),
                new DropPortalOnDeath("Spider Den Portal", 0.9f),
                new Wander(3.5f, distanceFromSpawn: 3),
                new Shoot(path: new LinePath(7f), targeted: true, projName: "White Bullet", damage: 48,
                    lifetimeMs: 1500, maxRadius: 8, predictive: 0.3f, cooldownMS: 1000, size: 80, multiHit: true),
                new State("idle",
                    new EntityWithinTransition("toss_horizontal_traps", radius: 10.5f)
                ),
                new State("toss_horizontal_traps",
                    new TossObject("Left Horizontal Trap", 9, 230, 100000),
                    new TossObject("Left Horizontal Trap", 10, 180, 100000),
                    new TossObject("Left Horizontal Trap", 9, 140, 100000),
                    new TimedTransition(1000, "toss_vertical_traps")
                ),
                new State("toss_vertical_traps",
                    new TossObject("Top Vertical Trap", 8, 200, 100000),
                    new TossObject("Top Vertical Trap", 10, 240, 100000),
                    new TossObject("Top Vertical Trap", 10, 280, 100000),
                    new TossObject("Top Vertical Trap", 8, 320, 100000),
                    new TimedTransition(1000, "toss_diagonal_traps")
                ),
                new State("toss_diagonal_traps",
                    new TossObject("45-225 Diagonal Trap", 2, 45, 100000),
                    new TossObject("45-225 Diagonal Trap", 7, 45, 100000),
                    new TossObject("45-225 Diagonal Trap", 11, 225, 100000),
                    new TossObject("45-225 Diagonal Trap", 6, 225, 100000),
                    new TossObject("135-315 Diagonal Trap", 2, 135, 100000),
                    new TossObject("135-315 Diagonal Trap", 7, 135, 100000),
                    new TossObject("135-315 Diagonal Trap", 11, 315, 100000),
                    new TossObject("135-315 Diagonal Trap", 6, 315, 100000),
                    new TimedTransition(1000, "wait")
                ),
                new State("wait",
                    new TimedTransition(2400, "idle")
                )
            );

        [CharacterBehavior("Kage Kami")]
        public static State KageKami =>
            new(
                new CharacterLoot( // Threshold 1%
                    new ItemLoot("Potion of Vitality", 1, 0.01f),
                    new ItemLoot("Potion of Dexterity", 1, 0.01f),
                    new ItemLoot("Potion of Defense", 1, 0.01f),
                    new ItemLoot("Potion of Speed", 1, 0.01f)
                ),
                // new ScaleHP2(30),
                new DropPortalOnDeath("Manor of the Immortals Portal", 1),
                new State("yay i am good",
                    new Taunt("Kyoufu no kage!", 0, 0.5f),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ChangeSize(20, 120),
                    new SetAltTexture(1),
                    new TimedTransition(2000, "Attack")
                ),
                new State("Attack",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Wander(2.96f),
                    new SetAltTexture(1),
                    new TimedTransition(5000, "Charge"),
                    new TossObject("Specter Mine", cooldownMS: 2000),
                    new State("Shoot1",
                        new Shoot(path: new LinePath(3.5f), targeted: false, projName: "Specter Shot", damage: 75,
                            lifetimeMs: 2600, maxRadius: 12, count: 1, fixedAngle: 0, rotateAngle: 30, cooldownMS: 300,
                            size: 90, effects: (ConditionEffectIndex.Paralyzed, 400)),
                        new Shoot(path: new LinePath(3.5f), targeted: false, projName: "Specter Shot", damage: 75,
                            lifetimeMs: 2600, maxRadius: 12, count: 1, fixedAngle: 180, rotateAngle: 30, cooldownMS: 300,
                            size: 90, effects: (ConditionEffectIndex.Paralyzed, 400)),
                        new TimedTransition(1000, "Shoot2")
                    ),
                    new State("Shoot2",
                        new Shoot(path: new LinePath(3.5f), targeted: false, projName: "Specter Shot", damage: 75,
                            lifetimeMs: 2600, maxRadius: 20, count: 2, shootAngle: 180, cooldownMS: 400, size: 90),
                        new TimedTransition(400, "Shoot1")
                    )
                ),
                new State("Charge",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TossObject("Specter Mine", cooldownMS: 2000),
                    new SetAltTexture(2),
                    new Follow(5.18f, 20, 1),
                    new Shoot(path: new LinePath(5.5f), targeted: false, projName: "Specter Spinner", damage: 50,
                        lifetimeMs: 2000, maxRadius: 20, count: 2, shootAngle: 50, cooldownMS: 400, size: 80, effects: [(ConditionEffectIndex.Sick, 4000), (ConditionEffectIndex.Confused, 1000)]),
                    new TimedTransition(4000, "Attack")
                )
            );

        [CharacterBehavior("Specter Mine")]
        public static State SpecterMine =>
            new(
                new State("Waiting",
                    new EntityWithinTransition("Suicide", radius: 3),
                    new TimedTransition(4000, "Suicide")
                ),
                new State("Suicide",
                    new Shoot(path: new LinePath(6f), targeted: true, projName: "Specter Spike", damage: 55,
                        lifetimeMs: 1200, maxRadius: 60, count: 4, shootAngle: 45, cooldownMS: 1000, multiHit: true, effects: (ConditionEffectIndex.Sick, 4000)),
                    new Suicide()
                )
            );
    }
}