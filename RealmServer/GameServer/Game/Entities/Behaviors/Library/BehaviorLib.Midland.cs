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
        [CharacterBehavior("Fire Sprite")]
        public static State FireSprite =>
            new(
                new CharacterLoot(
                    // new TierLoot(5, ItemType.Weapon, 0.02),
                    new ItemLoot("Magic Potion", 0.05f)
                ),
                new State("0",
                    new Spawn("Fire Sprite", 0, 0, maxSpawnsPerReset: 2, maxDensity: 2, cooldownMs: 60000),
                    new Shoot(path: new LinePath(5f), targeted: true, projName: "Red Fire", damage: 17,
                        lifetimeMs: 4000, maxRadius: 10, count: 2, shootAngle: 7, cooldownMS: 300),
                    new Wander(8.5f)
                )
            );

        [CharacterBehavior("Ice Sprite")]
        public static State IceSprite =>
            new(
                new CharacterLoot(
                    // new TierLoot(2, ItemType.Ability, 0.04),
                    new ItemLoot("Magic Potion", 0.05f)
                ),
                new State("0",
                    new Spawn("Ice Sprite", 0, 0, maxSpawnsPerReset: 2, maxDensity: 2, cooldownMs: 60000),
                    new Shoot(path: new LinePath(8f), targeted: true, projName: "Ice Spinner", damage: 14,
                        lifetimeMs: 1800, maxRadius: 10, count: 3, shootAngle: 7, cooldownMS: 300, size: 70, effects: (ConditionEffectIndex.Slowed, 1600)),
                    new Wander(8.5f)
                )
            );

        [CharacterBehavior("Magic Sprite")]
        public static State MagicSprite =>
            new(
                new CharacterLoot(
                    // new TierLoot(6, ItemType.Armor, 0.01),
                    new ItemLoot("Magic Potion", 0.05f)
                ),
                new State("0",
                    new Spawn("Magic Sprite", 0, 0, maxSpawnsPerReset: 2, maxDensity: 2, cooldownMs: 60000),
                    new Shoot(path: new LinePath(5f), targeted: true, projName: "Green Magic", damage: 18,
                        lifetimeMs: 4000, maxRadius: 10, count: 4, shootAngle: 7, cooldownMS: 300, size: 120),
                    new Wander(8.5f)
                )
            );

        [CharacterBehavior("Orc King")]
        public static State OrcKing =>
            new(
                new CharacterLoot(
                    // new TierLoot(4, ItemType.Weapon, 0.18f),
                    // new TierLoot(5, ItemType.Weapon, 0.05f),
                    // new TierLoot(5, ItemType.Armor, 0.21f),
                    // new TierLoot(6, ItemType.Armor, 0.035f),
                    // new TierLoot(2, ItemType.Ring, 0.07f),
                    // new TierLoot(2, ItemType.Ability, 0.17f),
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new DropPortalOnDeath("Spider Den Portal", 0.1f),
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Blade", damage: 27,
                    lifetimeMs: 1200, maxRadius: 3, cooldownMS: 1000, size: 120),
                new Spawn("Orc Queen", 0, 0, maxSpawnsPerReset: 2, cooldownMs: 60000),
                new Follow(4.07f, acquireRange: 1, distFromTarget: 1, cooldownMS: 3000),
                new Wander(4.07f, distanceFromSpawn: 1)
            );

        [CharacterBehavior("Orc Queen")]
        public static State OrcQueen =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f)
                ),
                new Spawn("Orc Mage", 0, 0, maxSpawnsPerReset: 2, cooldownMs: 8000),
                new Spawn("Orc Warrior", 0, 0, maxSpawnsPerReset: 3, cooldownMs: 8000),
                new Protect(4.14f, "Orc King", 11, 7, 5.4f),
                new Wander(2.94f),
                new HealGroup(10, "OrcKings", 300)
            );

        [CharacterBehavior("Orc Mage")]
        public static State OrcMage =>
            new(
                new CharacterLoot(
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new State("circle_player",
                    new Shoot(8, new LinePath(6f), targeted: true, projName: "Red Fire", damage: 17,
                        lifetimeMs: 3000, predictive: 0.3f, cooldownMS: 1000, coolDownOffset: 500),
                    new Protect(4.61f, "Orc Queen", 11, 10, 3),
                    new Orbit(4.61f, 3.5f, 11),
                    new TimedTransition(3500, "circle_queen")
                ),
                new State("circle_queen",
                    new Shoot(8, new LinePath(6f), targeted: true, projName: "Red Fire", damage: 17,
                        lifetimeMs: 3000, count: 3, predictive: 0.3f, shootAngle: 120, cooldownMS: 1000,
                        coolDownOffset: 500),
                    new Orbit(7.36f, 2.5f, target: "Orc Queen", acquireRange: 12, speedVariance: 0.1f,
                        radiusVariance: 0.1f),
                    new TimedTransition(3500, "circle_player")
                )
            );

        [CharacterBehavior("Orc Warrior")]
        public static State OrcWarrior =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f)
                ),
                new Shoot(3, new LinePath(7f), targeted: true, projName: "Blade", damage: 19,
                    lifetimeMs: 1200, predictive: 1f, cooldownMS: 500),
                new Orbit(8.21f, 2.5f, target: "Orc Queen", acquireRange: 12, speedVariance: 0.1f,
                    radiusVariance: 0.1f)
            );

        [CharacterBehavior("Pink Blob")]
        public static State PinkBlob =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f),
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Shoot(6, new LinePath(5f), targeted: true, projName: "Green Magic", damage: 10,
                    lifetimeMs: 2000, count: 3, shootAngle: 7, cooldownMS: 1000),
                new Follow(4.14f, acquireRange: 15, distFromTarget: 5),
                new Wander(2.94f),
                new Spawn("Pink Blob", 0, 0, maxSpawnsPerReset: 5, maxDensity: 5, cooldownMs: 60000)
            );

        [CharacterBehavior("Gray Blob")]
        public static State GrayBlob =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f),
                    new ItemLoot("Magic Potion", 0.03f),
                    new ItemLoot("Magic Mushroom", 0.005f)
                ),
                new State("searching",
                    new Charge(11.84f),
                    new Wander(2.94f),
                    new Spawn("Gray Blob", 0, 0, maxSpawnsPerReset: 5, maxDensity: 5, cooldownMs: 60000),
                    new EntityWithinTransition("creeping", radius: 2)
                ),
                new State("creeping",
                    new Shoot(12, new LinePath(7f), targeted: false, projName: "Red Fire", damage: 25,
                        lifetimeMs: 1200, count: 10, shootAngle: 36, fixedAngle: 0, cooldownMS: 1000),
                    new Suicide()
                )
            );

        [CharacterBehavior("Big Green Slime")]
        public static State BigGreenSlime =>
            new(
                new Shoot(9, new LinePath(7f), targeted: true, projName: "Red Fire", damage: 10,
                    lifetimeMs: 1200, cooldownMS: 1000),
                new Wander(2.94f),
                new Spawn("Big Green Slime", 0, 0, maxSpawnsPerReset: 5, maxDensity: 5, cooldownMs: 60000),
                new TransformOnDeath("Little Green Slime"),
                new TransformOnDeath("Little Green Slime"),
                new TransformOnDeath("Little Green Slime"),
                new TransformOnDeath("Little Green Slime")
            );

        [CharacterBehavior("Little Green Slime")]
        public static State LittleGreenSlime =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f),
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Shoot(6, new LinePath(7f), targeted: true, projName: "Red Fire", damage: 10,
                    lifetimeMs: 1200, cooldownMS: 1000),
                new Wander(2.94f),
                new Protect(2.94f, "Big Green Slime")
            );

        [CharacterBehavior("Wasp Queen")]
        public static State WaspQueen =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.15f),
                    new ItemLoot("Magic Potion", 0.07f)
                    // new TierLoot(5, ItemType.Weapon, 0.14f),
                    // new TierLoot(6, ItemType.Weapon, 0.05f),
                    // new TierLoot(5, ItemType.Armor, 0.19f),
                    // new TierLoot(6, ItemType.Armor, 0.02f),
                    // new TierLoot(2, ItemType.Ring, 0.07f),
                    // new TierLoot(3, ItemType.Ring, 0.001f),
                    // new TierLoot(2, ItemType.Ability, 0.28f),
                    // new TierLoot(3, ItemType.Ability, 0.01f)
                ),
                new Spawn("Worker Wasp", 0, 0, maxSpawnsPerReset: 5, cooldownMs: 3400),
                new Spawn("Warrior Wasp", 0, 0, maxSpawnsPerReset: 2, cooldownMs: 4400),
                new State("idle",
                    new Wander(3.79f),
                    new EntityWithinTransition("froth", 10)
                ),
                new State("froth",
                    new Shoot(8, new LinePath(7.5f), targeted: true, projName: "Yellow Missile", damage: 22,
                        lifetimeMs: 1200, predictive: 0.1f, cooldownMS: 1600, size: 80),
                    new Wander(3.79f)
                )
            );

        [CharacterBehavior("Worker Wasp")]
        public static State WorkerWasp =>
            new(
                new Shoot(8, new LinePath(7.5f), targeted: true, projName: "Yellow Missile", damage: 16,
                    lifetimeMs: 1200, cooldownMS: 4000, size: 60),
                new Orbit(6.3f, 2, target: "Wasp Queen", radiusVariance: 0.5f),
                new Wander(4.91f)
            );

        [CharacterBehavior("Warrior Wasp")]
        public static State WarriorWasp =>
            new(
                new Shoot(8, new LinePath(7.5f), targeted: true, projName: "Blue Missile", damage: 25,
                    lifetimeMs: 1200, predictive: 200, cooldownMS: 1000, size: 80),
                new State("protecting",
                    new Orbit(6.3f, 2, target: "Wasp Queen", radiusVariance: 0),
                    new Wander(4.91f),
                    new TimedTransition(3000, "attacking")
                ),
                new State("attacking",
                    new Follow(4.18f, acquireRange: 9, distFromTarget: 3.4f),
                    new Orbit(6.3f, 2, target: "Wasp Queen", radiusVariance: 0),
                    new Wander(4.91f),
                    new TimedTransition(2200, "protecting")
                )
            );

        [CharacterBehavior("Shambling Sludge")]
        public static State ShamblingSludge =>
            new(
                new State("idle",
                    new EntityWithinTransition("toss_sludge", radius: 10)
                ),
                new State("toss_sludge", // Should be parametric
                    new Shoot(8, new LinePath(4.2f), targeted: true, projName: "Gray Spinner", damage: 22,
                        lifetimeMs: 3200, cooldownMS: 1200, size: 80, effects: (ConditionEffectIndex.Slowed, 2000)),
                    new TossObject("Sludget", 3, 20, 100000),
                    new TossObject("Sludget", 3, 92, 100000),
                    new TossObject("Sludget", 3, 164, 100000),
                    new TossObject("Sludget", 3, 236, 100000),
                    new TossObject("Sludget", 3, 308, 100000),
                    new TimedTransition(8000, "pause")
                ),
                new State("pause",
                    new Wander(3.495f),
                    new TimedTransition(1000, "idle")
                )
            );

        [CharacterBehavior("Sludget")]
        public static State Sludget =>
            new(
                new State("idle",
                    new Shoot(8, new LinePath(7f), targeted: true, projName: "White Flame", damage: 14,
                        lifetimeMs: 510, predictive: 0.5f, cooldownMS: 600, size: 82),
                    new Protect(3.495f, "Shambling Sludge", 11, 7.5f, 7.4f),
                    new Wander(3.495f),
                    new TimedTransition(1400, "wander")
                ),
                new State("wander",
                    new Protect(3.495f, "Shambling Sludge", 11, 7.5f, 7.4f),
                    new Wander(3.495f),
                    new TimedTransition(5400, "jump")
                ),
                new State("jump",
                    new Protect(3.495f, "Shambling Sludge", 11, 7.5f, 7.4f),
                    new Follow(39.64f, acquireRange: 6, distFromTarget: 1),
                    new Wander(3.495f),
                    new TimedTransition(200, "attack")
                ),
                new State("attack",
                    new Shoot(8, new LinePath(5f), targeted: true, projName: "Green Magic", damage: 18,
                        lifetimeMs: 4000, predictive: 0.5f, cooldownMS: 600, coolDownOffset: 300, size: 82),
                    new Protect(3.495f, "Shambling Sludge", 11, 7.5f, 7.4f),
                    new Follow(3.495f, acquireRange: 6, distFromTarget: 1),
                    new Wander(3.495f),
                    new TimedTransition(4000, "idle")
                ),
                new Suicide(9000)
            );

        [CharacterBehavior("Swarm")]
        public static State Swarm =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.24f),
                    new ItemLoot("Magic Potion", 0.07f)
                    // new TierLoot(3, ItemType.Weapon, 0.22f),
                    // new TierLoot(4, ItemType.Weapon, 0.05f),
                    // new TierLoot(3, ItemType.Armor, 0.22f),
                    // new TierLoot(4, ItemType.Armor, 0.12f),
                    // new TierLoot(5, ItemType.Armor, 0.02f),
                    // new TierLoot(1, ItemType.Ring, 0.1f),
                    // new TierLoot(1, ItemType.Ability, 0.21f)
                ),
                new State("circle",
                    new Follow(22.94f, acquireRange: 11, distFromTarget: 3.5f, cooldownMS: 5000),
                    new Orbit(11.27f, 3.5f, 12),
                    new Wander(2.94f),
                    new Shoot(4, new LinePath(6.5f), targeted: true, projName: "White Flame", damage: 15,
                        lifetimeMs: 1100, predictive: 0.1f, cooldownMS: 500, size: 60),
                    new TimedTransition(3000, "dart_away")
                ),
                new State("dart_away",
                    new StayAwayFrom(11.84f, 5),
                    new Wander(2.94f),
                    new Shoot(8, new LinePath(6.5f), targeted: false, projName: "White Flame", damage: 15,
                        lifetimeMs: 1100, count: 5, shootAngle: 72, fixedAngle: 20, cooldownMS: 100000,
                        coolDownOffset: 800, size: 60),
                    new Shoot(8, new LinePath(6.5f), targeted: false, projName: "White Flame", damage: 15,
                        lifetimeMs: 1100, count: 5, shootAngle: 72, fixedAngle: 56, cooldownMS: 100000,
                        coolDownOffset: 1400, size: 60),
                    new TimedTransition(1600, "circle")
                ),
                new Spawn("Swarm", 0, 0, maxSpawnsPerReset: 1, maxDensity: 1, densityRadius: 100, cooldownMs: 60000)
            );

        [CharacterBehavior("Black Bat")]
        public static State BlackBat =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.01f),
                    new ItemLoot("Magic Potion", 0.01f)
                    // new TierLoot(2, ItemType.Armor, 0.01f)
                ),
                new Charge(),
                new Wander(),
                new Shoot(1, new LinePath(10f), targeted: true, projName: "Blue Magic", damage: 25,
                    lifetimeMs: 200, cooldownMS: 1000)
            );

        [CharacterBehavior("Red Spider")]
        public static State RedSpider =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f),
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Shoot(9, new LinePath(5f), targeted: true, projName: "Green Magic", damage: 25,
                    lifetimeMs: 2000, cooldownMS: 1000),
                new Wander(2.94f)
            );

        [CharacterBehavior("Dwarf Axebearer")]
        public static State DwarfAxebearer =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(3.4f, new LinePath(6f), targeted: true, projName: "Blade", damage: 10,
                    lifetimeMs: 600, cooldownMS: 1000, size: 80),
                new State("Default",
                    new Wander(2.94f)
                ),
                new State("Circling",
                    new Orbit(2.94f, 2.7f, 11),
                    new Protect(7.36f, "Dwarf King", 15, 6, 3),
                    new Wander(2.94f),
                    new TimedTransition(3300, "Default"),
                    new EntityNotWithinTransition("Dwarf King", 8, targetStates: "Default")
                ),
                new State("Engaging",
                    new Follow(6.29f, acquireRange: 15, distFromTarget: 1),
                    new Protect(7.36f, "Dwarf King", 15, 6, 3),
                    new Wander(2.94f),
                    new TimedTransition(2500, "Circling"),
                    new EntityNotWithinTransition("Dwarf King", 8, targetStates: "Default")
                )
            );

        [CharacterBehavior("Dwarf Mage")]
        public static State DwarfMage =>
            new(
                new State("Default",
                    new Protect(7.38f, "Dwarf King", 15, 6, 3),
                    new Wander(4.07f),
                    new State("fire1_def",
                        new Shoot(10, new AmplitudePath(3.5f, 1, 3, lifetimeMs: 3500), targeted: true,
                            projName: "Red Fire",
                            damage: 12, lifetimeMs: 3500, predictive: 0.2f, cooldownMS: 100000, size: 80),
                        new TimedTransition(1500, "fire2_def")
                    ),
                    new State("fire2_def",
                        new Shoot(5, new AmplitudePath(3.5f, 1, 3, lifetimeMs: 3500), targeted: true,
                            projName: "Red Fire",
                            damage: 12, lifetimeMs: 3500, predictive: 0.2f, cooldownMS: 100000, size: 80),
                        new TimedTransition(1500, "fire1_def")
                    )
                ),
                new State("Circling",
                    new Orbit(2.92f, acquireRange: 11, radius: 2.7f),
                    new Protect(7.38f, "Dwarf King", 15, 6, 3),
                    new Wander(4.07f),
                    new State("fire1_cir",
                        new Shoot(10, new AmplitudePath(3.5f, 1, 3, lifetimeMs: 3500), targeted: true,
                            projName: "Red Fire",
                            damage: 12, lifetimeMs: 3500, predictive: 0.2f, cooldownMS: 100000, size: 80),
                        new TimedTransition(1500, "fire2_cir")
                    ),
                    new State("fire2_cir",
                        new Shoot(5, new AmplitudePath(3.5f, 1, 3, lifetimeMs: 3500), targeted: true,
                            projName: "Red Fire",
                            damage: 12, lifetimeMs: 3500, predictive: 0.2f, cooldownMS: 100000, size: 80),
                        new TimedTransition(1500, "fire1_cir")
                    ),
                    new TimedTransition(3300, "Default"),
                    new EntityNotWithinTransition("Dwarf King", 8, targetStates: "Default")
                ),
                new State("Engaging",
                    new Follow(6.29f, acquireRange: 15, distFromTarget: 1),
                    new Protect(7.38f, "Dwarf King", 15, 6, 3),
                    new Wander(2.94f),
                    new State("fire1_eng",
                        new Shoot(10, new AmplitudePath(3.5f, 1, 3, lifetimeMs: 3500), targeted: true,
                            projName: "Red Fire",
                            damage: 12, lifetimeMs: 3500, predictive: 0.2f, cooldownMS: 100000, size: 80),
                        new TimedTransition(1500, "fire2_eng")
                    ),
                    new State("fire2_eng",
                        new Shoot(5, new AmplitudePath(3.5f, 1, 3, lifetimeMs: 3500), targeted: true,
                            projName: "Red Fire",
                            damage: 12, lifetimeMs: 3500, predictive: 0.2f, cooldownMS: 100000, size: 80),
                        new TimedTransition(1500, "fire1_eng")
                    ),
                    new TimedTransition(2500, "Circling"),
                    new EntityNotWithinTransition("Dwarf King", 8, targetStates: "Default")
                )
            );

        [CharacterBehavior("Dwarf Veteran")]
        public static State DwarfVeteran =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(4, new LinePath(6f), targeted: true, projName: "Blade", damage: 14,
                    lifetimeMs: 600, cooldownMS: 1000, size: 110),
                new State("Default",
                    new Follow(6.29f, acquireRange: 9, distFromTarget: 2, cooldownOffsetMS: 3000,
                        cooldownMS: 1000),
                    new Wander(2.94f)
                ),
                new State("Circling",
                    new Orbit(2.92f, acquireRange: 11, radius: 2.7f),
                    new Protect(7.38f, "Dwarf King", 15, 6, 3),
                    new Wander(2.94f),
                    new TimedTransition(3300, "Default"),
                    new EntityNotWithinTransition("Dwarf King", 8, targetStates: "Default")
                ),
                new State("Engaging",
                    new Follow(6.29f, acquireRange: 15, distFromTarget: 1),
                    new Protect(7.38f, "Dwarf King", 15, 6, 3),
                    new Wander(2.94f),
                    new TimedTransition(2500, "Circling"),
                    new EntityNotWithinTransition("Dwarf King", 8, targetStates: "Default")
                )
            );

        [CharacterBehavior("Dwarf King")]
        public static State DwarfKing =>
            new(
                new CharacterLoot(
                    // new TierLoot(3, ItemType.Weapon, 0.2f),
                    // new TierLoot(4, ItemType.Weapon, 0.12f),
                    // new TierLoot(3, ItemType.Armor, 0.2f),
                    // new TierLoot(4, ItemType.Armor, 0.15f),
                    // new TierLoot(5, ItemType.Armor, 0.02f),
                    // new TierLoot(1, ItemType.Ring, 0.11f),
                    // new TierLoot(1, ItemType.Ability, 0.38f),
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Spawn(null, 0, 0, maxSpawnsPerReset: 10, cooldownMs: 8000, group: "Dwarves"),
                new Shoot(4, new LinePath(6f), targeted: true, projName: "Blade", damage: 16,
                    lifetimeMs: 900, cooldownMS: 2000, size: 120),
                new State("Circling",
                    new Orbit(2.92f, acquireRange: 11, radius: 2.7f),
                    new Wander(2.94f),
                    new TimedTransition(3400, "Engaging")
                ),
                new State("Engaging",
                    new Taunt("You'll taste my axe!", 0, 0.2f),
                    new Follow(6.29f, acquireRange: 15, distFromTarget: 1),
                    new Wander(2.94f),
                    new TimedTransition(2600, "Circling")
                )
            );

        [CharacterBehavior("Werelion")]
        public static State Werelion =>
            new(
                new CharacterLoot(
                    // new TierLoot(4, ItemType.Weapon, 0.18f),
                    // new TierLoot(5, ItemType.Weapon, 0.05f),
                    // new TierLoot(5, ItemType.Armor, 0.24f),
                    // new TierLoot(6, ItemType.Armor, 0.03f),
                    // new TierLoot(2, ItemType.Ring, 0.07f),
                    // new TierLoot(2, ItemType.Ability, 0.2f),
                    new ItemLoot("Health Potion", 0.04f),
                    new ItemLoot("Magic Potion", 0.05f)
                ),
                new DropPortalOnDeath("Spider Den Portal", 0.1f),
                new Spawn("Weretiger", 0, 0, maxSpawnsPerReset: 1, cooldownMs: 23000),
                new Spawn("Wereleopard", 0, 0, maxSpawnsPerReset: 2, cooldownMs: 9000),
                new Spawn("Werepanther", 0, 0, maxSpawnsPerReset: 3, cooldownMs: 15000),
                new Shoot(4, new LinePath(5.5f), targeted: true, projName: "Grey Missile", damage: 22,
                    lifetimeMs: 2700, cooldownMS: 2000, size: 110),
                new State("idle",
                    new Wander(4.06f),
                    new EntityWithinTransition(radius: 11, targetState: "player_nearby")
                ),
                new State("player_nearby",
                    new State("normal_attack",
                        new Shoot(10, new LinePath(5.5f), targeted: true, projName: "Grey Missile",
                            damage: 22,
                            lifetimeMs: 2700, count: 3, shootAngle: 15, predictive: 1f, cooldownMS: 10000, size: 110),
                        new TimedTransition(900, "if_cloaked")
                    ),
                    new State("if_cloaked",
                        new Shoot(10, new LinePath(5.5f), targeted: true, projName: "Grey Missile",
                            damage: 22,
                            lifetimeMs: 2700, count: 8, shootAngle: 45, fixedAngle: 20, cooldownMS: 1600,
                            coolDownOffset: 400, size: 110),
                        new Shoot(10, new LinePath(5.5f), targeted: false, projName: "Grey Missile",
                            damage: 22,
                            lifetimeMs: 2700, count: 8, shootAngle: 45, fixedAngle: 42, cooldownMS: 1600,
                            coolDownOffset: 1200, size: 110),
                        new EntityWithinTransition(radius: 10, targetState: "normal_attack")
                    ),
                    new Follow(2.94f, acquireRange: 7, distFromTarget: 3),
                    new Wander(4.06f),
                    new TimedTransition(30000, "idle")
                )
            );

        [CharacterBehavior("Weretiger")]
        public static State Weretiger =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f)
                ),
                new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Violet Missile", damage: 17,
                    lifetimeMs: 2200, predictive: 0.3f, cooldownMS: 1000, size: 82, passesCover: true),
                new Protect(6.84f, "Werelion", 12, 10, 5),
                new Follow(5.18f, 6.3f),
                new Wander(4.06f)
            );

        [CharacterBehavior("Wereleopard")]
        public static State Wereleopard =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f)
                ),
                new Shoot(4.5f, new LinePath(8f), targeted: true, projName: "Fire Missile", damage: 19,
                    lifetimeMs: 1000, predictive: 0.4f, cooldownMS: 900, size: 82),
                new Protect(6.84f, "Werelion", 12, 10, 5),
                new Follow(6.84f, 3),
                new Wander(6.29f)
            );

        [CharacterBehavior("Werepanther")]
        public static State Werepanther =>
            new(
                new CharacterLoot(
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new State("idle",
                    new Protect(4.36f, "Werelion", 11, 7.5f, 7.4f),
                    new EntityWithinTransition(radius: 9.5f, targetState: "wander")
                ),
                new State("wander",
                    new Protect(4.36f, "Werelion", 11, 7.5f, 7.4f),
                    new Follow(4.36f, 5, 10),
                    new Wander(4.36f),
                    new EntityWithinTransition(radius: 4, targetState: "jump")
                ),
                new State("jump",
                    new Protect(4.36f, "Werelion", 11, 7.5f, 7.4f),
                    new Follow(39.55f, 1, 6),
                    new Wander(3.8f),
                    new TimedTransition(200, "attack")
                ),
                new State("attack",
                    new Protect(4.36f, "Werelion", 11, 7.5f, 7.4f),
                    new Follow(3.47f, 1, 6),
                    new Wander(3.47f),
                    new Shoot(4, new LinePath(7f), targeted: true, projName: "Brown Magic", damage: 16,
                        lifetimeMs: 2000, predictive: 0.5f, cooldownMS: 800, coolDownOffset: 300, size: 82),
                    new TimedTransition(4000, "idle")
                )
            );

        [CharacterBehavior("Horned Drake")]
        public static State HornedDrake =>
            new(
                new CharacterLoot(
                    // new TierLoot(5, ItemType.Weapon, 0.14f),
                    // new TierLoot(6, ItemType.Weapon, 0.05f),
                    // new TierLoot(5, ItemType.Armor, 0.19f),
                    // new TierLoot(6, ItemType.Armor, 0.02f),
                    // new TierLoot(2, ItemType.Ring, 0.07f),
                    // new TierLoot(3, ItemType.Ring, 0.001f),
                    // new TierLoot(2, ItemType.Ability, 0.28f),
                    // new TierLoot(3, ItemType.Ability, 0.001f),
                    new ItemLoot("Health Potion", 0.09f),
                    new ItemLoot("Magic Potion", 0.12f)
                ),
                new Spawn("Drake Baby", 0, 0, maxSpawnsPerReset: 1, cooldownMs: 50000),
                new State("idle",
                    new EntityWithinTransition(radius: 10, targetState: "get_player")
                ),
                new State("get_player",
                    new Follow(5.26f, 2.7f, 10, cooldownOffsetMS: 5000,
                        cooldownMS: 1800),
                    new Wander(2.94f),
                    new State("one_shot1",
                        new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile",
                            damage: 30, lifetimeMs: 1500, predictive: 0.1f, cooldownMS: 800, size: 110),
                        new TimedTransition(900, "three_shot1")
                    ),
                    new State("three_shot1",
                        new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile",
                            damage: 30, lifetimeMs: 1500, count: 3, shootAngle: 40, predictive: 0.1f,
                            cooldownMS: 100000, coolDownOffset: 800, size: 110),
                        new TimedTransition(2000, "one_shot1")
                    )
                ),
                new State("protect_me",
                    new Protect(5.26f, "Drake Baby", 12, 2.5f,
                        1.5f),
                    new State("one_shot2",
                        new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile",
                            damage: 30, lifetimeMs: 1500, predictive: 0.1f, cooldownMS: 700, size: 110),
                        new TimedTransition(800, "three_shot2")
                    ),
                    new State("three_shot2",
                        new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile",
                            damage: 30, lifetimeMs: 1500, count: 3, shootAngle: 40, predictive: 0.1f,
                            cooldownMS: 100000, coolDownOffset: 700, size: 110),
                        new TimedTransition(1800, "one_shot2")
                    ),
                    new EntityNotWithinTransition("Drake Baby", 8, targetStates: "idle")
                )
            );

        [CharacterBehavior("Drake Baby")]
        public static State DrakeBaby =>
            new(
                new State("unharmed",
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile", damage: 20,
                        lifetimeMs: 1300, cooldownMS: 1500, size: 80),
                    new State("wander",
                        new Wander(2.94f),
                        new TimedTransition(2000, "find_mama")
                    ),
                    new State("find_mama",
                        new Protect(8.47f, "Horned Drake", 15, 4,
                            4),
                        new TimedTransition(2000, "wander")
                    ),
                    new HpLessTransition(0.65f, "call_mama")
                ),
                new State("call_mama",
                    new Flash(0x484848, 0.6f, 5000),
                    new State("get_close_to_mama",
                        new Taunt("Awwwk! Awwwk!"),
                        new Protect(8.47f, "Horned Drake", 15, 1,
                            1),
                        new TimedTransition(1500, "cry_for_mama")
                    ),
                    new State("cry_for_mama",
                        new StayAwayFrom(4.245f, 8),
                        new Order(8, "Horned Drake", "protect_me")
                    )
                )
            );

        [CharacterBehavior("Nomadic Shaman")]
        public static State NomadicShaman =>
            new(
                new CharacterLoot(
                    new ItemLoot("Magic Potion", 0.04f)
                ),
                new Wander(4.65f),
                new State("fire1",
                    new Shoot(10, new LinePath(5.5f), targeted: true, projName: "Green Magic",
                        damage: 20, lifetimeMs: 2600, count: 3, shootAngle: 11, cooldownMS: 500,
                        coolDownOffset: 500),
                    new TimedTransition(3100, "fire2")
                ),
                new State("fire2",
                    new Shoot(10, new LinePath(5.5f), targeted: true, projName: "Green Magic",
                        damage: 20, lifetimeMs: 2600, cooldownMS: 700, coolDownOffset: 700),
                    new TimedTransition(2200, "fire1")
                )
            );

        [CharacterBehavior("Sand Phantom")]
        public static State SandPhantom =>
            new(
                new Follow(5.445f, acquireRange: 10.5f, distFromTarget: 1),
                new Wander(5.445f),
                new Shoot(8, new LinePath(7f), targeted: true, projName: "Red Fire", damage: 18,
                    lifetimeMs: 1700, predictive: 0.4f, cooldownMS: 400, coolDownOffset: 600),
                new State("follow_player",
                    new EntityWithinTransition(radius: 4.4f, targetState: "sneak_away_from_player")
                ),
                new State("sneak_away_from_player",
                    new Transform("Sand Phantom Wisp")
                )
            );

        [CharacterBehavior("Sand Phantom Wisp")]
        public static State SandPhantomWisp =>
            new(
                new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Red Fire", damage: 20,
                    lifetimeMs: 2000, predictive: 0.4f, cooldownMS: 400, coolDownOffset: 600),
                new State("move_away_from_player",
                    new State("keep_back",
                        new StayAwayFrom(4.245f, 5),
                        new Wander(5.645f),
                        new TimedTransition(800, "wander")
                    ),
                    new State("wander",
                        new Wander(5.645f),
                        new TimedTransition(800, "keep_back")
                    ),
                    new TimedTransition(6500, "wisp_finished")
                ),
                new State("wisp_finished",
                    new Transform("Sand Phantom")
                )
            );

        [CharacterBehavior("Great Lizard")]
        public static State GreatLizard =>
            new(
                new CharacterLoot(
                    // new TierLoot(4, ItemType.Weapon, 0.14f),
                    // new TierLoot(5, ItemType.Weapon, 0.05f),
                    // new TierLoot(5, ItemType.Armor, 0.19f),
                    // new TierLoot(6, ItemType.Armor, 0.02f),
                    // new TierLoot(2, ItemType.Ring, 0.07f),
                    // new TierLoot(2, ItemType.Ability, 0.27f),
                    new ItemLoot("Health Potion", 0.12f),
                    new ItemLoot("Magic Potion", 0.10f)
                ),
                new State("idle",
                    new Wander(4.65f),
                    new EntityWithinTransition(radius: 10, targetState: "charge")
                ),
                new State("charge",
                    new Follow(33.94f, acquireRange: 11, distFromTarget: 1.5f),
                    new TimedTransition(200, "spit")
                ),
                new State("spit",
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile", damage: 28,
                        lifetimeMs: 1700, cooldownMS: 100000, coolDownOffset: 1000, size: 80),
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile", damage: 28,
                        lifetimeMs: 1700, count: 2, shootAngle: 16, cooldownMS: 100000, coolDownOffset: 1200, size: 80),
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile", damage: 28,
                        lifetimeMs: 1700, predictive: 0.2f, cooldownMS: 100000, coolDownOffset: 1600, size: 80),
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile", damage: 28,
                        lifetimeMs: 1700, count: 2, shootAngle: 24, cooldownMS: 100000, coolDownOffset: 2200, size: 80),
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile", damage: 28,
                        lifetimeMs: 1700, predictive: 0.2f, cooldownMS: 100000, coolDownOffset: 2800, size: 80),
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile", damage: 28,
                        lifetimeMs: 1700, count: 2, shootAngle: 16, cooldownMS: 100000, coolDownOffset: 3200, size: 80),
                    new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Fire Missile", damage: 28,
                        lifetimeMs: 1700, predictive: 0.1f, cooldownMS: 100000, coolDownOffset: 3800, size: 80),
                    new Wander(4.65f),
                    new TimedTransition(5000, "flame_ring")
                ),
                new State("flame_ring",
                    new Shoot(7, new LinePath(5.5f), targeted: true, projName: "Fire Missile", damage: 15,
                        lifetimeMs: 530, count: 30, shootAngle: 12, cooldownMS: 400, coolDownOffset: 600),
                    new Follow(4.65f, acquireRange: 9, distFromTarget: 1),
                    new Wander(4.65f),
                    new TimedTransition(3500, "pause")
                ),
                new State("pause",
                    new Wander(4.65f),
                    new TimedTransition(1000, "idle")
                )
            );

        [CharacterBehavior("Tawny Warg")]
        public static State TawnyWarg =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(3.4f, new LinePath(6f), targeted: true, projName: "Blade", damage: 11,
                    lifetimeMs: 600, cooldownMS: 1000),
                new Protect(7.38f, "Desert Werewolf", 14, 8,
                    5),
                new Follow(4.48f, acquireRange: 9, distFromTarget: 2),
                new Wander(2.94f)
            );

        [CharacterBehavior("Demon Warg")]
        public static State DemonWarg =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f)
                ),
                new Shoot(4.5f, new LinePath(6f), targeted: true, projName: "Red Fire", damage: 13,
                    lifetimeMs: 2000, cooldownMS: 1000, size: 65),
                new Protect(7.38f, "Desert Werewolf", 14, 8,
                    5),
                new Wander(2.94f)
            );

        [CharacterBehavior("Desert Werewolf")]
        public static State DesertWerewolf =>
            new(
                new CharacterLoot(
                    // new TierLoot(3, ItemType.Weapon, 0.2f),
                    // new TierLoot(4, ItemType.Weapon, 0.12f),
                    // new TierLoot(3, ItemType.Armor, 0.2f),
                    // new TierLoot(4, ItemType.Armor, 0.15f),
                    // new TierLoot(5, ItemType.Armor, 0.02f),
                    // new TierLoot(1, ItemType.Ring, 0.11f),
                    // new TierLoot(1, ItemType.Ability, 0.38f),
                    new ItemLoot("Magic Potion", 0.03f)
                ),
                new Spawn(null, 0, 0, group: "Wargs", maxSpawnsPerReset: 8, cooldownMs: 8000),
                new State("unharmed",
                    new Shoot(8, new LinePath(6f), targeted: true, projName: "Red Fire", damage: 16,
                        lifetimeMs: 2000, predictive: 0.3f, cooldownMS: 1000, coolDownOffset: 500, size: 80),
                    new Follow(3.475f, acquireRange: 10.5f, distFromTarget: 2.5f),
                    new Wander(3.475f),
                    new HpLessTransition(0.75f, "enraged")
                ),
                new State("enraged",
                    new Shoot(8, new LinePath(6f), targeted: true, projName: "Red Fire", damage: 16,
                        lifetimeMs: 2000, predictive: 0.3f, cooldownMS: 1000, coolDownOffset: 500, size: 80),
                    new Taunt("GRRRRAAGH!", probability: 0.7f),
                    new ChangeSize(20, 170),
                    new Flash(0xff0000, 0.4f, 5000),
                    new Follow(4.245f, acquireRange: 9, distFromTarget: 2),
                    new Wander(4.245f)
                )
            );
    }
}