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
        [CharacterBehavior("Arena Horseman Anchor")]
        public static State ArenaHorsemanAnchor =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible)
            );

        [CharacterBehavior("Arena Headless Horseman")]
        public static State ArenaHeadlessHorseman =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(),
                    new ItemLoot("Potion of Wisdom", 0.05f, 0.01f)
                ),
                new Spawn("Arena Horseman Anchor", 0, 0, minSpawnCount: 1, maxSpawnsPerReset: 1),
                new State("EverythingIsCool",
                    new HpLessTransition(0.1f, "End"),
                    new State("Circle",
                        new Shoot(
                            path: new LinePath(7.5f),
                            targeted: true,
                            projName: "Orange Pumpkin Blast",
                            damage: 85,
                            lifetimeMs: 2000,
                            maxRadius: 15,
                            count: 3,
                            shootAngle: 25,
                            cooldownMS: 1000, size: 140
                        ),
                        new Shoot(
                            path: new LinePath(6f),
                            targeted: true,
                            projName: "Orange Shot",
                            damage: 120,
                            lifetimeMs: 2500,
                            maxRadius: 15,
                            cooldownMS: 1000, size: 170
                        ),
                        new TimedTransition(8000, "Shoot")
                    ),
                    new State("Shoot",
                        new ReturnToSpawn(9.045f), // Speed adjusted
                        new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                        new Flash(0xF0E68C, 1, 6),
                        new Shoot(
                            path: new LinePath(7.5f),
                            targeted: true,
                            projName: "Horseman Blast",
                            damage: 70,
                            lifetimeMs: 1500,
                            maxRadius: 15,
                            count: 8,
                            cooldownMS: 1500, size: 150
                        ),
                        new Shoot(
                            path: new LinePath(6f),
                            targeted: true,
                            projName: "Orange Shot",
                            damage: 120,
                            lifetimeMs: 2500,
                            maxRadius: 15,
                            cooldownMS: 2500, size: 170
                        ),
                        new TimedTransition(6000, "Circle")
                    )
                ),
                new State("End",
                    new Follow(9.045f, acquireRange: 20, distFromTarget: 1),
                    new Wander(3.995f), // Speed adjusted
                    new Flash(0xF0E68C, 1, 1000),
                    new Shoot(
                        path: new LinePath(7.5f),
                        targeted: true,
                        projName: "Orange Pumpkin Blast",
                        damage: 85,
                        lifetimeMs: 2000,
                        maxRadius: 15,
                        count: 3,
                        shootAngle: 25,
                        cooldownMS: 1000, size: 140
                    ),
                    new Shoot(
                        path: new LinePath(6f),
                        targeted: true,
                        projName: "Orange Shot",
                        damage: 120,
                        lifetimeMs: 2500,
                        maxRadius: 15,
                        cooldownMS: 1000, size: 170
                    )
                ),
                new DropPortalOnDeath("Haunted Cemetery Portal", 0.5f)
            );

        [CharacterBehavior("White Demon")]
        public static State WhiteDemon =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(),
                    new ItemLoot("Potion of Attack", 0.05f, 0.01f)
                ),
                new DropPortalOnDeath("Abyss of Demons Portal", 0.25f),
                new Follow(5.29f, 7), // Speed adjusted
                new Wander(2.92f), // Speed adjusted
                new Shoot(
                    path: new LinePath(5f),
                    targeted: true,
                    projName: "White Demon Shot",
                    damage: 45,
                    lifetimeMs: 2000,
                    maxRadius: 10,
                    count: 3,
                    shootAngle: 20,
                    predictive: 1,
                    cooldownMS: 500, size: 60,
                    multiHit: true, armorPiercing: true
                ),
                new Spawn("White Demon", 0, 0, maxDensity: 3, cooldownMs: 60000)
            );

        [CharacterBehavior("Sprite God")]
        public static State SpriteGod =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Attack", 0.05f, 0.01f)
                ),
                new State("0",
                    new DropPortalOnDeath("Glowing Portal", 0.25f),
                    new Wander(2.22f), // Converted speed: 0.4 -> 2.22
                    new Shoot(path: new LinePath(7f), targeted: true, projName: "Purple Magic", damage: 100,
                        lifetimeMs: 2000,
                        maxRadius: 12, count: 4, shootAngle: 10, cooldownMS: 1000),
                    new Shoot(projectileIndex: 1, targeted: true, maxRadius: 10, predictive: 1, cooldownMS: 1000),
                    new Spawn("Sprite God", 0, 0, maxDensity: 3, cooldownMs: 60000),
                    new Spawn("Sprite Child", 0, 0, minSpawnCount: 5, maxSpawnCount: 5, maxSpawnsPerReset: 1)
                )
            );

        [CharacterBehavior("Sprite Child")]
        public static State SpriteChild =>
            new(
                new DropPortalOnDeath("Glowing Portal", 0.15f),
                new Protect(2.22f, "Sprite God", protectionRange: 1), // Speed converted: 0.4 -> 2.22
                new Wander(2.22f) // Speed converted: 0.4 -> 2.22
            );

        [CharacterBehavior("Medusa")]
        public static State Medusa =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Speed", 0.05f, 0.01f)
                ),
                new DropPortalOnDeath("Snake Pit Portal", 0.25f),
                new Follow(distFromTarget: 7, speed: 6.29f), // Speed converted: 1 -> 6.29
                new Wander(2.22f), // Speed converted: 0.4 -> 2.22
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Green Bolt", damage: 100, lifetimeMs: 2000,
                    maxRadius: 12, count: 5, shootAngle: 10, cooldownMS: 1000),
                new AOE(4, 150, range: 8, cooldownMs: 3000),
                new Spawn("Medusa", 0, 0, maxDensity: 3, cooldownMs: 60000)
            );

        [CharacterBehavior("Ent God")]
        public static State EntGod =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Defense", 0.05f, 0.01f)
                ),
                new Follow(distFromTarget: 7, speed: 6.29f), // Speed converted: 1.0 -> 6.29
                new Wander(2.22f), // Speed converted: 0.4 -> 2.22
                new Shoot(path: new LinePath(8f), targeted: true, projName: "Fire Bolt", damage: 70, lifetimeMs: 2000,
                    maxRadius: 12, count: 5, shootAngle: 10, predictive: 1, cooldownMS: 1250, multiHit: true),
                new Spawn("Ent God", 0, 0, maxDensity: 3, cooldownMs: 60000)
            );

        [CharacterBehavior("Beholder")]
        public static State Beholder =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Defense", 0.05f, 0.01f)
                ),
                new Follow(distFromTarget: 7, speed: 6.29f), // Speed converted: 1.0 -> 6.29
                new Wander(2.22f), // Speed converted: 0.4 -> 2.22
                new Shoot(path: new LinePath(5f), targeted: true, projName: "White Bolt", damage: 120, lifetimeMs: 2700,
                    maxRadius: 12, count: 5, shootAngle: 72, predictive: 1, cooldownMS: 750, size: 120),
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Purple Star", damage: 0, lifetimeMs: 3000,
                    maxRadius: 10, predictive: 1, cooldownMS: 1000, effects: (ConditionEffectIndex.Blind, 5000)),
                new Spawn("Beholder", 0, 0, maxDensity: 3, cooldownMs: 60000)
            );

        [CharacterBehavior("Flying Brain")]
        public static State FlyingBrain =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Attack", 0.05f, 0.01f)
                ),
                new Follow(distFromTarget: 7, speed: 6.29f), // Speed converted: 1.0 -> 6.29
                new Wander(2.22f), // Speed converted: 0.4 -> 2.22
                new Shoot(path: new LinePath(12f), targeted: true, projName: "Pink Bolt", damage: 50, lifetimeMs: 1800,
                    maxRadius: 12, count: 5, shootAngle: 72, cooldownMS: 500),
                new Spawn("Flying Brain", 0, 0, maxDensity: 3, cooldownMs: 60000),
                new DropPortalOnDeath("Mad Lab Portal", 0.17f)
            );

        [CharacterBehavior("Slime God")]
        public static State SlimeGod =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Defense", 0.05f, 0.01f)
                ),
                new Follow(distFromTarget: 7, speed: 6.29f), // Speed converted: 1.0 -> 6.29
                new Wander(2.22f), // Speed converted: 0.4 -> 2.22
                new Shoot(path: new LinePath(10f), targeted: true, projName: "Red Fire", damage: 80, lifetimeMs: 1100,
                    maxRadius: 12, count: 5, shootAngle: 10, predictive: 1, cooldownMS: 1000),
                new Shoot(path: new LinePath(7f), targeted: true, projName: "Green Star", damage: 0, lifetimeMs: 2000,
                    maxRadius: 10, predictive: 1, cooldownMS: 650, effects: (ConditionEffectIndex.Slowed, 6000)),
                new Spawn("Slime God", 0, 0, maxDensity: 2, cooldownMs: 60000),
                new DropPortalOnDeath("Toxic Sewers Portal", 0.3f)
            );

        [CharacterBehavior("Ghost God")]
        public static State GhostGod =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Speed", 0.05f, 0.01f)
                ),
                new Follow(distFromTarget: 7, speed: 6.29f), // Speed converted: 1.0 -> 6.29
                new Wander(2.22f), // Speed converted: 0.4 -> 2.22
                new Shoot(path: new LinePath(5f), targeted: true, projName: "White Bolt", damage: 120, lifetimeMs: 2700,
                    maxRadius: 12, count: 7, shootAngle: 25, predictive: 1, cooldownMS: 900, size: 110),
                new Spawn("Ghost God", 0, 0, maxDensity: 3, cooldownMs: 60000),
                new DropPortalOnDeath("Undead Lair Portal", 0.25f)
            );

        [CharacterBehavior("Rock Bot")]
        public static State RockBot =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Dexterity", 0.05f, 0.01f)
                ),
                new Spawn("Paper Bot", 0, 0, maxSpawnsPerReset: 1, cooldownMs: 10000),
                new Spawn("Steel Bot", 0, 0, maxSpawnsPerReset: 1, cooldownMs: 10000),
                new Swirl(4.07f, 3, targeted: false),
                new State("Waiting",
                    new EntityWithinTransition("Attacking", radius: 15)
                ),
                new State("Attacking",
                    new Shoot(8, new LinePath(7f), targeted: true, projName: "White Bullet", damage: 80,
                        lifetimeMs: 2000, cooldownMS: 2000, size: 80, multiHit: true),
                    new HealGroup(8, "Papers", 1000),
                    new Taunt("We are impervious to non-mystic attacks!", probability: 0.5f),
                    new TimedTransition(10000, "Waiting")
                )
            );

        [CharacterBehavior("Paper Bot")]
        public static State PaperBot =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Vitality", 0.05f, 0.01f)
                ),
                new DropPortalOnDeath("Puppet Theatre Portal", 0.15f),
                new Orbit(2.94f, 3, target: "Rock Bot"),
                new Wander(2.94f),
                new State("Idle",
                    new EntityWithinTransition("Attack", radius: 15)
                ),
                new State("Attack",
                    new Shoot(8, new LinePath(3f), targeted: true, projName: "White Bullet", damage: 50,
                        lifetimeMs: 4000, count: 3, shootAngle: 20, cooldownMS: 800, size: 50, multiHit: true),
                    new HealGroup(8, "Steels", 1000),
                    new EntityNotWithinTransition("Idle", radius: 30),
                    new HpLessTransition(0.2f, "Explode")
                ),
                new State("Explode",
                    new Shoot(10, new LinePath(3f), targeted: false, projName: "White Bullet", damage: 50,
                        lifetimeMs: 4000, count: 10, shootAngle: 36, fixedAngle: 0, cooldownMS: 1000, size: 50,
                        multiHit: true),
                    new Suicide()
                )
            );

        [CharacterBehavior("Steel Bot")]
        public static State SteelBot =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Speed", 0.05f, 0.01f)
                ),
                new Orbit(2.94f, 3, target: "Rock Bot"),
                new Wander(2.94f),
                new State("Idle",
                    new EntityWithinTransition("Attack", radius: 15)
                ),
                new State("Attack",
                    new Shoot(8, new LinePath(3f), targeted: true, projName: "White Bullet", damage: 50,
                        lifetimeMs: 4000, count: 3, shootAngle: 20, cooldownMS: 800, size: 50, multiHit: true),
                    new HealGroup(8, "Rocks", 1000),
                    new Taunt("Silly squishy. We heal our brothers in a circle.", probability: 0.5f),
                    new EntityNotWithinTransition("Idle", radius: 30),
                    new HpLessTransition(0.2f, "Explode")
                ),
                new State("Explode",
                    new Shoot(10, new LinePath(3f), targeted: false, projName: "White Bullet", damage: 50,
                        lifetimeMs: 4000, count: 10, shootAngle: 36, fixedAngle: 0, cooldownMS: 1000, size: 50,
                        multiHit: true),
                    new Suicide()
                )
            );

        [CharacterBehavior("Djinn")]
        public static State Djinn =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Speed", 0.05f, 0.01f)
                ),
                new DropPortalOnDeath("Treasure Cave Portal", 0.25f),
                new State("Idle",
                    new Wander(2.94f),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Spawn("Djinn", 0, 0, maxDensity: 3, densityRadius: 20, cooldownMs: 60000),
                    new EntityWithinTransition("Attacking", radius: 8)
                ),
                new State("Attacking",
                    new State("Bullet",
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 90, coolDownOffset: 0,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 100, coolDownOffset: 200,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 110, coolDownOffset: 400,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 120, coolDownOffset: 600,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 130, coolDownOffset: 800,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 140,
                            coolDownOffset: 1000, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 150,
                            coolDownOffset: 1200, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 160,
                            coolDownOffset: 1400, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 170,
                            coolDownOffset: 1600, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 180,
                            coolDownOffset: 1800, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 8, cooldownMS: 10000, fixedAngle: 180,
                            coolDownOffset: 2000, shootAngle: 45, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 180, coolDownOffset: 0,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 170, coolDownOffset: 200,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 160, coolDownOffset: 400,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 150, coolDownOffset: 600,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 140, coolDownOffset: 800,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 130,
                            coolDownOffset: 1000, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 120,
                            coolDownOffset: 1200, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 110,
                            coolDownOffset: 1400, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 100,
                            coolDownOffset: 1600, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 90, coolDownOffset: 1800,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 8, cooldownMS: 10000, fixedAngle: 90, coolDownOffset: 2000,
                            shootAngle: 22.5f, targeted: false),
                        new TimedTransition(2000, "Wait")
                    ),
                    new State("Wait",
                        new Follow(4.07f, 0.5f),
                        new Flash(0x00ff00, 0.1f, 20),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(2000, "Bullet")
                    ),
                    new EntityNotWithinTransition("Idle", radius: 13),
                    new HpLessTransition(0.5f, "FlashBeforeExplode")
                ),
                new State("FlashBeforeExplode",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0xff0000, 0.3f, 3),
                    new TimedTransition(1000, "Explode")
                ),
                new State("Explode",
                    new Shoot(10, projectileIndex: 0, count: 10, shootAngle: 36, fixedAngle: 0, targeted: false,
                        cooldownMS: 1000),
                    new Suicide()
                )
            );

        [CharacterBehavior("Lucky Djinn")]
        public static State LuckyDjinn =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Defense", 0.05f, 0.01f)
                ),
                // new ScaleHP2(30),
                new DropPortalOnDeath("The Crawling Depths", 1),
                new State("Idle",
                    new Wander(2.94f),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntityWithinTransition("Attacking", radius: 8)
                ),
                new State("Attacking",
                    new State("Bullet",
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 90, coolDownOffset: 0,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 100, coolDownOffset: 200,
                            shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 110, coolDownOffset: 400, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 120, coolDownOffset: 600, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 130, coolDownOffset: 800, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 140, coolDownOffset: 1000, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 150, coolDownOffset: 1200, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 160, coolDownOffset: 1400, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 170, coolDownOffset: 1600, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 180, coolDownOffset: 1800, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 8, cooldownMS: 10000, fixedAngle: 180, coolDownOffset: 2000, shootAngle: 45, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 180, coolDownOffset: 0, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 170, coolDownOffset: 200, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 160, coolDownOffset: 400, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 150, coolDownOffset: 600, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 140, coolDownOffset: 800, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 130, coolDownOffset: 1000, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 120, coolDownOffset: 1200, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 110, coolDownOffset: 1400, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 100, coolDownOffset: 1600, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 4, cooldownMS: 10000, fixedAngle: 90, coolDownOffset: 1800, shootAngle: 90, targeted: false),
                        new Shoot(10, projectileIndex: 0, count: 8, cooldownMS: 10000, fixedAngle: 90, coolDownOffset: 2000, shootAngle: 22.5f, targeted: false),
                        new TimedTransition(2000, "Wait")
                    ),
                    new State("Wait",
                        new Follow(4.07f, 0.5f),
                        new Flash(0x00ff00, 0.1f, 20),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(2000, "Bullet")
                    ),
                    new EntityNotWithinTransition("Idle", radius: 13),
                    new HpLessTransition(0.5f, "FlashBeforeExplode")
                ),
                new State("FlashBeforeExplode",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0xff0000, 0.3f, 3),
                    new TimedTransition(1000, "Explode")
                ),
                new State("Explode",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(10, projectileIndex: 0, count: 10, shootAngle: 36, fixedAngle: 0, targeted: false, cooldownMS: 1000),
                    new Suicide()
                )
            );

        [CharacterBehavior("Lucky Ent God")]
        public static State LuckyEntGod =>
            new(
                new CharacterLoot(
                    // new TierLoot(5, ItemType.Weapon, 0.08),
                    // new TierLoot(6, ItemType.Weapon, 0.04),
                    // new TierLoot(7, ItemType.Weapon, 0.02),
                    // new TierLoot(8, ItemType.Weapon, 0.01),
                    // new TierLoot(5, ItemType.Armor, 0.08),
                    // new TierLoot(6, ItemType.Armor, 0.04),
                    // new TierLoot(7, ItemType.Armor, 0.02),
                    // new TierLoot(8, ItemType.Armor, 0.01),
                    // new TierLoot(3, ItemType.Ring, 0.05),
                    // new TierLoot(4, ItemType.Ring, 0.025),
                    // new TierLoot(3, ItemType.Ability, 0.08),
                    // new TierLoot(4, ItemType.Ability, 0.04),
                    new ItemLoot("Potion of Defense", 0.025f, 0.18f)
                ),
                // new ScaleHP2(20),
                new DropPortalOnDeath("Woodland Labyrinth", 1),
                new Follow(distFromTarget: 7, speed: 6.29f),
                new Wander(2.94f),
                new Shoot(12, new LinePath(8f), targeted: true, projName: "Fire Bolt", damage: 100, lifetimeMs: 2000,
                    count: 5, shootAngle: 10, predictive: 1, cooldownMS: 1250, multiHit: true)
            );

        [CharacterBehavior("Leviathan")]
        public static State Leviathan =>
            new(
                new CharacterLoot(
                    // LootTemplates.MountainDrop(0.01f),
                    new ItemLoot("Potion of Defense", 0.05f, 0.01f)
                ),
                new DropPortalOnDeath("Puppet Theatre Portal", 0.25f),
                new State("Wander",
                    new Swirl(),
                    new Shoot(10, new LinePath(8.5f), targeted: true, projName: "Blue Missile", damage: 90,
                        lifetimeMs: 1800, count: 2, shootAngle: 10, cooldownMS: 500),
                    new TimedTransition(5000, "Triangle")
                ),
                new State("Triangle",
                    new State("1",
                        new MoveLine(4.07f, 40),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 34, cooldownMS: 300),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 38, cooldownMS: 300),
                        new Shoot(1, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 42, cooldownMS: 300),
                        new Shoot(1, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 46, cooldownMS: 300),
                        new TimedTransition(1500, "2")
                    ),
                    new State("2",
                        new MoveLine(4.07f, 160),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 94, cooldownMS: 300),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 98, cooldownMS: 300),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 102, cooldownMS: 300),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 106, cooldownMS: 300),
                        new TimedTransition(1500, "3")
                    ),
                    new State("3",
                        new MoveLine(4.07f, 280),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 274, cooldownMS: 300),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 278, cooldownMS: 300),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 282, cooldownMS: 300),
                        new Shoot(10, new LinePath(6.7f), targeted: false, projName: "Aqua Missile", damage: 70,
                            lifetimeMs: 1700, count: 3, shootAngle: 120, fixedAngle: 286, cooldownMS: 300),
                        new TimedTransition(1500, "Wander")
                    )
                )
            );
    }
}