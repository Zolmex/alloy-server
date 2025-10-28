#region

using Common;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;
using GameServer.Game.Entities.Loot;

#endregion

//FULLY SHATTERS
//BEHAVIORS
//MADE BY
//MIKE (Qkm)
//MOISTED ON BY PATPOT

namespace GameServer.Game.Entities.Behaviors.Library
{
    public partial class BehaviorLib
    {
        #region restofmobs

        [CharacterBehavior("shtrs Stone Paladin")]
        public static State ShtrsStonePaladin =>
            new(
                new State("Idle",
                    new Wander((0.4f * 5.55f) + 0.74f), // Adjusted speed
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Reproduce(maxDensity: 4),
                    new EntityWithinTransition(radius: 8, targetState: "Attacking")
                ),
                new State("Attacking",
                    new State("Bullet",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 90,
                            shootAngle: 90,
                            cooldownMS: 10000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 100,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 110,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 120,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 130,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 140,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 150,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 160,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 170,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 180,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 180,
                            shootAngle: 45,
                            cooldownMS: 10000,
                            coolDownOffset: 2000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 180,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 0
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 170,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 160,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 150,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 140,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 130,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 120,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 110,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 100,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 90,
                            shootAngle: 90,
                            cooldownMS: 10000,
                            coolDownOffset: 1800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 11,
                            count: 4,
                            fixedAngle: 90,
                            shootAngle: 22.5f,
                            cooldownMS: 10000,
                            coolDownOffset: 2000
                        ),
                        new TimedTransition(2000, "Wait")
                    ),
                    new State("Wait",
                        new Follow((1f * 5.55f) + 0.74f, acquireRange: 2), // Adjusted speed
                        new Flash(0x00ff00, 0.1f, 20),
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(2000, "Bullet")
                    ),
                    new EntityNotWithinTransition(radius: 13, targetState: "Idle")
                )
            );

