#region
using Common;
using Common.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;
using GameServer.Game.Entities.Loot;
namespace GameServer.Game.Entities.Behaviors.Library
#endregion

{
    public partial class BehaviorLib
    {
        [CharacterBehavior("Malphas Protector")]
        public static State MalphasProtector =>
            new (
                new CharacterLoot(
                    new ItemLoot("Magic Potion", 0.06f, 0.01f),
                    new ItemLoot("Admin Staff", 1f, 0.01f)
                ),
                new State("0",
                    new Shoot(path: new LinePath(8), targeted: true, maxRadius: 5, count: 3, shootAngle: 5,
                        projName: "Red Fire", predictive: 0.45f, cooldownMS: 1200, damage: 50, lifetimeMs: 600),
                    new Orbit(3.2f, 9, 20, "Archdemon Malphas", 0,
                        0, true)
                )
            );

        [CharacterBehavior("Brute Warrior of the Abyss")]
        public static State BruteWarriorAbyss =>
            new (
                new CharacterLoot(
                    new ItemLoot("Glass Sword", 0.01f, 0.5f),
                    new ItemLoot("Ring of Greater Dexterity", 0.01f, 0.5f),
                    new ItemLoot("Magesteel Quiver", 0.01f, 0.5f)
                ),
                new State(
                    "0", // Speed from old sources needs to be transformed into tiles/sec like this: spd = spd * 5.55f + 0.74f
                    new Follow(4.45f, 1, 8),
                    new Wander(2.12f),
                    new Shoot(path: new LinePath(8), targeted: true, projName: "Blade", damage: 80, lifetimeMs: 400,
                        maxRadius: 8, count: 3, shootAngle: 10, cooldownMS: 500)
                )
            );

        [CharacterBehavior("Imp of the Abyss")]
        public static State ImpoftheAbyss =>
            new (
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.1f),
                    new ItemLoot("Magic Potion", 0.1f),
                    new ItemLoot("Cloak of the Red Agent", 0.01f, 0.5f)
                ),
                new State("0",
                    new Wander(5.6f),
                    new Shoot(path: new LinePath(7), targeted: true, projName: "Fire Bolt", damage: 60,
                        lifetimeMs: 3000, maxRadius: 8, count: 5, shootAngle: 10, cooldownMS: 1000)
                )
            );

        [CharacterBehavior("Demon of the Abyss")]
        public static State DemonoftheAbyss =>
            new (
                new CharacterLoot(
                    new ItemLoot("Fire Bow", 0.05f),
                    new ItemLoot("Mithril Armor", 0.01f, 0.5f)
                ),
                new State("0",
                    new Follow(6.3f, 5, 8),
                    new Wander(2.12f),
                    new Shoot(path: new LinePath(5), targeted: true, projName: "Red Fire", damage: 120,
                        lifetimeMs: 3000, maxRadius: 8, count: 3, shootAngle: 10, cooldownMS: 1000, size: 140)
                )
            );

        [CharacterBehavior("Demon Warrior of the Abyss")]
        public static State DemonWarrioroftheAbyss =>
            new (
                new CharacterLoot(
                    new ItemLoot("Fire Sword", 0.025f),
                    new ItemLoot("Steel Shield", 0.025f)
                ),
                new State("0",
                    new Follow(6.3f, 5, 8),
                    new Wander(2.12f),
                    new Shoot(path: new LinePath(6), targeted: true, projName: "Red Fire", damage: 80, lifetimeMs: 3000,
                        maxRadius: 8, count: 3, shootAngle: 10, cooldownMS: 1000)
                )
            );

        [CharacterBehavior("Demon Mage of the Abyss")]
        public static State DemonMageoftheAbyss =>
            new (
                new CharacterLoot(
                    new ItemLoot("Fire Nova Spell", 0.02f),
                    new ItemLoot("Wand of Dark Magic", 0.01f, 0.1f),
                    new ItemLoot("Avenger Staff", 0.01f, 0.1f),
                    new ItemLoot("Robe of the Invoker", 0.01f, 0.1f),
                    new ItemLoot("Essence Tap Skull", 0.01f, 0.1f),
                    new ItemLoot("Demonhunter Trap", 0.01f, 0.1f)
                ),
                new State("0",
                    new Follow(6.3f, 5, 8),
                    new Wander(2.12f),
                    new Shoot(path: new LinePath(8), targeted: true, projName: "Red Fire", damage: 80, lifetimeMs: 3000,
                        maxRadius: 8, count: 3, shootAngle: 10, cooldownMS: 1000)
                )
            );

        [CharacterBehavior("Brute of the Abyss")]
        public static State BruteoftheAbyss =>
            new (
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.1f),
                    new ItemLoot("Obsidian Dagger", 0.02f, 0.1f),
                    new ItemLoot("Steel Helm", 0.02f, 0.1f)
                ),
                new State("0",
                    new Follow(9f, 1, 8),
                    new Wander(2.12f),
                    new Shoot(path: new LinePath(12), targeted: true, projName: "Blade", damage: 80, lifetimeMs: 400,
                        maxRadius: 8, count: 3, shootAngle: 10, cooldownMS: 500)
                )
            );

        [CharacterBehavior("Malphas Missile")]
        public static State MalphasMissile =>
            new (
                new State("Start",
                    new TimedTransition(50, "Attacking")
                ),
                new State("Attacking",
                    new Follow(6.84f, acquireRange: 10, distFromTarget: 0.2f),
                    new EntityWithinTransition("FlashBeforeExplode", "Player", 1.3f),
                    new TimedTransition(5000, "FlashBeforeExplode")
                ),
                new State("FlashBeforeExplode",
                    new Flash(0xFFFFFF, 0.1, 6),
                    new TimedTransition(600, "Explode")
                ),
                new State("Explode",
                    new Shoot(path: new LinePath(6.6f), targeted: true, projName: "Red Star", damage: 40,
                        lifetimeMs: 760, maxRadius: 12f, count: 8, shootAngle: 45, fixedAngle: 0, cooldownMS: 1000,
                        multiHit: true),
                    new Suicide()
                )
            );

        [CharacterBehavior("Malphas Flamer")]
        public static State MalphasFlamer =>
            new (
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.1f, 0.01f),
                    new ItemLoot("Magic Potion", 0.1f, 0.01f)
                ),
                new State("Attacking",
                    new State("Charge",
                        new Follow(4.6f, acquireRange: 10, distFromTarget: 0.1f),
                        new EntityWithinTransition(target: "player", radius: 2, targetState: "Bullet1")
                    ),
                    new State("Bullet1",
                        new Flash(0xFFAA00, 0.2, 20),
                        new Shoot(path: new AmplitudePath(8f, 0.3f, 0.5f, lifetimeMs: 500), targeted: true, projName: "White Flame",
                            damage: 20, lifetimeMs: 500, maxRadius: 8, cooldownMS: 200, multiHit: true),
                        new TimedTransition(4000, "Wait1")
                    ),
                    new State("Wait1",
                        new Charge(17.5f, 20, 600)
                    ),
                    new HpLessTransition(0.2f, "FlashBeforeExplode")
                ),
                new State("FlashBeforeExplode",
                    new Flash(0xFF0000, 0.75, 1),
                    new TimedTransition(300, "Explode")
                ),
                new State("Explode",
                    new Shoot(path: new AmplitudePath(8f, 0.3f, 0.5f, lifetimeMs: 500), targeted: true, projName: "White Flame",
                        damage: 20, lifetimeMs: 500, maxRadius: 10f, count: 8, shootAngle: 45, fixedAngle: 0,
                        cooldownMS: 1000, multiHit: true),
                    new Suicide()
                )
            );
        [CharacterBehavior("AbyssTemporaryLava")]
        public static State AbyssTemporaryLava =>
            new State(
                new State("lavaWAIT",
                    new EntityWithinTransition(target: "AbyssTransition", targetState: "lava", radius: 30)
                ),
                new State("lava",
                    new ChangeGround(["TempRed Quad"], ["RedQuadLava"], 1),
                    new ChangeGround(["TempRed Checker Board"], ["RedCheckerBoardLava"], 1),
                    new TimedTransition(1000, "lava2")
                ),
                new State("lava2",
                    new ChangeGround(["TempRed Quad"], ["RedQuadLava"], 3),
                    new ChangeGround(["TempRed Checker Board"], ["RedCheckerBoardLava"], 3),
                    new TimedTransition(1000, "lava3")
                ),
                new State("lava3",
                    new ChangeGround(["TempRed Quad"], ["RedQuadLava"], 5),
                    new ChangeGround(["TempRed Checker Board"], ["RedCheckerBoardLava"], 5),
                    new TimedTransition(1000, "lava4")
                ),
                new State("lava4",
                    new ChangeGround(["TempRed Quad"], ["RedQuadLava"], 7),
                    new ChangeGround(["TempRed Checker Board"], ["RedCheckerBoardLava"], 7),
                    new EntitiesNotWithinTransition(targets: ["Archdemon Malphas", "Abyss Idol"], targetStates: "gg1", radius: 30)
                ),
                new State("gg1",
                    new ChangeGround(["RedQuadLava"], ["TempRed Quad"], 1),
                    new ChangeGround(["RedCheckerBoardLava"], ["TempRed Checker Board"], 1),
                    new TimedTransition(1000, "gg2")
                ),
                new State("gg2",
                    new ChangeGround(["RedQuadLava"], ["TempRed Quad"], 3),
                    new ChangeGround(["RedCheckerBoardLava"], ["TempRed Checker Board"], 3),
                    new TimedTransition(1000, "gg3")
                ),
                new State("gg3",
                    new ChangeGround(["RedQuadLava"], ["TempRed Quad"], 5),
                    new ChangeGround(["RedCheckerBoardLava"], ["TempRed Checker Board"], 5),
                    new TimedTransition(1000, "gg4")
                ),
                new State("gg4",
                    new ChangeGround(["RedQuadLava"], ["TempRed Quad"], 7),
                    new ChangeGround(["RedCheckerBoardLava"], ["TempRed Checker Board"], 7),
                    new Suicide(500)
                )
            );
        [CharacterBehavior("AbyssAnchor")]
        public static State AbyssAnchor =>
            new State(
                new State("p1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition(target: "Archdemon Malphas", targetState: "p2", radius: 20)
                ),
                new State("p2",
                    new Suicide(500)
                )
            );
        [CharacterBehavior("AbyssTransition")]
        public static State AbyssTransition =>
            new State(
                new State("p1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition(target: "Archdemon Malphas", targetState: "p2", radius: 20)
                ),
                new State("p2",
                    new Suicide(500)
                )
            );

        [CharacterBehavior("Archdemon Malphas")]
        public static State ArchdemonMalphas =>
            new (
                new CharacterLoot( // Need to implement better loot handling
                    // new TierLoot(10, ItemType.Weapon, .07),
                    // new TierLoot(11, ItemType.Weapon, .07),
                    // new TierLoot(4, ItemType.Ability, .07),
                    // new TierLoot(5, ItemType.Ability, .07),
                    // new TierLoot(11, ItemType.Armor, .07),
                    // new TierLoot(12, ItemType.Armor, .07),
                    // new TierLoot(4, ItemType.Ring, .07),
                    // new TierLoot(5, ItemType.Ring, .07)
                    new ItemLoot("Demon Blade", 0.7f, 0.01f)
                ),
                new DropPortalOnDeath("Realm Portal"),
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntityWithinTransition(target: "player", radius: 8, targetState: "start2")
                ),
                new State("start2",
                    new Taunt(text: "You’ve walked into Hell itself, fool!", coolDownMS: 5000),
                    new TossObject("AbyssAnchor", range: 0, angle: 0, cooldownMS: 50000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0xF73626, 0.5F, 4),
                    new TimedTransition(2000, "shoot1")
                ),
                new State("shoot1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 2000),
                    new Shoot(15,
                        new ProjectilePath(1600, new LinePath(6.5F))
                            .Then(6000, new LinePath(0))
                            .Then(1600, new LinePath(-6.5F)),
                        count: 10,
                        projName: "Science Fire Ball",
                        targeted: false,
                        cooldownMS: 8000,
                        fixedAngle: 110,
                        armorPiercing: true,
                        multiHit: true,
                        damage: 100,
                        shootAngle: 36),
                    new Shoot(15,
                        new ProjectilePath(1600, new LinePath(7.5F))
                            .Then(6000, new LinePath(0))
                            .Then(1600, new LinePath(-7.5F)),
                        count: 10,
                        projName: "Science Fire Ball",
                        targeted: false,
                        cooldownMS: 8000,
                        fixedAngle: 90,
                        armorPiercing: true,
                        multiHit: true,
                        damage: 100,
                        shootAngle: 36),
                    new Follow(3, 2, 10, cooldownOffsetMS: 1000),
                    new Protect(speed: 3, "AbyssAnchor", acquireRange: 10, protectionRange: 6),
                    new Shoot(15,
                        new ProjectilePath(800, new LinePath(6.5F))
                            .Then(2000, new DeceleratePath(4)),
                        count: 4,
                        shootAngle: 10,
                        projName: "Wakizashi Fire",
                        targeted: true,
                        cooldownMS: 1000,
                        coolDownOffset: 1500,
                        damage: 50),
                    new Shoot(15,
                        new ProjectilePath(200, new LinePath(6.5F))
                            .Then(2000, new DeceleratePath(4)),
                        count: 8,
                        size: 120,
                        shootAngle: 45,
                        projName: "Demon Fire Blast",
                        targeted: false,
                        cooldownMS: 2000,
                        coolDownOffset: 2500,
                        damage: 60,
                        multiHit: true),
                    new HpLessTransition(0.5f, "return")
                ),
                new State("return",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new ReturnToSpawn(3, 0),
                    new TimedTransition(3000, "lava")
                ),
                new State("lava",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TossObject("AbyssTransition", range: 0, angle: 0, cooldownMS: 50000),
                    new TimedTransition(2000, "portal")
                ),
                new State("portal",
                    new Taunt(text: "The lava is warmer than your courage!", coolDownMS: 5000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Spawn("MalphasPortal", cooldownMs: 20000, maxDensity: 1, densityRadius: 20),
                    new TimedTransition(2000, "shoot2")
                ),
                new State("shoot2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(15,
                        new ProjectilePath(1000, new LinePath(6.5F))
                            .Then(2000, new AmplitudePath(1, 0.2F, 0.1F))
                            .Then(8000, new AmplitudePath(-1, 8, 0.5F)),
                        count: 8,
                        shootAngle: 45,
                        projName: "Science Fire Ball",
                        targeted: false,
                        cooldownMS: 3000,
                        coolDownOffset: 500,
                        damage: 60,
                        multiHit: true),
                    new EntityNotWithinTransition(target: "MalphasPortal", radius: 20, targetState: "size")
                ),
                new State("size",
                    new Taunt(text: "This realm shall burn with my final breath!", coolDownMS: 5000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new ChangeSize(4, 200),
                    new Flash(0xF73626, 0.5F, 8),
                    new TimedTransition(4000, "finale")
                ),
                new State("finale",
                    new State("test",
                    new StayAwayFrom(speed: 2, distFromTarget: 4, acquireRange: 10),
                    new Wander(2, 2, 5),
                    new Shoot(15,
                        new ProjectilePath(400, new LinePath(0))
                            .Then(3000, new LinePath(7)),
                        count: 3,
                        shootAngle: 15,
                        projName: "Fire Bust Shot",
                        targeted: true,
                        cooldownMS: 1500,
                        coolDownOffset: 1000,
                        damage: 80,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(400, new LinePath(0))
                            .Then(3000, new LinePath(7)),
                        count: 2,
                        shootAngle: 45,
                        projName: "Fire Bust Shot",
                        targeted: true,
                        cooldownMS: 1500,
                        coolDownOffset: 1000,
                        damage: 80,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(400, new LinePath(0))
                            .Then(3000, new LinePath(7)),
                        count: 2,
                        shootAngle: 75,
                        projName: "Fire Bust Shot",
                        targeted: true,
                        cooldownMS: 1500,
                        coolDownOffset: 1000,
                        damage: 80,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(1000, new LinePath(6))
                            .Then(1000, new LinePath(0))
                            .Then(1000, new LinePath(6))
                            .Then(1000, new LinePath(0))
                            .Then(1000, new LinePath(6)),
                        count: 10,
                        shootAngle: 36,
                        projName: "Dragon Attack Red Firewave",
                        targeted: true,
                        cooldownMS: 3500,
                        coolDownOffset: 2000,
                        damage: 100,
                        predictive: 1,
                        armorPiercing: true,
                        multiHit: true)
                    )
                )
            );
        [CharacterBehavior("MalphasPortal")]
        public static State MalphasPortal =>
            new State(
                new State("p1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 5000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 5000),
                    new Orbit(3, 2, 10, "Archdemon Malphas", speedVariance: 0, radiusVariance: 0),
                    new TossObject("Brute Warrior of the Abyss", range: 0, angle: 0, cooldownOffsetMS: 5000, cooldownMS: 3000, probability: 0.5F),
                    new TossObject("Demon Warrior of the Abyss", range: 0, angle: 0, cooldownOffsetMS: 5000, cooldownMS: 3000, probability: 0.5F),
                    new TossObject("Demon Mage of the Abyss", range: 0, angle: 0, cooldownOffsetMS: 5000, cooldownMS: 3000, probability: 0.5F)
                )
            );
        [CharacterBehavior("Abyss Idol")]
        public static State AbyssIdol =>
            new State(
                new State("start1",
                    new HpLessTransition(0.99F, "lava")
                ),
                new State("lava",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 2000),
                    new TossObject("AbyssTransition", range: 0, angle: 0, cooldownMS: 50000),
                    new TimedTransition(2000, "shoot1")
                ),
                new State("shoot1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 3000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 3000),

                    // wave

                    new Shoot(15,
                        new ProjectilePath(4000, new DeceleratePath(2)),
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 90,
                        rotateAngle: 40,
                        projName: "Idol Wave",
                        targeted: false,
                        cooldownMS: 4000,
                        coolDownOffset: 1000,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new DeceleratePath(2)),
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 110,
                        rotateAngle: 40,
                        projName: "Idol Wave",
                        targeted: false,
                        cooldownMS: 4000,
                        coolDownOffset: 1200,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new DeceleratePath(2)),
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 70,
                        rotateAngle: 40,
                        projName: "Idol Wave",
                        targeted: false,
                        cooldownMS: 4000,
                        coolDownOffset: 1200,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new DeceleratePath(2)),
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 130,
                        rotateAngle: 40,
                        projName: "Idol Wave",
                        targeted: false,
                        cooldownMS: 4000,
                        coolDownOffset: 1400,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new DeceleratePath(2)),
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 50,
                        rotateAngle: 40,
                        projName: "Idol Wave",
                        targeted: false,
                        cooldownMS: 4000,
                        coolDownOffset: 1400,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),

                    // blast

                    new Shoot(15,
                        new ProjectilePath(2000, new ChangeSpeedPath(2, 0.5F, 500)),
                        count: 2,
                        shootAngle: 5,
                        projName: "Idol Blast",
                        targeted: true,
                        cooldownMS: 2000,
                        coolDownOffset: 2000,
                        damage: 100,
                        size: 120,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(2000, new ChangeSpeedPath(2, 0.5F, 500)),
                        count: 2,
                        shootAngle: 50,
                        projName: "Idol Blast",
                        targeted: true,
                        cooldownMS: 2000,
                        coolDownOffset: 2000,
                        damage: 85,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(2000, new ChangeSpeedPath(2, 0.5F, 500)),
                        count: 2,
                        shootAngle: 55,
                        projName: "Idol Blast",
                        targeted: true,
                        cooldownMS: 2000,
                        coolDownOffset: 2000,
                        damage: 85,
                        multiHit: true),

                    // circles
                    // 1

                    new Shoot(15,
                        new ProjectilePath(4000, new CirclePath(0.2F, 2))
                            .Then(1000, new LinePath(3))
                            .Then(1200, new LinePath(-3)),
                        count: 1,
                        fixedAngle: 90,
                        projName: "LH Yellow",
                        targeted: false,
                        cooldownMS: 12400,
                        coolDownOffset: 3000,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new CirclePath(0.2F, 2))
                            .Then(1000, new LinePath(3))
                            .Then(1200, new LinePath(-3)),
                        count: 1,
                        fixedAngle: 180,
                        projName: "LH Yellow",
                        targeted: false,
                        cooldownMS: 12400,
                        coolDownOffset: 3000,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new CirclePath(0.2F, 2))
                            .Then(1000, new LinePath(3))
                            .Then(1200, new LinePath(-3)),
                        count: 1,
                        fixedAngle: 270,
                        projName: "LH Yellow",
                        targeted: false,
                        cooldownMS: 12400,
                        coolDownOffset: 3000,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new CirclePath(0.2F, 2))
                            .Then(1000, new LinePath(3))
                            .Then(1200, new LinePath(-3)),
                        count: 1,
                        fixedAngle: 360,
                        projName: "LH Yellow",
                        targeted: false,
                        cooldownMS: 12400,
                        coolDownOffset: 3000,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),

                    // 2

                    new Shoot(15,
                        new ProjectilePath(4000, new CirclePath(-0.2F, 2))
                            .Then(1000, new LinePath(3))
                            .Then(1200, new LinePath(-3)),
                        count: 1,
                        fixedAngle: 90,
                        projName: "LH Yellow",
                        targeted: false,
                        cooldownMS: 12400,
                        coolDownOffset: 9200,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new CirclePath(-0.2F, 2))
                            .Then(1000, new LinePath(3))
                            .Then(1200, new LinePath(-3)),
                        count: 1,
                        fixedAngle: 180,
                        projName: "LH Yellow",
                        targeted: false,
                        cooldownMS: 12400,
                        coolDownOffset: 9200,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new CirclePath(-0.2F, 2))
                            .Then(1000, new LinePath(3))
                            .Then(1200, new LinePath(-3)),
                        count: 1,
                        fixedAngle: 270,
                        projName: "LH Yellow",
                        targeted: false,
                        cooldownMS: 12400,
                        coolDownOffset: 9200,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true),
                    new Shoot(15,
                        new ProjectilePath(4000, new CirclePath(-0.2F, 2))
                            .Then(1000, new LinePath(3))
                            .Then(1200, new LinePath(-3)),
                        count: 1,
                        fixedAngle: 360,
                        projName: "LH Yellow",
                        targeted: false,
                        cooldownMS: 12400,
                        coolDownOffset: 9200,
                        damage: 100,
                        armorPiercing: true,
                        multiHit: true)
                )
            );
    }
}