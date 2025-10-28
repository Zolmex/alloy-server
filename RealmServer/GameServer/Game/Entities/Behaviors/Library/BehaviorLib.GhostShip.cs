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
        [CharacterBehavior("Vengeful Spirit")]
        public static State VengefulSpirit =>
            new(
                new State("Start",
                    new ChangeSize(50, 120),
                    new Follow((0.5f * 5.55f) + 0.74f, 1, 8),
                    new Wander((0.3f * 5.55f) + 0.74f),
                    new Shoot(8.4f, 3, projectileIndex: 0, shootAngle: 16, cooldownMS: 800, targeted: true),
                    new TimedTransition(1000, "Vengeful")
                ),
                new State("Vengeful",
                    new Follow((0.5f * 5.55f) + 0.74f, 8, 1),
                    new Wander((0.3f * 5.55f) + 0.74f),
                    new Shoot(8.4f, 3, projectileIndex: 0, shootAngle: 16, cooldownMS: 1245, targeted: true),
                    new TimedTransition(3000, "Vengeful2")
                ),
                new State("Vengeful2",
                    new ReturnToSpawn((1 * 5.55f) + 0.74f),
                    new Shoot(8.4f, 3, projectileIndex: 0, shootAngle: 16, cooldownMS: 750, targeted: true),
                    new TimedTransition(1500, "Vengeful")
                )
            );

        [CharacterBehavior("Water Mine")]
        public static State WaterMine =>
            new(
                new State("Seek",
                    new Follow((.9f * 5.55f) + 0.74f, 1, 8),
                    new Wander((0.3f * 5.55f) + 0.74f),
                    new TimedTransition(3750, "Boom")
                ),
                new State("Boom",
                    new Shoot(8.4f, 10, 360 / 10f, 0, cooldownMS: 700, targeted: true),
                    new Suicide()
                )
            );

        [CharacterBehavior("Beach Spectre")]
        public static State BeachSpectre =>
            new(
                new State("Fight",
                    new Wander((0.3f * 5.55f) + 0.74f),
                    new ChangeSize(10, 120),
                    new Shoot(8.4f, 3, projectileIndex: 0, shootAngle: 14, cooldownMS: 1250, targeted: true)
                )
            );

        [CharacterBehavior("Beach Spectre Spawner")]
        public static State BeachSpectreSpawner =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Idle",
                    new TimedTransition(1000, "Spawn")
                ),
                new State("Spawn",
                    new EntityNotWithinTransition("Ghost Ship", 30, targetStates: "Death"),
                    new Reproduce("Beach Spectre", maxDensity: 1, densityRadius: 3, cooldownMs: 1250)
                ),
                new State("Death",
                    new Suicide()
                )
            );

        [CharacterBehavior("Tempest Cloud")]
        public static State TempestCloud =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Start1",
                    new ChangeSize(70, 130),
                    new TimedTransition(3000, "Start2")
                ),
                new State("Start2",
                    new SetAltTexture(1),
                    new TimedTransition(1, "Start3")
                ),
                new State("Start3",
                    new SetAltTexture(2),
                    new TimedTransition(1, "Start4")
                ),
                new State("Start4",
                    new SetAltTexture(3),
                    new TimedTransition(1, "Start5")
                ),
                new State("Start5",
                    new SetAltTexture(4),
                    new TimedTransition(1, "Start6")
                ),
                new State("Start6",
                    new SetAltTexture(5),
                    new TimedTransition(1, "Start7")
                ),
                new State("Start7",
                    new SetAltTexture(6),
                    new TimedTransition(1, "Start8")
                ),
                new State("Start8",
                    new SetAltTexture(7),
                    new TimedTransition(1, "Start9")
                ),
                new State("Start9",
                    new SetAltTexture(8),
                    new TimedTransition(1, "Final")
                ),
                new State("Final",
                    new SetAltTexture(9),
                    new TimedTransition(1, "CircleAndStorm")
                ),
                new State("CircleAndStorm",
                    new EntityNotWithinTransition("Ghost Ship", 30, targetStates: "Death"),
                    new Orbit(1, 9, 20, "Ghost Ship Anchor"),
                    new Shoot(8.4f, 7, 360 / 7f, 0, cooldownMS: 1000, targeted: true)
                ),
                new State("Death",
                    new Suicide()
                )
            );

        [CharacterBehavior("Ghost Ship Anchor")]
        public static State GhostShipAnchor =>
            new(
                new State("idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible)
                ),
                new State("tempestcloud",
                    new TossObject("Tempest Cloud", 9, 0, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 45, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 90, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 135, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 180, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 225, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 270, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 315, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 350, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 250, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 110, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 200, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 10, 9999999, tossInvis: true),
                    new TossObject("Tempest Cloud", 9, 290, 9999999, tossInvis: true),

                    //Spectre Spawner
                    new TossObject("Beach Spectre Spawner", 17, 0, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 45, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 90, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 135, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 180, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 225, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 270, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 315, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 250, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 110, 9999999, tossInvis: true),
                    new TossObject("Beach Spectre Spawner", 17, 200, 9999999, tossInvis: true),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible)
                )
            );

        [CharacterBehavior("Ghost Ship")]
        public static State GhostShip =>
            new(
                new CharacterLoot(
                    // LootTemplates.BasicDrop(),
                    new ItemLoot("Trap of the Vile Spirit", 0.001f, 0.01f),
                    new ItemLoot("Ghost Pirate Rum", 1)
                ),
                new SpawnSetpieceOnDeath("GhostShipDeath", true),
                // new ScaleHP2(20),
                new State("idle",
                    new SetAltTexture(1),
                    new Wander((1 * 5.55f) + 0.74f),
                    new DamageTakenTransition(2000, "pause")
                ),
                new State("pause",
                    new SetAltTexture(2),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(2000, "start")
                ),
                new State("start",
                    new SetAltTexture(0),
                    new Reproduce("Vengeful Spirit", maxDensity: 2, cooldownMs: 2500),
                    new TimedTransition(15000, "midfight"),
                    new State("2",
                        new SetAltTexture(0),
                        new Wander((0.1f * 5.55f) + 0.74f),
                        new StayAwayFrom((0.1f * 5.55f) + 0.74f, 5),
                        new Shoot(12, 1, projectileIndex: 0, cooldownMS: 450, targeted: true),
                        new Shoot(12, 3, projectileIndex: 0, shootAngle: 20, cooldownMS: 1050, targeted: true),
                        new TimedTransition(3250, "1")
                    ),
                    new State("1",
                        new TossObject("Water Mine", 7, cooldownMS: 1000),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new ReturnToSpawn((1 * 5.55f) + 0.74f),
                        new Shoot(12, 1, projectileIndex: 0, cooldownMS: 450, targeted: true),
                        new Shoot(12, 3, projectileIndex: 0, shootAngle: 20, cooldownMS: 1050, targeted: true),
                        new TimedTransition(1500, "2")
                    )
                ),
                new State("midfight",
                    new Order(100, "Ghost Ship Anchor", "tempestcloud"),
                    new Reproduce("Vengeful Spirit", maxDensity: 1, cooldownMs: 5000),
                    new TossObject("Water Mine", 5, cooldownMS: 2250),
                    new TimedTransition(10000, "countdown"),
                    new State("midfight2",
                        new SetAltTexture(0),
                        new ReturnToSpawn((1 * 5.55f) + 0.74f),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new Shoot(10, 4, 360 / 4f, 0, coolDownOffset: 800,
                            angleOffset: 270, cooldownMS: 1000, targeted: true),
                        new Shoot(10, 4, 360 / 4f, 0, coolDownOffset: 800,
                            angleOffset: 90, cooldownMS: 1000, targeted: true),
                        new Shoot(8.4f, 1, projectileIndex: 1, cooldownMS: 1250, targeted: true),
                        new TimedTransition(3000, "midfight1")
                    ),
                    new State("midfight1",
                        new Follow((0.8f * 5.55f) + 0.74f, 1, 8),
                        new Wander((0.5f * 5.55f) + 0.74f),
                        new Taunt("Fire at will!"),
                        new Shoot(8.4f, 2, 25, 1, cooldownMS: 1250, targeted: true),
                        new Shoot(8.4f, 6, projectileIndex: 0, shootAngle: 10, cooldownMS: 950, targeted: true),
                        new TimedTransition(4000, "midfight2")
                    )
                ),
                new State("countdown",
                    new Wander((0.3f * 5.55f) + 0.74f),
                    new Timed(1000,
                        new Taunt("Ready..")
                    ),
                    new Timed(2000,
                        new Taunt("Aim..")
                    ),
                    new Shoot(8.4f, 1, projectileIndex: 0, cooldownMS: 450, targeted: true),
                    new Shoot(8.4f, 5, projectileIndex: 0, shootAngle: 20, cooldownMS: 750, targeted: true),
                    new TimedTransition(2000, "fire")
                ),
                new State("fire",
                    new Follow((0.3f * 5.55f) + 0.74f, 1, 8),
                    new Wander((0.1f * 5.55f) + 0.74f),
                    new Shoot(10, 4, 360 / 4f, 1, coolDownOffset: 1100,
                        angleOffset: 270, cooldownMS: 850, targeted: true),
                    new Shoot(10, 4, 360 / 4f, 1, coolDownOffset: 1100, angleOffset: 90,
                        cooldownMS: 850, targeted: true),
                    new Shoot(8.4f, 10, projectileIndex: 0, cooldownMS: 1300, targeted: true),
                    new TimedTransition(3400, "midfight")
                )
            );
    }
}