        [CharacterBehavior("shtrs Stone Knight")]
        public static State ShtrsStoneKnight =>
            new(
                new State("Follow",
                    new Follow((1f * 5.55f) + 0.74f, acquireRange: 10, distFromTarget: 5), // Adjusted speed
                    new EntityWithinTransition(radius: 10, targetState: "Charge")
                ),
                new State("Charge",
                    new TimedTransition(2000, "Follow"),
                    new Charge((1f * 5.55f) + 0.74f, 6, 2000), // Adjusted speed
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 5,
                        count: 16,
                        shootAngle: 360 / 16f,
                        cooldownMS: 2400,
                        coolDownOffset: 400,
                        targeted: true // Added
                    )
                )
            );

        [CharacterBehavior("shtrs Lava Souls")]
        public static State ShtrsLavaSouls =>
            new(
                new State("active",
                    new Follow((1f * 5.55f) + 0.74f, acquireRange: 0), // Adjusted speed
                    new EntityWithinTransition(radius: 2, targetState: "blink")
                ),
                new State("blink",
                    new Flash(0xFF0000, flashRepeats: 10000, flashPeriod: 0.1f),
                    new TimedTransition(2000, "explode")
                ),
                new State("explode",
                    new Flash(0xFF0000, flashRepeats: 5, flashPeriod: 0.1f),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 5,
                        count: 9,
                        shootAngle: 360f / 9, // Calculated shootAngle
                        cooldownMS: 1000
                    ),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Glassier Archmage")]
        public static State ShtrsGlassierArchmage =>
            new(
                new StayAwayFrom((0.5f * 5.55f) + 0.74f, 5), // Adjusted speed
                new State("1st",
                    new Follow((1.8f * 5.55f) + 0.74f, acquireRange: 7), // Adjusted speed
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 12,
                        count: 1,
                        cooldownMS: 350,
                        targeted: true // Added
                    ),
                    new TimedTransition(5000, "next")
                ),
                new State("next",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 12,
                        count: 25,
                        cooldownMS: 5000,
                        targeted: true // Added
                    ),
                    new TimedTransition(25, "1st")
                )
            );

        [CharacterBehavior("shtrs Ice Adept")]
        public static State ShtrsIceAdept =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new EntityWithinTransition("Main", radius: 7.5f)
                ),
                new State("Main",
                    new TimedTransition(10000, "Throw"),
                    new Follow(3f, 5, 20),
                    new Shoot(15, 8, 20, 1, cooldownMS: 4000, targeted: true),
                    new State("1",
                        new Shoot(15, 1, cooldownMS: 1000, targeted: true),
                        new TimedTransition(200, TransitionType.Random, "1", "2", "3")
                    ),
                    new State("2",
                        new Shoot(15, 2, 10, cooldownMS: 1000, targeted: true),
                        new TimedTransition(200, TransitionType.Random, "1", "2", "3")
                    ),
                    new State("3",
                        new Shoot(15, 3, 5, cooldownMS: 1000, targeted: true),
                        new TimedTransition(200, TransitionType.Random, "1", "2", "3")
                    )
                ),
                new State("Throw",
                    new Flash(byte.MaxValue, .25, 4),
                    new TossObject("shtrs Ice Portal", cooldownMS: 8000),
                    new TimedTransition(2000, "Main")
                )
            );

        [CharacterBehavior("shtrs Fire Adept")]
        public static State ShtrsFireAdept =>
            new(
                new State("Main",
                    new TimedTransition(5000, "Throw"),
                    new Follow((1f * 5.55f) + 0.74f, acquireRange: 1), // Adjusted speed
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 1,
                        predictive: 1f,
                        cooldownMS: 200,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 10,
                        count: 3,
                        shootAngle: 10,
                        predictive: 1f,
                        cooldownMS: 4000,
                        targeted: true // Added
                    )
                ),
                new State("Throw",
                    new TossObject("shtrs Fire Portal", 5, cooldownMS: 8000, cooldownOffsetMS: 7000),
                    new TimedTransition(1000, "Main")
                )
            );

        #endregion

        #region generators

        [CharacterBehavior("shtrs MagiGenerators")]
        public static State ShtrsMagiGenerators =>
            new(
                new State("Main",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 15,
                        count: 10,
                        shootAngle: 360f / 10, // Calculated shootAngle
                        cooldownMS: 1000,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 15,
                        count: 1,
                        cooldownMS: 2500,
                        targeted: true // Added
                    ),
                    new EntitiesNotWithinTransition(30, "Hide", "shtrs Twilight Archmage",
                        "shtrs Inferno", "shtrs Blizzard")
                ),
                new State("Hide",
                    new SetAltTexture(1),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(100, "Despawn")
                ),
                new State("vulnerable",
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored)
                ),
                new State("Despawn",
                    new Suicide()
                )
            );

        #endregion

        #region portals

        [CharacterBehavior("shtrs Ice Portal")]
        public static State ShtrsIcePortal =>
            new(
                new State("Idle",
                    new TimedTransition(1000, "Spin")
                ),
                new State("Spin",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 0,
                        cooldownMS: 1200
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 15,
                        cooldownMS: 1200,
                        coolDownOffset: 200
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 30,
                        cooldownMS: 1200,
                        coolDownOffset: 400
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 45,
                        cooldownMS: 1200,
                        coolDownOffset: 600
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 60,
                        cooldownMS: 1200,
                        coolDownOffset: 800
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 75,
                        cooldownMS: 1200,
                        coolDownOffset: 1000
                    ),
                    new TimedTransition(4200, "Pause")
                ),
                new State("Pause",
                    new TimedTransition(5000, "Idle")
                )
            );

        [CharacterBehavior("shtrs Fire Portal")]
        public static State ShtrsFirePortal =>
            new(
                new State("Idle",
                    new TimedTransition(1000, "Spin")
                ),
                new State("Spin",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 0,
                        cooldownMS: 1200
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 15,
                        cooldownMS: 1200,
                        coolDownOffset: 200
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 30,
                        cooldownMS: 1200,
                        coolDownOffset: 400
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 45,
                        cooldownMS: 1200,
                        coolDownOffset: 600
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 60,
                        cooldownMS: 1200,
                        coolDownOffset: 800
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 6,
                        shootAngle: 60,
                        fixedAngle: 75,
                        cooldownMS: 1200,
                        coolDownOffset: 1000
                    ),
                    new TimedTransition(4200, "Pause")
                ),
                new State("Pause",
                    new TimedTransition(5000, "Idle")
                )
            );

        #endregion

        #region 1stboss

        [CharacterBehavior("shtrs Bridge Sentinel")]
        public static State ShtrsBridgeSentinel =>
            new(
                // new ScaleHP2(35),
                new Shoot(2, projectileIndex: 5, count: 3, fixedAngle: 90, cooldownMS: 500),
                new Shoot(2, projectileIndex: 5, count: 3, fixedAngle: 45 + 90, cooldownMS: 500),
                new Shoot(2, projectileIndex: 5, count: 3, fixedAngle: 90 + 90, cooldownMS: 500),
                new Shoot(2, projectileIndex: 5, count: 3, fixedAngle: 135 + 90, cooldownMS: 500),
                new Shoot(2, projectileIndex: 5, count: 3, fixedAngle: 180 + 90, cooldownMS: 500),
                new HpLessTransition(0.1f, "Death"),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntityWithinTransition(radius: 15, targetState: "Close Bridge")
                ),
                new State("Close Bridge",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Order(46, "shtrs Bridge Closer", "Closer"),
                    new TimedTransition(5000, "Close Bridge2")
                ),
                new State("Close Bridge2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Order(46, "shtrs Bridge Closer2", "Closer"),
                    new TimedTransition(5000, "Close Bridge3")
                ),
                new State("Close Bridge3",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Order(46, "shtrs Bridge Closer3", "Closer"),
                    new TimedTransition(5000, "Close Bridge4")
                ),
                new State("Close Bridge4",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Order(46, "shtrs Bridge Closer4", "Closer"),
                    new TimedTransition(6000, "BEGIN")
                ),
                new State("BEGIN",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntitiesNotWithinTransition(30, "Wake", "shtrs Bridge Obelisk A",
                        "shtrs Bridge Obelisk B", "shtrs Bridge Obelisk D", "shtrs Bridge Obelisk E",
                        "shtrs Bridge Obelisk C", "shtrs Bridge Obelisk F")
                ),
                new State("Wake",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("Who has woken me...? Leave this place."),
                    new Timed(2100, new Shoot(
                        projectileIndex: 0,
                        maxRadius: 15,
                        count: 15,
                        shootAngle: 11,
                        fixedAngle: 102,
                        cooldownMS: 700,
                        coolDownOffset: 3000
                    )),
                    new TimedTransition(8000, "Swirl Shot")
                ),
                new State("Swirl Shot",
                    new Taunt("Go."),
                    new TimedTransition(10000, "Blobomb"),
                    new State("Swirl1",
                        new HpLessTransition(0.05f, "Death"),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 102,
                            fixedAngle: 102,
                            cooldownMS: 6200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 114,
                            fixedAngle: 114,
                            cooldownMS: 6200,
                            coolDownOffset: 200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 126,
                            fixedAngle: 126,
                            cooldownMS: 6200,
                            coolDownOffset: 400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 138,
                            fixedAngle: 138,
                            cooldownMS: 6200,
                            coolDownOffset: 600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 150,
                            fixedAngle: 150,
                            cooldownMS: 6200,
                            coolDownOffset: 800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 162,
                            fixedAngle: 162,
                            cooldownMS: 6200,
                            coolDownOffset: 1000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 174,
                            fixedAngle: 174,
                            cooldownMS: 6200,
                            coolDownOffset: 1200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 186,
                            fixedAngle: 186,
                            cooldownMS: 6200,
                            coolDownOffset: 1400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 198,
                            fixedAngle: 198,
                            cooldownMS: 6200,
                            coolDownOffset: 1600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 210,
                            fixedAngle: 210,
                            cooldownMS: 6200,
                            coolDownOffset: 1800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 222,
                            fixedAngle: 222,
                            cooldownMS: 6200,
                            coolDownOffset: 2000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 234,
                            fixedAngle: 234,
                            cooldownMS: 6200,
                            coolDownOffset: 2200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 246,
                            fixedAngle: 246,
                            cooldownMS: 6200,
                            coolDownOffset: 2400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 258,
                            fixedAngle: 258,
                            cooldownMS: 6200,
                            coolDownOffset: 2600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 270,
                            fixedAngle: 270,
                            cooldownMS: 6200,
                            coolDownOffset: 2800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 282,
                            fixedAngle: 282,
                            cooldownMS: 6200,
                            coolDownOffset: 3000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 290,
                            fixedAngle: 282,
                            cooldownMS: 6200,
                            coolDownOffset: 3200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 270,
                            fixedAngle: 270,
                            cooldownMS: 6200,
                            coolDownOffset: 3400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 258,
                            fixedAngle: 258,
                            cooldownMS: 6200,
                            coolDownOffset: 3600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 246,
                            fixedAngle: 246,
                            cooldownMS: 6200,
                            coolDownOffset: 3800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 234,
                            fixedAngle: 234,
                            cooldownMS: 6200,
                            coolDownOffset: 4000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 222,
                            fixedAngle: 222,
                            cooldownMS: 6200,
                            coolDownOffset: 4200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 210,
                            fixedAngle: 210,
                            cooldownMS: 6200,
                            coolDownOffset: 4400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 198,
                            fixedAngle: 198,
                            cooldownMS: 6200,
                            coolDownOffset: 4600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 186,
                            fixedAngle: 186,
                            cooldownMS: 6200,
                            coolDownOffset: 4800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 174,
                            fixedAngle: 174,
                            cooldownMS: 6200,
                            coolDownOffset: 5000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 162,
                            fixedAngle: 162,
                            cooldownMS: 6200,
                            coolDownOffset: 5200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 150,
                            fixedAngle: 150,
                            cooldownMS: 6200,
                            coolDownOffset: 5400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 138,
                            fixedAngle: 138,
                            cooldownMS: 6200,
                            coolDownOffset: 5600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 126,
                            fixedAngle: 126,
                            cooldownMS: 6200,
                            coolDownOffset: 5800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 114,
                            fixedAngle: 114,
                            cooldownMS: 6200,
                            coolDownOffset: 6000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 102,
                            fixedAngle: 102,
                            cooldownMS: 6200,
                            coolDownOffset: 6200
                        ),
                        new TimedTransition(6000, "Swirl1")
                    )
                ),
                new State("Blobomb",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Taunt("You live still? DO NOT TEMPT FATE!"),
                    new Taunt("CONSUME!"),
                    new Order(20, "shtrs blobomb maker", "Spawn"),
                    new EntityNotWithinTransition(target: "shtrs Blobomb", radius: 30, targetState: "SwirlAndShoot")
                ),
                new State("SwirlAndShoot",
                    new TimedTransition(10000, "Blobomb"),
                    new Taunt("FOOLS! YOU DO NOT UNDERSTAND!"),
                    new ChangeSize(20, 130),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 15,
                        count: 15,
                        shootAngle: 11,
                        fixedAngle: 102,
                        cooldownMS: 700,
                        coolDownOffset: 700
                    ),
                    new State("Swirl1_2",
                        new HpLessTransition(0.05f, "Death"),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 102,
                            fixedAngle: 102,
                            cooldownMS: 6200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 114,
                            fixedAngle: 114,
                            cooldownMS: 6200,
                            coolDownOffset: 200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 126,
                            fixedAngle: 126,
                            cooldownMS: 6200,
                            coolDownOffset: 400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 138,
                            fixedAngle: 138,
                            cooldownMS: 6200,
                            coolDownOffset: 600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 150,
                            fixedAngle: 150,
                            cooldownMS: 6200,
                            coolDownOffset: 800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 162,
                            fixedAngle: 162,
                            cooldownMS: 6200,
                            coolDownOffset: 1000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 174,
                            fixedAngle: 174,
                            cooldownMS: 6200,
                            coolDownOffset: 1200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 186,
                            fixedAngle: 186,
                            cooldownMS: 6200,
                            coolDownOffset: 1400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 198,
                            fixedAngle: 198,
                            cooldownMS: 6200,
                            coolDownOffset: 1600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 210,
                            fixedAngle: 210,
                            cooldownMS: 6200,
                            coolDownOffset: 1800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 222,
                            fixedAngle: 222,
                            cooldownMS: 6200,
                            coolDownOffset: 2000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 234,
                            fixedAngle: 234,
                            cooldownMS: 6200,
                            coolDownOffset: 2200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 246,
                            fixedAngle: 246,
                            cooldownMS: 6200,
                            coolDownOffset: 2400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 258,
                            fixedAngle: 258,
                            cooldownMS: 6200,
                            coolDownOffset: 2600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 270,
                            fixedAngle: 270,
                            cooldownMS: 6200,
                            coolDownOffset: 2800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 282,
                            fixedAngle: 282,
                            cooldownMS: 6200,
                            coolDownOffset: 3000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 290,
                            fixedAngle: 282,
                            cooldownMS: 6200,
                            coolDownOffset: 3200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 270,
                            fixedAngle: 270,
                            cooldownMS: 6200,
                            coolDownOffset: 3400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 258,
                            fixedAngle: 258,
                            cooldownMS: 6200,
                            coolDownOffset: 3600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 246,
                            fixedAngle: 246,
                            cooldownMS: 6200,
                            coolDownOffset: 3800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 234,
                            fixedAngle: 234,
                            cooldownMS: 6200,
                            coolDownOffset: 4000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 222,
                            fixedAngle: 222,
                            cooldownMS: 6200,
                            coolDownOffset: 4200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 210,
                            fixedAngle: 210,
                            cooldownMS: 6200,
                            coolDownOffset: 4400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 198,
                            fixedAngle: 198,
                            cooldownMS: 6200,
                            coolDownOffset: 4600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 186,
                            fixedAngle: 186,
                            cooldownMS: 6200,
                            coolDownOffset: 4800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 174,
                            fixedAngle: 174,
                            cooldownMS: 6200,
                            coolDownOffset: 5000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 162,
                            fixedAngle: 162,
                            cooldownMS: 6200
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 150,
                            fixedAngle: 150,
                            cooldownMS: 6200,
                            coolDownOffset: 5400
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 138,
                            fixedAngle: 138,
                            cooldownMS: 6200,
                            coolDownOffset: 5600
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 126,
                            fixedAngle: 126,
                            cooldownMS: 6200,
                            coolDownOffset: 5800
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 114,
                            fixedAngle: 114,
                            cooldownMS: 6200,
                            coolDownOffset: 6000
                        ),
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 50,
                            count: 1,
                            shootAngle: 102,
                            fixedAngle: 102,
                            cooldownMS: 6200,
                            coolDownOffset: 6200
                        ),
                        new TimedTransition(6000, "Swirl1_2")
                    )
                ),
                new State("Death",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("I tried to protect you... I have failed. You release a great evil upon this realm...."),
                    new TimedTransition(2000, "Suicide")
                ),
                new State("Suicide",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 35,
                        count: 30,
                        shootAngle: 360f / 30, // Calculated shootAngle
                        cooldownMS: 1000
                    ),
                    new Suicide()
                )
            );

        #endregion

        #region blobomb

        [CharacterBehavior("shtrs Blobomb")]
        public static State ShtrsBlobomb =>
            new(
                new State("active",
                    new Follow((0.7f * 5.55f) + 0.74f, acquireRange: 40, distFromTarget: 0), // Adjusted speed
                    new EntityWithinTransition(radius: 2, targetState: "blink")
                ),
                new State("blink",
                    new Flash(0xFF0000, flashRepeats: 10000, flashPeriod: 0.1f),
                    new TimedTransition(2000, "explode")
                ),
                new State("explode",
                    new Flash(0xFF0000, flashRepeats: 5, flashPeriod: 0.1f),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 30,
                        count: 36,
                        fixedAngle: 0,
                        shootAngle: 360f / 36, // Calculated shootAngle
                        cooldownMS: 1000
                    ),
                    new Suicide(200)
                )
            );

        #endregion

        #region 2ndboss

        [CharacterBehavior("shtrs Twilight Archmage")]
        public static State ShtrsTwilightArchmage =>
            new(
                // new ScaleHP2(35), // Removed in new format
                new HpLessTransition(0.1f, "Death"),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntitiesNotWithinTransition(6, "Wake", "shtrs Archmage of Flame")
                ),
                new State("Wake",
                    new State("Comment1",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new SetAltTexture(1),
                        new Taunt("Ha...ha........hahahahahaha! You will make a fine sacrifice!"),
                        new TimedTransition(5000, "Comment2")
                    ),
                    new SetAltTexture(1),
                    new State("Comment2",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new Taunt("You will find that it was...unwise...to wake me."),
                        new TimedTransition(5000, "Comment3")
                    ),
                    new State("Comment3",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new SetAltTexture(1),
                        new Taunt("Let us see what can conjure up!"),
                        new TimedTransition(5000, "Comment4")
                    ),
                    new State("Comment4",
                        new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                        new SetAltTexture(1),
                        new Taunt("I will freeze the life from you!"),
                        new TimedTransition(5000, "Shoot")
                    )
                ),
                new State("TossShit",
                    new TossObject("shtrs Ice Portal", 10, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 15, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 15, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 7, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 1, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 4, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 8, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 9, cooldownMS: 25000), // NOT IN USE!
                    new TossObject("shtrs Firebomb", 5, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 7, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 11, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 13, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 12, cooldownMS: 25000),
                    new TossObject("shtrs Firebomb", 10, cooldownMS: 25000),
                    new Spawn("shtrs Ice Shield 2", maxSpawnsPerReset: 1, cooldownMs: 25000),
                    new TimedTransition(1, "Shoot")
                ),
                new State("Shoot",
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 15,
                        count: 5,
                        shootAngle: 5,
                        cooldownMS: 800,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 15,
                        count: 5,
                        shootAngle: 5,
                        cooldownMS: 800,
                        coolDownOffset: 200,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 15,
                        count: 5,
                        shootAngle: 5,
                        cooldownMS: 800,
                        coolDownOffset: 400,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 15,
                        count: 5,
                        shootAngle: 5,
                        cooldownMS: 800,
                        coolDownOffset: 600,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 1,
                        maxRadius: 15,
                        count: 5,
                        shootAngle: 5,
                        cooldownMS: 800,
                        coolDownOffset: 800,
                        targeted: true // Added
                    ),
                    new TimedTransition(800, "Shoot"),
                    new HpLessTransition(0.50f, "Pre Birds")
                ),
                new State("Pre Birds",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("You leave me no choice...Inferno! Blizzard!"),
                    new TimedTransition(2000, "Birds")
                ),
                new State("Birds",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Spawn("shtrs Inferno", maxSpawnsPerReset: 1, cooldownMs: 1000000000),
                    new Spawn("shtrs Blizzard", maxSpawnsPerReset: 1, cooldownMs: 1000000000),
                    new TimedTransition(300, "WaitNoBirds")
                ),
                new State("WaitNoBirds",
                    new EntitiesNotWithinTransition(500, "PreNewShit2", "shtrs Inferno",
                        "shtrs Blizzard")
                ),
                new State("PreNewShit2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new ReturnToSpawn((1 * 5.55f) + 0.74f),
                    new TimedTransition(3000, "NewShit2")
                ),
                new State("NewShit2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new MoveTo(0, -6, (1 * 5.55f) + 0.74f, true),
                    new TimedTransition(3000, "Active2")
                ),
                new State("Active2",
                    new Taunt("THE POWER...IT CONSUMES ME!"),
                    new Order(2, "shtrs MagiGenerators", "vulnerable"),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 100,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 200,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 300,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 400,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 5,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 6,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 600,
                        targeted: true // Added
                    ),
                    new TimedTransition(2000, "NewShit3")
                ),
                new State("NewShit3",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new MoveTo(4, 0, (1 * 5.55f) + 0.74f, true),
                    new TimedTransition(3000, "Active3")
                ),
                new State("Active3",
                    new Taunt("THE POWER...IT CONSUMES ME!"),
                    new Order(2, "shtrs MagiGenerators", "vulnerable"),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 100,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 200,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 300,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 400,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 5,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 6,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 600,
                        targeted: true // Added
                    ),
                    new TimedTransition(2000, "NewShit4")
                ),
                new State("NewShit4",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new MoveTo(0, 13, (1 * 5.55f) + 0.74f, true),
                    new TimedTransition(3000, "Active4")
                ),
                new State("Active4",
                    new Taunt("THE POWER...IT CONSUMES ME!"),
                    new Order(2, "shtrs MagiGenerators", "vulnerable"),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 100,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 200,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 300,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 400,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 5,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 6,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 600,
                        targeted: true // Added
                    ),
                    new TimedTransition(2000, "NewShit5")
                ),
                new State("NewShit5",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new MoveTo(0, 13, (1 * 5.55f) + 0.74f, true),
                    new TimedTransition(3000, "Active5")
                ),
                new State("Active5",
                    new Taunt("THE POWER...IT CONSUMES ME!"),
                    new Order(2, "shtrs MagiGenerators", "vulnerable"),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 100,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 200,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 300,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 400,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 5,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 6,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 600,
                        targeted: true // Added
                    ),
                    new TimedTransition(2000, "NewShit6")
                ),
                new State("NewShit6",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new MoveTo(0, 13, (1 * 5.55f) + 0.74f, true),
                    new TimedTransition(3000, "Active6")
                ),
                new State("Active6",
                    new Taunt("THE POWER...IT CONSUMES ME!"),
                    new Order(2, "shtrs MagiGenerators", "vulnerable"),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 100,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 200,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 300,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 400,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 5,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 6,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 600,
                        targeted: true // Added
                    ),
                    new TimedTransition(2000, "NewShit7")
                ),
                new State("NewShit7",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new MoveTo(0, -13, (1 * 5.55f) + 0.74f, true),
                    new TimedTransition(3000, "Active7")
                ),
                new State("Active7",
                    new Taunt("THE POWER...IT CONSUMES ME!"),
                    new Order(2, "shtrs MagiGenerators", "vulnerable"),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 100,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 3,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 200,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 4,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 300,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 2,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 400,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 5,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 500,
                        targeted: true // Added
                    ),
                    new Shoot(
                        projectileIndex: 6,
                        maxRadius: 15,
                        count: 20,
                        shootAngle: 360 / 20f,
                        cooldownMS: 100000000,
                        coolDownOffset: 600,
                        targeted: true // Added
                    )
                ),
                new State("Death",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("IM..POSSI...BLE!"),
                    new TimedTransition(2000, "Suicide")
                ),
                new State("Suicide",
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 35,
                        count: 30,
                        shootAngle: 360f / 30, // Calculated shootAngle
                        cooldownMS: 1000,
                        targeted: true
                    ),
                    new Suicide()
                )
            );

        #endregion

        #region birds

        [CharacterBehavior("shtrs Inferno")]
        public static State ShtrsInferno =>
            new(
                new Orbit(
                    target: "shtrs Blizzard",
                    radius: 10,
                    speed: 8,
                    speedVariance: 0,
                    radiusVariance: 0
                ),
                new Shoot(
                    projectileIndex: 0,
                    maxRadius: 10,
                    count: 6,
                    shootAngle: 60,
                    fixedAngle: 15,
                    cooldownMS: 4333
                ),
                new Shoot(
                    projectileIndex: 0,
                    maxRadius: 10,
                    count: 6,
                    shootAngle: 60,
                    fixedAngle: 30,
                    cooldownMS: 3500
                ),
                new Shoot(
                    projectileIndex: 0,
                    maxRadius: 10,
                    count: 6,
                    shootAngle: 60,
                    fixedAngle: 60,
                    cooldownMS: 7250
                ),
                new Shoot(
                    projectileIndex: 0,
                    maxRadius: 10,
                    count: 6,
                    shootAngle: 60,
                    fixedAngle: 90,
                    cooldownMS: 10000
                )
            );

        [CharacterBehavior("shtrs Blizzard")]
        public static State ShtrsBlizzard =>
            new(
                new State("Follow",
                    new Follow(
                        (1 * 5.55f) + 0.74f,
                        1,
                        cooldownMS: 1000
                    ),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 45,
                        cooldownMS: 250
                    ),
                    new TimedTransition(7000, "Spin")
                ),
                new State("Spin",
                    new State("Quadforce1",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 0,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce2")
                    ),
                    new State("Quadforce2",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 15,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce3")
                    ),
                    new State("Quadforce3",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 30,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce4")
                    ),
                    new State("Quadforce4",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 45,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce5")
                    ),
                    new State("Quadforce5",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 60,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce6")
                    ),
                    new State("Quadforce6",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 75,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce7")
                    ),
                    new State("Quadforce7",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 90,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce8")
                    ),
                    new State("Quadforce8",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 105,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce9")
                    ),
                    new State("Quadforce9",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 120,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce10")
                    ),
                    new State("Quadforce10",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 135,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce11")
                    ),
                    new State("Quadforce11",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 150,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce12")
                    ),
                    new State("Quadforce12",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 165,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce13")
                    ),
                    new State("Quadforce13",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 180,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce14")
                    ),
                    new State("Quadforce14",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 195,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce15")
                    ),
                    new State("Quadforce15",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 210,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Quadforce16")
                    ),
                    new State("Quadforce16",
                        new Shoot(
                            projectileIndex: 0,
                            maxRadius: 10,
                            count: 6,
                            shootAngle: 60,
                            fixedAngle: 225,
                            cooldownMS: 300
                        ),
                        new TimedTransition(10, "Follow")
                    )
                )
            );

        #endregion

        #region 1stbosschest

        [CharacterBehavior("shtrs Loot Balloon Bridge")]
        public static State ShtrsLootBalloonBridge =>
            new(
                new CharacterLoot(
                    new ItemLoot("Bracer of the Guardian", 0.01f, 0.03f),
                    // new TierLoot(11, ItemType.Weapon, 0.1f, 0.0001f),
                    // new TierLoot(12, ItemType.Weapon, 0.05f, 0.0001f),
                    // new TierLoot(5, ItemType.Ability, 0.1f, 0.0001f),
                    // new TierLoot(6, ItemType.Ability, 0.05f, 0.0001f),
                    // new TierLoot(12, ItemType.Armor, 0.1f, 0.0001f),
                    // new TierLoot(13, ItemType.Armor, 0.05f, 0.0001f),
                    // new TierLoot(5, ItemType.Ring, 0.1f, 0.0001f),
                    // new TierLoot(6, ItemType.Ring, 0.05f, 0.0001f),
                    new ItemLoot("Shatters Key", 0.002f, 0.00001f),
                    new ItemLoot("Potion of Defense", 1f, 0.00001f),
                    new ItemLoot("Potion of Attack", 1f, 0.00001f)
                ),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(5000, "Bridge")
                ),
                new State("Bridge")
            );

        #endregion

        #region 2ndbosschest

        [CharacterBehavior("shtrs Loot Balloon Mage")]
        public static State ShtrsLootBalloonMage =>
            new(
                new CharacterLoot(
                    new ItemLoot("The Twilight Gemstone", 0.0025f, 0.03f),
                    new ItemLoot("Potion of Mana", 1f, 0.0001f),
                    new ItemLoot("Shatters Key", 0.002f, 0.0001f)
                    // new TierLoot(11, ItemType.Weapon, 0.1f, 0.0001f),
                    // new TierLoot(12, ItemType.Weapon, 0.05f, 0.0001f),
                    // new TierLoot(5, ItemType.Ability, 0.1f, 0.0001f),
                    // new TierLoot(6, ItemType.Ability, 0.05f, 0.0001f),
                    // new TierLoot(12, ItemType.Armor, 0.1f, 0.0001f),
                    // new TierLoot(13, ItemType.Armor, 0.05f, 0.0001f),
                    // new TierLoot(5, ItemType.Ring, 0.1f, 0.0001f),
                    // new TierLoot(6, ItemType.Ring, 0.05f, 0.0001f)
                ),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(5000, "Mage")
                ),
                new State("Mage")
            );

        #endregion

        #region 3rdbosschest

        [CharacterBehavior("shtrs Loot Balloon King")]
        public static State ShtrsLootBalloonKing =>
            new(
                new CharacterLoot(
                    new ItemLoot("The Forgotten Crown", 0.0015f, 0.03f),
                    // new SeasonalItemLoot("Ice Crown", "winter", 0.0015f, 0.03f),
                    // new TierLoot(11, ItemType.Weapon, 0.1f, 0.0001f),
                    // new TierLoot(12, ItemType.Weapon, 0.05f, 0.0001f),
                    // new TierLoot(5, ItemType.Ability, 0.1f, 0.0001f),
                    // new TierLoot(6, ItemType.Ability, 0.05f, 0.0001f),
                    // new TierLoot(12, ItemType.Armor, 0.1f, 0.0001f),
                    // new TierLoot(13, ItemType.Armor, 0.05f, 0.0001f),
                    // new TierLoot(5, ItemType.Ring, 0.1f, 0.0001f),
                    // new TierLoot(6, ItemType.Ring, 0.05f, 0.0001f),
                    new ItemLoot("Shatters Key", 0.002f, 0.00001f),
                    new ItemLoot("Potion of Life", 1f, 0.00001f)
                ),
                new DropPortalOnDeath("Realm Portal", 1.0f),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(5000, "Crown")
                ),
                new State("Crown")
            );

        #endregion

        #region BridgeStatues

        [CharacterBehavior("shtrs Bridge Obelisk A")]
        public static State ShtrsBridgeObeliskA =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntityNotWithinTransition(target: "shtrs Bridge Closer4", radius: 100, targetState: "TALK")
                ),
                new State("TALK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("DO NOT WAKE THE BRIDGE GUARDIAN!"),
                    new TimedTransition(2000, "AFK")
                ),
                new State("AFK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0x0000FF0C, 0.5f, 4),
                    new TimedTransition(2500, "Shoot")
                ),
                new State("Shoot",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 0,
                        cooldownMS: 200
                    ),
                    new TimedTransition(10000, "Pause")
                ),
                new State("Pause",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 0),
                    new Spawn("shtrs Stone Knight", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new Spawn("shtrs Stone Mage", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new TimedTransition(7000, "Shoot")
                )
            );

        [CharacterBehavior("shtrs Bridge Obelisk B")]
        public static State ShtrsBridgeObeliskB =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntityNotWithinTransition(target: "shtrs Bridge Closer4", radius: 100, targetState: "TALK")
                ),
                new State("TALK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("DO NOT WAKE THE BRIDGE GUARDIAN!"),
                    new TimedTransition(2000, "AFK")
                ),
                new State("AFK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0x0000FF0C, 0.5f, 4),
                    new TimedTransition(2500, "Shoot")
                ),
                new State("Shoot",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(projectileIndex: 0, maxRadius: 10, count: 4, shootAngle: 90, fixedAngle: 0, cooldownMS: 200),
                    new TimedTransition(10000, "Pause")
                ),
                new State("Pause",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 0),
                    new Spawn("shtrs Stone Knight", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new Spawn("shtrs Stone Mage", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new TimedTransition(7000, "Shoot")
                )
            );

        [CharacterBehavior("shtrs Bridge Obelisk D")]
        public static State ShtrsBridgeObeliskD =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntityNotWithinTransition(target: "shtrs Bridge Closer4", radius: 100, targetState: "TALK")
                ),
                new State("TALK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("DO NOT WAKE THE BRIDGE GUARDIAN!"),
                    new TimedTransition(2000, "AFK")
                ),
                new State("AFK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0x0000FF0C, 0.5f, 4),
                    new TimedTransition(2500, "Shoot")
                ),
                new State("Shoot",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 0,
                        cooldownMS: 200
                    ),
                    new TimedTransition(10000, "Pause")
                ),
                new State("Pause",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 0),
                    new Spawn("shtrs Stone Knight", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new Spawn("shtrs Stone Mage", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new TimedTransition(7000, "Shoot")
                )
            );

        [CharacterBehavior("shtrs Bridge Obelisk E")]
        public static State ShtrsBridgeObeliskE =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new EntityNotWithinTransition(target: "shtrs Bridge Closer4", radius: 100, targetState: "TALK")
                ),
                new State("TALK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("DO NOT WAKE THE BRIDGE GUARDIAN!"),
                    new TimedTransition(2000, "AFK")
                ),
                new State("AFK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0x0000FF0C, 0.5f, 4),
                    new TimedTransition(2500, "Shoot")
                ),
                new State("Shoot",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 0,
                        cooldownMS: 200
                    ),
                    new TimedTransition(10000, "Pause")
                ),
                new State("Pause",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 0),
                    new Spawn("shtrs Stone Knight", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new Spawn("shtrs Stone Mage", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new TimedTransition(7000, "Shoot")
                )
            );

        [CharacterBehavior("shtrs Bridge Obelisk C")]
        public static State ShtrsBridgeObeliskC =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored),
                    new EntityNotWithinTransition(target: "shtrs Bridge Closer4", radius: 100,
                        targetState: "JustKillMe")
                ),
                new State("JustKillMe",
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(2000, "AFK")
                ),
                new State("AFK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0x0000FF0C, 0.5f, 4),
                    new TimedTransition(2500, "Shoot")
                ),
                new State("Shoot",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(
                        projectileIndex: 0,
                        maxRadius: 10,
                        count: 4,
                        shootAngle: 90,
                        fixedAngle: 0,
                        cooldownMS: 200
                    ),
                    new TimedTransition(10000, "Pause")
                ),
                new State("Pause",
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored),
                    new Spawn("shtrs Stone Paladin", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new TimedTransition(7000, "Shoot")
                )
            );

        [CharacterBehavior("shtrs Bridge Obelisk F")]
        public static State ShtrsBridgeObeliskF =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored),
                    new EntityNotWithinTransition("shtrs Bridge Closer4", 100f, targetStates: "JustKillMe")
                ),
                new State("JustKillMe",
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(2000, "AFK")
                ),
                new State("AFK",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored),
                    new Flash(0x0000FF0C, 0.5f, 4),
                    new TimedTransition(2500, "Shoot")
                ),
                new State("Shoot",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(10, projectileIndex: 0, count: 4, shootAngle: 90, fixedAngle: 0, cooldownMS: 200),
                    new TimedTransition(10000, "Pause")
                ),
                new State("Pause",
                    new ConditionEffectBehavior(ConditionEffectIndex.Armored),
                    new Spawn("shtrs Stone Paladin", maxSpawnsPerReset: 1, cooldownMs: 99999),
                    new TimedTransition(7000, "Shoot")
                )
            );

        #endregion

        #region SomeMobs

        // [CharacterBehavior("shtrs obelisk controller")]
        // public static State ShtrsObeliskController =>
        //     new State(
        //         new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
        //         new State("wait"
        //             // new EntitiesNotWithinTransition(30, "obeliskshoot", "shtrs Bridge Obelisk A",
        //             //     "shtrs Bridge Obelisk B", "shtrs Bridge Obelisk D", "shtrs Bridge Obelisk E")
        //         ),
        //         new State("obeliskshoot",
        //             new Order(60, "shtrs Bridge Obelisk A", "Shoot"),
        //             new Order(60, "shtrs Bridge Obelisk B", "Shoot"),
        //             new Order(60, "shtrs Bridge Obelisk D", "Shoot"),
        //             new Order(60, "shtrs Bridge Obelisk E", "Shoot")
        //         ),
        //         new State("guardiancheck",
        //             new Order(60, "shtrs Bridge Obelisk A", "Pause"),
        //             new Order(60, "shtrs Bridge Obelisk B", "Pause"),
        //             new Order(60, "shtrs Bridge Obelisk D", "Pause"),
        //             new Order(60, "shtrs Bridge Obelisk E", "Pause"),
        //             new TimedTransition(1, "leavemychecksalone")
        //         ),
        //         new State("leavemychecksalone",
        //             new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true)
        //         )
        //     );
        //
        // [CharacterBehavior("shtrs obelisk timer")]
        // public static State ShtrsObeliskTimer =>
        //     new State(
        //         new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
        //         new State("wait"
        //             // new EntitiesNotWithinTransition(30, "timer1", "shtrs Bridge Obelisk A", "shtrs Bridge Obelisk B",
        //             //     "shtrs Bridge Obelisk D", "shtrs Bridge Obelisk E")
        //         ),
        //         new State("timer1",
        //             new Order(60, "shtrs obelisk controller", "obeliskshoot"),
        //             new TimedTransition(10000, "guardiancheck")
        //         ),
        //         new State("guardiancheck",
        //             new Order(60, "shtrs obelisk controller", "guardiancheck"),
        //             new TimedTransition(1, "leavemychecksalone")
        //         ),
        //         new State("leavemychecksalone",
        //             new TimedTransition(7000, "timer1")
        //         )
        //     );

        [CharacterBehavior("shtrs Titanum")]
        public static State ShtrsTitanum =>
            new(
                new State("Wait",
                    new EntityWithinTransition("spawn", radius: 7)
                ),
                new State("spawn",
                    new Spawn("shtrs Stone Knight", maxSpawnsPerReset: 1, cooldownMs: 5000),
                    new Spawn("shtrs Stone Mage", maxSpawnsPerReset: 1, cooldownMs: 7500),
                    new TimedTransition(5000, "Wait")
                )
            );

        [CharacterBehavior("shtrs Paladin Obelisk")]
        public static State ShtrsPaladinObelisk =>
            new(
                new State("Wait",
                    new EntityWithinTransition("spawn", radius: 5)
                ),
                new State("spawn",
                    new Spawn("shtrs Stone Paladin", maxSpawnsPerReset: 1, cooldownMs: 7500)
                )
            );

        [CharacterBehavior("shtrs Ice Mage")]
        public static State ShtrsIceMage =>
            new(
                new State("Wait",
                    new EntityWithinTransition("fire", radius: 5)
                ),
                new State("fire",
                    new Follow(3.47f, 1), // Speed converted: 0.5 * 5.55 + 0.74 = 3.47f
                    new Shoot(10, 5, 10, 0, cooldownMS: 1500, targeted: true),
                    new TimedTransition(15000, "Spawn")
                ),
                new State("Spawn",
                    new Spawn("shtrs Ice Shield", maxSpawnsPerReset: 1, cooldownMs: 750000000),
                    new TimedTransition(25, "fire")
                )
            );

        [CharacterBehavior("shtrs Archmage of Flame")]
        public static State ShtrsArchmageOfFlame =>
            new(
                new State("wait",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new EntityWithinTransition(radius: 7, targetStates: "Follow")
                ),
                new State("Follow",
                    new Follow(6.29f, 1), // Speed converted: 1 * 5.55 + 0.74 = 6.29f
                    new TimedTransition(5000, "Throw")
                ),
                new State("Throw",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TossObject("shtrs Firebomb", 1, 90, 5000),
                    new TossObject("shtrs Firebomb", 2, 20, 5000),
                    new TossObject("shtrs Firebomb", 3, 72, 5000),
                    new TossObject("shtrs Firebomb", 4, 270, 5000),
                    new TossObject("shtrs Firebomb", 5, 140, 5000),
                    new TossObject("shtrs Firebomb", 6, 220, 5000),
                    new TossObject("shtrs Firebomb", 6, 48, 5000),
                    new TossObject("shtrs Firebomb", 5, 56, 5000),
                    new TossObject("shtrs Firebomb", 4, 180, 5000),
                    new TossObject("shtrs Firebomb", 3, 30, 5000),
                    new TossObject("shtrs Firebomb", 2, 0, 5000),
                    new TossObject("shtrs Firebomb", 1, 190, 5000),
                    new TimedTransition(4000, "Fire")
                ),
                new State("Fire",
                    new Shoot(0, projectileIndex: 0, count: 1, shootAngle: 45, fixedAngle: 45, cooldownMS: 170,
                        coolDownOffset: 0),
                    new Shoot(0, projectileIndex: 0, count: 1, shootAngle: 90, fixedAngle: 90, cooldownMS: 170,
                        coolDownOffset: 0),
                    new Shoot(0, projectileIndex: 0, count: 1, shootAngle: 135, fixedAngle: 135, cooldownMS: 170,
                        coolDownOffset: 0),
                    new Shoot(0, projectileIndex: 0, count: 1, shootAngle: 180, fixedAngle: 180, cooldownMS: 170,
                        coolDownOffset: 0),
                    new Shoot(0, projectileIndex: 0, count: 1, shootAngle: 225, fixedAngle: 225, cooldownMS: 170,
                        coolDownOffset: 0),
                    new Shoot(0, projectileIndex: 0, count: 1, shootAngle: 270, fixedAngle: 270, cooldownMS: 170,
                        coolDownOffset: 0),
                    new Shoot(0, projectileIndex: 0, count: 1, shootAngle: 315, fixedAngle: 315, cooldownMS: 170,
                        coolDownOffset: 0),
                    new Shoot(0, projectileIndex: 0, count: 1, shootAngle: 360, fixedAngle: 360, cooldownMS: 170,
                        coolDownOffset: 0),
                    new TimedTransition(5000, "wait")
                )
            );

        [CharacterBehavior("shtrs Firebomb")]
        public static State ShtrsFirebomb =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new TimedTransition(2000, "Explode")
                ),
                new State("Explode",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Shoot(100, shootAngle: 360 / 8f, projectileIndex: 0, count: 8, targeted: false,
                        cooldownMS: 1000),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Fire Mage")]
        public static State ShtrsFireMage =>
            new(
                new State("Wait",
                    new EntityWithinTransition("fire", radius: 5)
                ),
                new State("fire",
                    new Follow(3.47f, 1), // Speed converted: 0.5 * 5.55 + 0.74 = 3.47f
                    new Shoot(10, 5, 10, 0, cooldownMS: 4500, targeted: true),
                    new TimedTransition(10000, "nothing")
                ),
                new State("nothing",
                    new TimedTransition(1000, "fire")
                )
            );

        [CharacterBehavior("shtrs Stone Mage")]
        public static State ShtrsStoneMage =>
            new(
                new State("Wait",
                    new EntityWithinTransition("fire", radius: 10)
                ),
                new State("fire",
                    new Follow(3.47f, 1), // Speed converted: 0.5 * 5.55 + 0.74 = 3.47f
                    new Shoot(10, 2, 10, 1, cooldownMS: 200, targeted: true),
                    new TimedTransition(10000, "invulnerable")
                ),
                new State("invulnerable",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Shoot(10, 2, 10, 0, cooldownMS: 200, targeted: true),
                    new TimedTransition(3000, "fire")
                )
            );

        #endregion

        #region WOODENGATESSWITCHESBRIDGES

        [CharacterBehavior("shtrs Wooden Gate 3")]
        public static State ShtrsWoodenGate3 =>
            new(
                new State("Despawn",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("shtrs Wooden Gate")]
        public static State ShtrsWoodenGate =>
            new(
                new State("Idle",
                    new EntityNotWithinTransition("shtrs Abandoned Switch 1", 10, targetStates: "Despawn")
                ),
                new State("Despawn",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("shtrs Wooden Gate 4")]
        public static State ShtrsWoodenGate4 =>
            new(
                new State("Idle",
                    new EntityNotWithinTransition("shtrs Abandoned Switch 1", 10, targetStates: "Despawn")
                ),
                new State("Despawn",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("shtrs Wooden Gate 5")]
        public static State ShtrsWoodenGate5 =>
            new(
                new State("Idle",
                    new EntityNotWithinTransition("shtrs Abandoned Switch 2", 60, targetStates: "Despawn")
                ),
                new State("Despawn",
                    new Suicide(100)
                )
            );

        [CharacterBehavior("shtrs Wooden Gate Spawn")]
        public static State ShtrsWoodenGateSpawn =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Waiting",
                    new EntitiesNotWithinTransition(10, "KillWall", "shtrs Abandoned Switch 1")
                ),
                new State("KillWall",
                    new OpenGate("shtrs Wooden Gate", 10),
                    new Suicide(100)
                )
            );

        [CharacterBehavior("shtrs Wooden Gate Spawn 2")]
        public static State ShtrsWoodenGateSpawn2 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Waiting",
                    new EntitiesNotWithinTransition(70, "KillWall", "shtrs Abandoned Switch 2")
                ),
                new State("KillWall",
                    new OpenGate("shtrs Wooden Gate 2", 10),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Abandoned Switch 1")]
        public static State ShtrsAbandonedSwitch1 =>
            new(
                new RemoveObjectOnDeath("shtrs Wooden Gate", 8)
            );

        [CharacterBehavior("Tooky Shatters Master")]
        public static State TookyShattersMaster =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible)
            );

        [CharacterBehavior("shtrs Bridge Closer")]
        public static State ShtrsBridgeCloser =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true)
                ),
                new State("Closer",
                    new ChangeGround(new[] { "shtrs Bridge" }, new[] { "shtrs Pure Evil" }, 1),
                    new Suicide(100)
                ),
                new State("TwilightClose",
                    new ChangeGround(new[] { "shtrs Shattered Floor", "shtrs Disaster Floor" },
                        new[] { "shtrs Pure Evil" }, 1),
                    new Suicide(100)
                )
            );

        [CharacterBehavior("shtrs Bridge Closer2")]
        public static State ShtrsBridgeCloser2 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true)
                ),
                new State("Closer",
                    new ChangeGround(new[] { "shtrs Bridge" }, new[] { "shtrs Pure Evil" }, 1),
                    new Suicide(100)
                ),
                new State("TwilightClose",
                    new ChangeGround(new[] { "shtrs Shattered Floor", "shtrs Disaster Floor" },
                        new[] { "shtrs Pure Evil" }, 1),
                    new Suicide(100)
                )
            );

        [CharacterBehavior("shtrs Bridge Closer3")]
        public static State ShtrsBridgeCloser3 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true)
                ),
                new State("Closer",
                    new ChangeGround(new[] { "shtrs Bridge" }, new[] { "shtrs Pure Evil" }, 1),
                    new Suicide(100)
                ),
                new State("TwilightClose",
                    new ChangeGround(new[] { "shtrs Shattered Floor", "shtrs Disaster Floor" },
                        new[] { "shtrs Pure Evil" }, 1),
                    new Suicide(100)
                )
            );

        [CharacterBehavior("shtrs Bridge Closer4")]
        public static State ShtrsBridgeCloser4 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true)
                ),
                new State("Closer",
                    new ChangeGround(new[] { "shtrs Bridge" }, new[] { "shtrs Pure Evil" }, 1),
                    new Suicide(0)
                ),
                new State("TwilightClose",
                    new ChangeGround(new[] { "shtrs Shattered Floor", "shtrs Disaster Floor" },
                        new[] { "shtrs Pure Evil" }, 1),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Spawn Bridge")]
        public static State ShtrsSpawnBridge =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("Open", "shtrs Bridge Sentinel", 100)
                ),
                new State("Open",
                    new ChangeGround(new[] { "shtrs Pure Evil" }, new[] { "shtrs Bridge" }, 1),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Spawn Bridge 2")]
        public static State ShtrsSpawnBridge2 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("shtrs Abandoned Switch 3", 500, targetStates: "Open")
                ),
                new State("Open",
                    new ChangeGround(new[] { "shtrs Pure Evil" }, new[] { "shtrs Shattered Floor" }, 1),
                    new Suicide()
                ),
                new State("CloseBridge2",
                    new ChangeGround(new[] { "shtrs Shattered Floor" }, new[] { "shtrs Pure Evil" }, 1),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Spawn Bridge 3")]
        public static State ShtrsSpawnBridge3 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("shtrs Twilight Archmage", 500, targetStates: "Open")
                ),
                new State("Open",
                    new ChangeGround(new[] { "shtrs Pure Evil" }, new[] { "shtrs Shattered Floor" }, 1),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Spawn Bridge 5")]
        public static State ShtrsSpawnBridge5 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("shtrs Royal Guardian L", 100, targetStates: "Open")
                ),
                new State("Open",
                    new ChangeGround(new[] { "Dark Cobblestone" }, new[] { "Hot Lava" }, 1),
                    new Suicide()
                )
            );

        #endregion
    }
}