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
        [CharacterBehavior("Lord of the Lost Lands")]
        public static State LordOfTheLostLands =>
            new(
                new CharacterLoot(
                    // LootTemplates.BasicDrop(0.005),
                    new ItemLoot("Shield of Ogmur", 0.003f, 0.01f)
                ),
                // new ScaleHP2(20),
                new DropPortalOnDeath("Ice Cave Portal", 0.5f),
                new State("0",
                    new HpLessTransition(0.99f, "Start1.0")
                ),
                new State("Start1.0",
                    new HpLessTransition(0.1f, "Dead"),
                    new State("Start",
                        new SetAltTexture(0),
                        new Wander((0.4f * 5.55f) + 0.74f),
                        new Shoot(12, 7, 10, fixedAngle: 180, cooldownMS: 1000),
                        new Shoot(12, 7, 10, fixedAngle: 0, cooldownMS: 800),
                        new TossObject("Guardian of the Lost Lands", 5, cooldownMS: 1000),
                        new TimedTransition(100, "Spawning Guardian")
                    ),
                    new State("Spawning Guardian",
                        new TossObject("Guardian of the Lost Lands", 5, cooldownMS: 1000),
                        new TimedTransition(3100, "Attack")
                    ),
                    new State("Attack",
                        new SetAltTexture(0),
                        new Wander((0.4f * 5.55f) + 0.74f),
                        new EntityWithinTransition("Follow", radius: 1),
                        new TimedTransition(10000, "Gathering"),
                        new State("Attack1.0",
                            new TimedTransition(3000, TransitionType.Random,
                                "Attack1.1",
                                "Attack1.2"),
                            new State("Attack1.1",
                                new Shoot(12, 7, 10, cooldownMS: 1000, targeted: true),
                                new Shoot(12, 7, 190, cooldownMS: 800, targeted: true),
                                new TimedTransition(2000, "Attack1.0")
                            ),
                            new State("Attack1.2",
                                new Shoot(10, 7, 10, fixedAngle: 180, cooldownMS: 1000),
                                new Shoot(10, 7, 10, fixedAngle: 0, cooldownMS: 800),
                                new TimedTransition(2000, "Attack1.0")
                            )
                        )
                    ),
                    new State("Follow",
                        new Follow((1 * 5.55f) + 0.74f, 3, 20),
                        new Wander((0.4f * 5.55f) + 0.74f),
                        new Shoot(20, 7, 10, cooldownMS: 700, targeted: true),
                        new TimedTransition(5000, "Gathering")
                    ),
                    new State("Gathering",
                        new Taunt("Gathering power!", probability: 0.99f),
                        new SetAltTexture(3),
                        new TimedTransition(2000, "Gathering1.0")
                    ),
                    new State("Gathering1.0",
                        new TimedTransition(5000, "Protection"),
                        new State("Gathering1.1",
                            new Shoot(30, 4, 360 / 4f, fixedAngle: 90, projectileIndex: 1,
                                cooldownMS: 1200),
                            new TimedTransition(1500, "Gathering1.2")
                        ),
                        new State("Gathering1.2",
                            new Shoot(30, 4, 360 / 4f, fixedAngle: 45, projectileIndex: 1,
                                cooldownMS: 1000),
                            new TimedTransition(1500, "Gathering1.1")
                        )
                    ),
                    new State("Protection",
                        new SetAltTexture(0),
                        new TossObject("Protection Crystal", 4, 0, 5000),
                        new TossObject("Protection Crystal", 4, 45, 5000),
                        new TossObject("Protection Crystal", 4, 90, 5000),
                        new TossObject("Protection Crystal", 4, 135, 5000),
                        new TossObject("Protection Crystal", 4, 180, 5000),
                        new TossObject("Protection Crystal", 4, 225, 5000),
                        new TossObject("Protection Crystal", 4, 270, 5000),
                        new TossObject("Protection Crystal", 4, 315, 5000),
                        new EntityWithinTransition("Protection Crystal", 10, targetStates: "Waiting")
                    )
                ),
                new State("Waiting",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new SetAltTexture(1),
                    new EntityNotWithinTransition("Protection Crystal", 10, targetStates: "Start1.0")
                ),
                new State("Dead",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new SetAltTexture(3),
                    new Taunt("NOOOO!!!!!!"),
                    new Flash(0xFF0000, .1, 1000),
                    new TimedTransition(2000, "Suicide")
                ),
                new State("Suicide",
                    new ConditionEffectBehavior(ConditionEffectIndex.StunImmune, persist: true),
                    new Shoot(12, 8, 360 / 8f, fixedAngle: 360 / 8f, projectileIndex: 1, cooldownMS: 1000),
                    new Suicide()
                )
            );

        [CharacterBehavior("Protection Crystal")]
        public static State ProtectionCrystal =>
            new(
                new Orbit(3, 4, 10, "Lord of the Lost Lands", 0, 0),
                new Shoot(12, 4, 7, cooldownMS: 500, targeted: true)
            );

        [CharacterBehavior("Guardian of the Lost Lands")]
        public static State GuardianOfTheLostLands =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.1f, 0f),
                    new ItemLoot("Magic Potion", 0.1f, 0f)
                ),
                new State("Full",
                    new Spawn("Knight of the Lost Lands", maxSpawnsPerReset: 2, cooldownMs: 4000),
                    new Follow((2 * 5.55f) + 0.74f, 6, 20),
                    new Wander((0.3f * 5.55f) + 0.74f),
                    new Shoot(10, 8, 360 / 8f, fixedAngle: 360 / 8f, cooldownMS: 1300, projectileIndex: 1),
                    new Shoot(10, 5, 10, cooldownMS: 1200, targeted: true),
                    new HpLessTransition(0.25f, "Low")
                ),
                new State("Low",
                    new StayAwayFrom((1 * 5.55f) + 0.74f, 5),
                    new Wander((1 * 5.55f) + 0.74f),
                    new Shoot(10, 8, 360 / 8f, fixedAngle: 360 / 8f, cooldownMS: 3000, projectileIndex: 1),
                    new Shoot(10, 5, 10, cooldownMS: 1500, targeted: true)
                )
            );

        [CharacterBehavior("Knight of the Lost Lands")]
        public static State KnightOfTheLostLands =>
            new(
                new CharacterLoot(
                    new ItemLoot("Health Potion", 0.1f, 0f),
                    new ItemLoot("Magic Potion", 0.1f, 0f)
                ),
                new Follow((2 * 5.55f) + 0.74f, 4, 20),
                new StayAwayFrom((1 * 5.55f) + 0.74f, 2),
                new Wander((1 * 5.55f) + 0.74f),
                new Shoot(13, 1, cooldownMS: 700, targeted: true)
            );
    }
}