#region

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
        [CharacterBehavior("Pirate")]
        public static State Pirate =>
            new(
                new CharacterLoot(
                    // new TierLoot(1, ItemType.Weapon, 0.2),
                    new ItemLoot("Health Potion", 0.03f)
                ),
                new Shoot(3, new LinePath(4f), targeted: true, projName: "Blade", damage: 4,
                    lifetimeMs: 600, cooldownMS: 2500),
                new Follow(distFromTarget: 1, speed: 5.46f),
                new Wander(2.94f)
            );

        [CharacterBehavior("Piratess")]
        public static State Piratess =>
            new(
                new CharacterLoot(
                    // new TierLoot(1, ItemType.Armor, 0.2),
                    new ItemLoot("Health Potion", 0.03f)
                ),
                new Follow(6.825f, 1, cooldownOffsetMS: 3000, cooldownMS: 1500),
                new Wander(4.07f),
                new Shoot(3, new LinePath(4f), targeted: true, projName: "Blade", damage: 4,
                    lifetimeMs: 600, cooldownMS: 2500),
                new Spawn("Pirate", 0, 0, 60000, maxDensity: 5, densityRadius: 40),
                new Spawn("Piratess", 0, 0, 60000, maxDensity: 5, densityRadius: 40)
            );

        [CharacterBehavior("Snake")]
        public static State Snake =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new Wander(2.94f),
                new Shoot(10, new LinePath(4f), targeted: true, projName: "Green Magic", damage: 4,
                    lifetimeMs: 2000, cooldownMS: 2000),
                new Spawn("Snake", 0, 0, 60000, maxDensity: 5, densityRadius: 40)
            );

        [CharacterBehavior("Poison Scorpion")]
        public static State PoisonScorpion =>
            new(
                new Protect(2.94f, "Scorpion Queen"),
                new Wander(2.94f),
                new Shoot(8, new LinePath(4f), targeted: true, projName: "Green Magic", damage: 7,
                    lifetimeMs: 2000, cooldownMS: 2000)
            );

        [CharacterBehavior("Scorpion Queen")]
        public static State ScorpionQueen =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.03f),
                    new ItemLoot("Magic Potion", 0.02f)
                ),
                new ChangeSize(100, 200),
                new Wander(1.84f),
                new Spawn("Poison Scorpion", 0, 0, 10000, maxDensity: 10, densityRadius: 40),
                new Spawn("Scorpion Queen", 0, 0, 60000, maxDensity: 2, densityRadius: 40)
            );

        [CharacterBehavior("Bandit Enemy")]
        public static State BanditEnemy =>
            new(
                new State("fast_follow",
                    new Shoot(3, new LinePath(6f), targeted: true, projName: "Blade", damage: 9,
                        lifetimeMs: 600, cooldownMS: 1000, size: 60),
                    new Protect(4.07f, "Bandit Leader", 9, 7,
                        3),
                    new Follow(6.29f, 1),
                    new Wander(4.07f),
                    new TimedTransition(3000, "scatter1")
                ),
                new State("scatter1",
                    new Protect(4.07f, "Bandit Leader", 9, 7,
                        3),
                    new Wander(6.29f),
                    new Wander(4.07f),
                    new TimedTransition(2000, "slow_follow")
                ),
                new State("slow_follow",
                    new Shoot(4.5f, new LinePath(6f), targeted: true, projName: "Blade", damage: 9,
                        lifetimeMs: 600, cooldownMS: 1000, size: 60),
                    new Protect(4.07f, "Bandit Leader", 9, 7,
                        3),
                    new Follow(3.47f, acquireRange: 9, distFromTarget: 3.5f, cooldownOffsetMS: 4000),
                    new Wander(3.47f),
                    new TimedTransition(3000, "scatter2")
                ),
                new State("scatter2",
                    new Protect(4.07f, "Bandit Leader", 9, 7,
                        3),
                    new Wander(6.29f),
                    new Wander(4.07f),
                    new TimedTransition(2000, "fast_follow")
                ),
                new State("escape",
                    new StayAwayFrom(3.47f, 8),
                    new TimedTransition(15000, "fast_follow")
                )
            );

        [CharacterBehavior("Bandit Leader")]
        public static State BanditLeader =>
            new(
                new CharacterLoot(
                    // new TierLoot(1, ItemType.Weapon, 0.2),
                    // new TierLoot(1, ItemType.Armor, 0.2),
                    // new TierLoot(2, ItemType.Weapon, 0.12),
                    // new TierLoot(2, ItemType.Armor, 0.12),
                    new ItemLoot("Health Potion", 0.12f),
                    new ItemLoot("Magic Potion", 0.14f)
                ),
                new Spawn("Bandit Enemy", 0, 0, 8000, maxSpawnsPerReset: 4),
                new State("bold",
                    new State("warn_about_grenades",
                        new Taunt("Catch!", probability: 0.15f),
                        new TimedTransition(400, "wimpy_grenade1")
                    ),
                    new State("wimpy_grenade1",
                        new AOE(1.4f, 12, 10000),
                        new Wander(2.38f),
                        new TimedTransition(2000, "wimpy_grenade2")
                    ),
                    new State("wimpy_grenade2",
                        new AOE(1.4f, 12, 10000),
                        new Wander(3.47f),
                        new TimedTransition(3000, "slow_follow")
                    ),
                    new State("slow_follow",
                        new Shoot(13, new LinePath(6f), targeted: true, projName: "Blade", damage: 9,
                            lifetimeMs: 800, cooldownMS: 1000, size: 60),
                        new Follow(2.94f, acquireRange: 9, distFromTarget: 3.5f, cooldownOffsetMS: 4000),
                        new Wander(2.94f),
                        new TimedTransition(4000, "warn_about_grenades")
                    ),
                    new HpLessTransition(0.45f, "meek")
                ),
                new State("meek",
                    new Taunt("Forget this... run for it!", probability: 0.45f),
                    new StayAwayFrom(3.47f, 6),
                    new Order(10, "Bandit Enemy", "escape"),
                    new TimedTransition(12000, "bold")
                )
            );

        [CharacterBehavior("Red Gelatinous Cube")]
        public static State RedGelatinousCube =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f),
                    new ItemLoot("Magic Potion", 0.04f)
                ),
                new Shoot(8, new LinePath(6f), targeted: true, projName: "Salmon Missile", damage: 9,
                    lifetimeMs: 2000, count: 2, shootAngle: 10, predictive: 0.2f, cooldownMS: 1000),
                new Wander(2.94f),
                new Spawn("Red Gelatinous Cube", 0, 0, 60000, maxDensity: 5, densityRadius: 40),
                new DropPortalOnDeath("Pirate Cave Portal", 0.01f)
            );

        [CharacterBehavior("Purple Gelatinous Cube")]
        public static State PurpleGelatinousCube =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f),
                    new ItemLoot("Magic Potion", 0.04f)
                ),
                new Shoot(8, new LinePath(6.5f), targeted: true, projName: "Purple Magic", damage: 12,
                    lifetimeMs: 2000, predictive: 0.2f, cooldownMS: 600),
                new Wander(2.94f),
                new Spawn("Purple Gelatinous Cube", 0, 0, 60000, maxDensity: 5, densityRadius: 40),
                new DropPortalOnDeath("Pirate Cave Portal", 0.01f)
            );

        [CharacterBehavior("Green Gelatinous Cube")]
        public static State GreenGelatinousCube =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.04f),
                    new ItemLoot("Magic Potion", 0.04f)
                ),
                new Shoot(8, new LinePath(6f), targeted: true, projName: "Green Magic", damage: 10,
                    lifetimeMs: 2000, count: 5, shootAngle: 72, predictive: 0.2f, cooldownMS: 1800),
                new Wander(2.94f),
                new Spawn("Green Gelatinous Cube", 0, 0, 60000, maxDensity: 5, densityRadius: 40),
                new DropPortalOnDeath("Pirate Cave Portal", 0.01f)
            );
    }
}