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
        [CharacterBehavior("Hermit God")]
        public static State HermitGod =>
            new(
                new CharacterLoot(
                    // LootTemplates.BasicDrop(threshold: 0.005f),
                    new ItemLoot("Potion of Vitality", 1f, 0.01f),
                    new ItemLoot("Potion of Dexterity", 1f, 0.01f),
                    new ItemLoot("Helm of the Juggernaut", 0.004f, 0.01f)
                ),
                // new ScaleHP2(20),
                new SpawnSetpieceOnDeath("HermitDeath", true),
                new OrderOnDeath(20, "Hermit God Tentacle Spawner", "Die"),
                new OrderOnDeath(20, "Hermit God Drop", "Die"),
                new State("Idle",
                    new EntityWithinTransition(radius: 20, targetState: "Spawn Tentacle")
                ),
                new State("Spawn Tentacle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new SetAltTexture(2),
                    new Order(20, "Hermit God Tentacle Spawner", "Tentacle"),
                    new EntityWithinTransition(target: "Hermit God Tentacle", radius: 20, targetState: "Sleep")
                ),
                new State("Sleep",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Order(20, "Hermit God Tentacle Spawner", "Minions"),
                    new TimedTransition(1000, "Waiting")
                ),
                new State("Waiting",
                    new SetAltTexture(3),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new EntityNotWithinTransition(target: "Hermit God Tentacle", radius: 20, targetState: "Wake")
                ),
                new State("Wake",
                    new SetAltTexture(2),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TossObject("Hermit Minion", 10, 0),
                    new TossObject("Hermit Minion", 10, 45),
                    new TossObject("Hermit Minion", 10, 90),
                    new TossObject("Hermit Minion", 10, 135),
                    new TossObject("Hermit Minion", 10, 180),
                    new TossObject("Hermit Minion", 10, 225),
                    new TossObject("Hermit Minion", 10, 270),
                    new TossObject("Hermit Minion", 10, 315),
                    new TimedTransition(100, "Spawn Whirlpool")
                ),
                new State("Spawn Whirlpool",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Order(20, "Hermit God Tentacle Spawner", "Whirlpool"),
                    new EntityWithinTransition(target: "Whirlpool", radius: 20, targetState: "Attack1")
                ),
                new State("Attack1",
                    new SetAltTexture(0),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 20,
                        count: 3,
                        shootAngle: 5,
                        cooldownMS: 300,
                        targeted: true // Added
                    ),
                    new TimedTransition(6000, "Attack2")
                ),
                new State("Attack2",
                    new Order(20, "Whirlpool", "Die"),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 20,
                        count: 1,
                        fixedAngle: 0,
                        rotateAngle: 45,
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 20,
                        count: 1,
                        fixedAngle: 180,
                        rotateAngle: 45,
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new TimedTransition(6000, "Spawn Tentacle")
                )
            );

        [CharacterBehavior("Hermit Minion")]
        public static State HermitMinion =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.1f),
                    new ItemLoot("Magic Potion", 0.1f)
                ),
                new Follow((0.6f * 5.55f) + 0.74f, acquireRange: 4, distFromTarget: 1), // Adjusted speed
                new Orbit((0.3f * 5.55f) + 0.74f, 10f, target: "Hermit God", speedVariance: 0.2f,
                    radiusVariance: 1.5f), // Adjusted speed
                new Wander((0.6f * 5.55f) + 0.74f), // Adjusted speed
                new Shoot(
                    projectileIndex: 0,
                    maxRadius: 6,
                    count: 3,
                    shootAngle: 10,
                    cooldownMS: 1000,
                    targeted: true // Added
                ),
                new Shoot(
                    projectileIndex: 1,
                    maxRadius: 6,
                    count: 2,
                    shootAngle: 20,
                    predictive: 0.8f,
                    cooldownMS: 2600,
                    targeted: true // Added
                )
            );

        [CharacterBehavior("Whirlpool")]
        public static State Whirlpool =>
            new(
                new State("Attack",
                    new EntityNotWithinTransition(target: "Hermit God", radius: 100, targetState: "Die"),
                    new Orbit((0.6f * 5.55f) + 0.74f, 5.5f, target: "Hermit God"), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 0,
                        count: 1,
                        fixedAngle: 0,
                        rotateAngle: 30,
                        cooldownMS: 400,
                        targeted: true // Added
                    )
                ),
                new State("Die",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 0,
                        count: 8,
                        shootAngle: 360f / 8, // Calculated shootAngle
                        cooldownMS: 1000
                    ),
                    new Suicide()
                )
            );

        [CharacterBehavior("Hermit God Tentacle")]
        public static State HermitGodTentacle =>
            new(
                new Orbit((1f * 5.55f) + 0.74f, 5.5f, target: "Hermit God"), // Adjusted speed
                new Shoot(
                    projectileIndex: 0,
                    maxRadius: 3,
                    count: 8,
                    shootAngle: 360f / 8, // Calculated shootAngle
                    cooldownMS: 500,
                    targeted: true // Added
                )
            );

        [CharacterBehavior("Hermit God Tentacle Spawner")]
        public static State HermitGodTentacleSpawner =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Waiting Order"),
                new State("Tentacle",
                    new Spawn("Hermit God Tentacle", maxSpawnsPerReset: 1, cooldownMs: 2000),
                    new EntityWithinTransition(target: "Hermit God Tentacle", radius: 1, targetState: "Waiting Order")
                ),
                new State("Whirlpool",
                    new Spawn("Whirlpool", maxSpawnsPerReset: 1, cooldownMs: 2000),
                    new EntityWithinTransition(target: "Whirlpool", radius: 1, targetState: "Waiting Order")
                ),
                new State("Minions",
                    new Spawn("Hermit Minion", maxSpawnsPerReset: 1, cooldownMs: 2000),
                    new TimedTransition(2000, "Waiting Order")
                ),
                new State("Die",
                    new Suicide()
                )
            );

        [CharacterBehavior("Hermit portal maker")]
        public static State HermitPortalMaker =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new DropPortalOnDeath("Ocean Trench Portal", 1),
                new State("Wait",
                    new EntityNotWithinTransition(target: "Hermit God", radius: 50, targetState: "Transform")
                ),
                new State("Transform",
                    new Suicide()
                )
            );
    }
}