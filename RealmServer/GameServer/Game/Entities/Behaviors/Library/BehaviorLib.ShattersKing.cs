#region

using Common;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public partial class BehaviorLib
    {
        #region 3rdboss

        [CharacterBehavior("shtrs The Forgotten King")]
        public static State ShtrsTheForgottenKing =>
            new(
                new HpLessTransition(.02f, "Death"),
                // new HpScaling(10000),
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new SetAltTexture(1),
                    new TimedTransition(4000, "Crystals")
                ),
                new State("Crystals",
                    new Spawn("shtrs Green Crystal", cooldownMs: 100000),
                    new Spawn("shtrs Yellow Crystal", cooldownMs: 100000),
                    new Spawn("shtrs Red Crystal", cooldownMs: 100000),
                    new Spawn("shtrs Blue Crystal", cooldownMs: 100000),
                    new TimedTransition(3000, "WaitCrystals")
                ),
                new State("WaitCrystals",
                    new Taunt("I will make quick work of you. Be gone."),
                    new EntitiesNotWithinTransition(999, "Swirl1", "shtrs Green Crystal", "shtrs Yellow Crystal",
                        "shtrs Red Crystal", "shtrs Blue Crystal")
                ),
                new State("Swirl1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 0),
                    new HpLessTransition(.7f, "SpawnLava"),
                    new SetAltTexture(0),
                    new Flash(0xff0000, .5, 4),
                    new ChangeSize(75, 150),
                    new Shoot(15, 2, 1, 2, cooldownMS: 800, targeted: true),
                    new Shoot(15, 2, 1, 3, coolDownOffset: 200, cooldownMS: 800, targeted: true),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1000),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1000),
                    new Shoot(40, 1, fixedAngle: 103, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1150),
                    new Shoot(40, 1, fixedAngle: 77, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1150),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1250),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1250),
                    new Shoot(40, 1, fixedAngle: 100, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1310),
                    new Shoot(40, 1, fixedAngle: 80, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1310),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1400),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1400),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1550),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1550),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1650),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1650),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1750),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1750),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1850),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1850),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1950),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1950),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2050),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2050),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2150),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2150),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2250),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2250),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2350),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2350),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2450),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2450),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2550),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2550),
                    new Shoot(40, 1, fixedAngle: 100, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2610),
                    new Shoot(40, 1, fixedAngle: 80, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2610),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2680),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2680),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2830),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2830),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2980),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2980),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3030),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3030),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3180),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3180),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3230),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3230),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3380),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3380),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3530),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3530),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3680),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3680),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3830),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3830),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3980),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3980),
                    new TimedTransition(5100, "Guardians")
                ),
                new State("Guardians",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new SetAltTexture(1),
                    new Spawn("shtrs Royal Guardian J", maxSpawnsPerReset: 7, cooldownMs: 100000),
                    new TimedTransition(500, "MiddleMove")
                ),
                new State("MiddleMove",
                    new MoveTo(0, 9f, 4f, true),
                    new Taunt(
                        "You fools. Guards, surround me.||Guards, come to my aid!||Do not think, even for a moment, that you will best me."),
                    new TimedTransition(3000, "GuardianWait")
                ),
                new State("GuardianWait",
                    new EntityNotWithinTransition("shtrs Royal Guardian J", 10, targetStates: "GuardianWait2")
                ),
                new State("GuardianWait2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 0),
                    new Taunt(
                        "Be consumed, you wretched whelps.||Laughable attempt.||Do not mistake your success so far as progress."),
                    new Shoot(15, 2, 1, 2, cooldownMS: 800, targeted: true),
                    new Shoot(15, 2, 1, 3, coolDownOffset: 200, cooldownMS: 800, targeted: true),
                    new Shoot(15, 8, 360 / 8f, 1, cooldownMS: 1000, targeted: true),
                    new TimedTransition(4000, "GuardianSpawn"),
                    new HpLessTransition(.7f, "SpawnLava")
                ),
                new State("GuardianSpawn",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Spawn("shtrs Royal Guardian J", maxSpawnsPerReset: 7),
                    new Taunt(
                        "You fools. Guards, surround me.||Guards, come to my aid!||Do not think, even for a moment, that you will best me."),
                    new TimedTransition(3000, "GuardianWait")
                ),
                new State("SpawnLava",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new SetAltTexture(1),
                    new Taunt("So you are still here. Adorable. Perhaps a nice dip in the LAVA!?"),
                    new Order(50, "shtrs Final Mediator Lava", "Wait2"),
                    new TimedTransition(2000, "Tentacles")
                ),
                new State("Tentacles",
                    new Order(50, "shtrs Soul Spawner", "Spawn"),
                    new Taunt("TENTACLES OF WRATH!"),
                    new Shoot(15, 2, 1, 2, cooldownMS: 800, targeted: true),
                    new Shoot(15, 2, 1, 3, coolDownOffset: 200, cooldownMS: 800, targeted: true),
                    new Spawn("shtrs Laser1", cooldownMs: 100000),
                    new Spawn("shtrs Laser2", cooldownMs: 100000),
                    new Spawn("shtrs Laser3", cooldownMs: 100000),
                    new Spawn("shtrs Laser4", cooldownMs: 100000),
                    new Spawn("shtrs Laser5", cooldownMs: 100000),
                    new Spawn("shtrs Laser6", cooldownMs: 100000),
                    new HpLessTransition(.5f, "MoveBack"),
                    new TimedTransition(4200, "Tentacles2")
                ),
                new State("Tentacles2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new HpLessTransition(.5f, "MoveBack"),
                    new TimedTransition(19800, "RageBreak")
                ),
                new State("RageBreak",
                    new Order(5, "shtrs Laser1", "Suicide"),
                    new Order(5, "shtrs Laser2", "Suicide"),
                    new Order(5, "shtrs Laser3", "Suicide"),
                    new Order(5, "shtrs Laser4", "Suicide"),
                    new Order(5, "shtrs Laser5", "Suicide"),
                    new Order(5, "shtrs Laser6", "Suicide"),
                    new TimedTransition(4000, "Tentacles")
                ),
                new State("MoveBack",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Order(5, "shtrs Laser1", "Suicide"),
                    new Order(5, "shtrs Laser2", "Suicide"),
                    new Order(5, "shtrs Laser3", "Suicide"),
                    new Order(5, "shtrs Laser4", "Suicide"),
                    new Order(5, "shtrs Laser5", "Suicide"),
                    new Order(5, "shtrs Laser6", "Suicide"),
                    new Taunt("Drown and be swallowed by those who have failed before."),
                    new TimedTransition(3000, "Drown")
                ),
                new State("Drown",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new SetAltTexture(0),
                    new Shoot(15, 2, 1, 2, cooldownMS: 800, targeted: true),
                    new Shoot(15, 2, 1, 3, coolDownOffset: 200, cooldownMS: 800, targeted: true),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1000),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1000),
                    new Shoot(40, 1, fixedAngle: 103, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1150),
                    new Shoot(40, 1, fixedAngle: 77, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1150),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1250),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1250),
                    new Shoot(40, 1, fixedAngle: 100, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1310),
                    new Shoot(40, 1, fixedAngle: 80, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1310),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1400),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1400),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1550),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1550),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1650),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1650),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1750),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1750),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1850),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1850),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1950),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1950),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2050),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2050),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2150),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2150),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2250),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2250),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2350),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2350),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2450),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2450),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2550),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2550),
                    new Shoot(40, 1, fixedAngle: 100, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2610),
                    new Shoot(40, 1, fixedAngle: 80, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2610),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2680),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2680),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2830),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2830),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2980),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2980),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3030),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3030),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3180),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3180),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3230),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3230),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3380),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3380),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3530),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3530),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3680),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3680),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3830),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3830),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3980),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3980),
                    new HpLessTransition(.3f, "God"),
                    new TimedTransition(8000, "DrownTransit")
                ),
                new State("GodTransit",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Taunt("YOU TEST THE PATIENCE OF A GOD!")
                ),
                new State("God",
                    new Taunt("DIE! DIE! DIE!!!"),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Shoot(15, 2, 1, 2, cooldownMS: 800, targeted: true),
                    new Shoot(15, 2, 1, 3, coolDownOffset: 200, cooldownMS: 800, targeted: true),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 400),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 700),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1000),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1000),
                    new Shoot(40, 1, fixedAngle: 103, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1150),
                    new Shoot(40, 1, fixedAngle: 77, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1150),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1250),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1250),
                    new Shoot(40, 1, fixedAngle: 100, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1310),
                    new Shoot(40, 1, fixedAngle: 80, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1310),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1400),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1400),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1550),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1550),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1650),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1650),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1750),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1750),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1850),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1850),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1950),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 1950),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2050),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2050),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2150),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2150),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2250),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2250),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2350),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2350),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2450),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2450),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2550),
                    new Shoot(40, 1, fixedAngle: 90, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2550),
                    new Shoot(40, 1, fixedAngle: 100, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2610),
                    new Shoot(40, 1, fixedAngle: 80, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2610),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2680),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2680),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2830),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2830),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2980),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 2980),
                    new Shoot(40, 1, fixedAngle: 180, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3030),
                    new Shoot(40, 1, fixedAngle: 0, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3030),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3180),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3180),
                    new Shoot(40, 1, fixedAngle: 169, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3230),
                    new Shoot(40, 1, fixedAngle: 11, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3230),
                    new Shoot(40, 1, fixedAngle: 158, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3380),
                    new Shoot(40, 1, fixedAngle: 22, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3380),
                    new Shoot(40, 1, fixedAngle: 147, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3530),
                    new Shoot(40, 1, fixedAngle: 33, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3530),
                    new Shoot(40, 1, fixedAngle: 135, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3680),
                    new Shoot(40, 1, fixedAngle: 45, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3680),
                    new Shoot(40, 1, fixedAngle: 124, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3830),
                    new Shoot(40, 1, fixedAngle: 56, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3830),
                    new Shoot(40, 1, fixedAngle: 117, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3980),
                    new Shoot(40, 1, fixedAngle: 63, projectileIndex: 1, cooldownMS: 9999, coolDownOffset: 3980),
                    new Spawn("shtrs Laser1", cooldownMs: 100000),
                    new Spawn("shtrs Laser2", cooldownMs: 100000),
                    new Spawn("shtrs Laser3", cooldownMs: 100000),
                    new Spawn("shtrs Laser4", cooldownMs: 100000),
                    new Spawn("shtrs Laser5", cooldownMs: 100000),
                    new Spawn("shtrs Laser6", cooldownMs: 100000),
                    new TimedTransition(21000, "Wait")
                ),
                new State("Wait",
                    new Taunt("Ha....Haaa...haaaa"),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 0),
                    new Order(5, "shtrs Laser1", "Suicide"),
                    new Order(5, "shtrs Laser2", "Suicide"),
                    new Order(5, "shtrs Laser3", "Suicide"),
                    new Order(5, "shtrs Laser4", "Suicide"),
                    new Order(5, "shtrs Laser5", "Suicide"),
                    new Order(5, "shtrs Laser6", "Suicide"),
                    new TimedTransition(6000, "God")
                ),
                new State("Death",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Order(50, "shtrs Soul Spawner", "Idle"),
                    new Taunt("Impossible..........IMPOSSIBLE!"),
                    new TimedTransition(2000, "Suicide")
                ),
                new State("Suicide",
                    new Spawn("shtrs Chest Spawner 3"),
                    new Shoot(35, 30, 360 / 30f, 0, cooldownMS: 1000),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Laser1")]
        public static State ShtrsLaser1 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Shoot",
                    new EntityNotWithinTransition("shtrs The Forgotten King", 10, targetStates: "Suicide"),
                    new Shoot(14, 2, 3, fixedAngle: 62, rotateAngle: 3, cooldownMS: 200)
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Laser2")]
        public static State ShtrsLaser2 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Shoot",
                    new EntityNotWithinTransition("shtrs The Forgotten King", 10, targetStates: "Suicide"),
                    new Shoot(14, 2, 3, fixedAngle: 122, rotateAngle: 3, cooldownMS: 200)
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Laser3")]
        public static State ShtrsLaser3 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Shoot",
                    new EntityNotWithinTransition("shtrs The Forgotten King", 10, targetStates: "Suicide"),
                    new Shoot(14, 2, 3, fixedAngle: 182, rotateAngle: 3, cooldownMS: 200)
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Laser4")]
        public static State ShtrsLaser4 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Shoot",
                    new EntityNotWithinTransition("shtrs The Forgotten King", 10, targetStates: "Suicide"),
                    new Shoot(14, 2, 3, fixedAngle: 242, rotateAngle: 3, cooldownMS: 200)
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Laser5")]
        public static State ShtrsLaser5 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Shoot",
                    new EntityNotWithinTransition("shtrs The Forgotten King", 10, targetStates: "Suicide"),
                    new Shoot(14, 2, 3, fixedAngle: 302, rotateAngle: 3, cooldownMS: 200)
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Laser6")]
        public static State ShtrsLaser6 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Shoot",
                    new EntityNotWithinTransition("shtrs The Forgotten King", 10, targetStates: "Suicide"),
                    new Shoot(14, 2, 3, fixedAngle: 362, rotateAngle: 3, cooldownMS: 200)
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Royal Guardian J")]
        public static State ShtrsRoyalGuardianJ =>
            new(
                // new ScaleHP2(5),
                new State("shoot",
                    new Orbit(6, 5, 5, "shtrs The Forgotten King", 0, 0),
                    new Shoot(15, 8, 360 / 8f, 0, cooldownMS: 2000, targeted: true)
                )
            );

        [CharacterBehavior("shtrs Royal Guardian L")]
        public static State ShtrsRoyalGuardianL =>
            new(
                // new ScaleHP2(20),
                new State("Idle"),
                new State("1st",
                    new Follow((1 * 5.55f) + 0.74f, 8, 5),
                    new Shoot(15, 20, 360 / 20f, 0, cooldownMS: 1000, targeted: true),
                    new TimedTransition(1000, "2nd")
                ),
                new State("2nd",
                    new Follow((1 * 5.55f) + 0.74f, 8, 5),
                    new Shoot(10, projectileIndex: 1, cooldownMS: 1000, targeted: true),
                    new TimedTransition(1000, "3rd")
                ),
                new State("3rd",
                    new Follow((1 * 5.55f) + 0.74f, 8, 5),
                    new Shoot(10, projectileIndex: 1, cooldownMS: 1000, targeted: true),
                    new TimedTransition(1000, "1st")
                )
            );

        [CharacterBehavior("shtrs Green Crystal")]
        public static State shtrsGreenCrystal =>
            new(
                new HealGroup(30, "Crystals", 2000, 1500),
                new State("spawn",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit((3 * 5.55f) + 0.74f, 1, 5, "shtrs The Forgotten King", .2f, .5f),
                    new TimedTransition(7000, "follow")
                ),
                new State("follow",
                    new Follow((1 * 5.55f) + 0.74f, 6),
                    new Follow((1 * 5.55f) + 0.74f, 2),
                    new TimedTransition(3000, "dafuq")
                ),
                new State("dafuq",
                    new Orbit((3 * 5.55f) + 0.74f, 4, 30, "shtrs Crystal Tracker", .2f, .5f),
                    new Follow((1 * 5.55f) + 0.74f, 6),
                    new Follow((1 * 5.55f) + 0.74f, 2, cooldownOffsetMS: 2000, cooldownMS: 1500),
                    new TimedTransition(2000, "follow")
                )
            );

        [CharacterBehavior("shtrs Yellow Crystal")]
        public static State shtrsYellowCrystal =>
            new(
                new State("spawn",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit((3 * 5.55f) + 0.74f, 1, 5, "shtrs The Forgotten King", .2f, .5f),
                    new TimedTransition(7000, "follow")
                ),
                new State("follow",
                    new Follow((1 * 5.55f) + 0.74f, 6),
                    new TimedTransition(25, "dafuq")
                ),
                new State("dafuq",
                    new Orbit((3 * 5.55f) + 0.74f, 4, 30, "shtrs Crystal Tracker", .2f, .5f),
                    new Follow((1 * 5.55f) + 0.74f, 6),
                    new Shoot(5, 4, 4, 0, cooldownMS: 1000, targeted: true)
                )
            );

        [CharacterBehavior("shtrs Red Crystal")]
        public static State shtrsRedCrystal =>
            new(
                new State("spawn",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit((3 * 5.55f) + 0.74f, 1, 5, "shtrs The Forgotten King", .2f, .5f),
                    new TimedTransition(7000, "orbit")
                ),
                new State("orbit",
                    new TossObject("shtrs Fire Portal", 5, cooldownMS: 8000),
                    new Orbit((3 * 5.55f) + 0.74f, 4, 10, "shtrs Crystal Tracker", .2f, .5f),
                    new Follow((1 * 5.55f) + 0.74f, 6),
                    new TimedTransition(5000, "ThrowPortal")
                ),
                new State("ThrowPortal",
                    new Orbit((3 * 5.55f) + 0.74f, 4, 10, "shtrs Crystal Tracker", .2f, .5f),
                    new Follow((1 * 5.55f) + 0.74f, 6),
                    new TossObject("shtrs Fire Portal", 5, cooldownMS: 8000),
                    new TimedTransition(8000, "orbit")
                )
            );

        [CharacterBehavior("shtrs Blue Crystal")]
        public static State shtrsBlueCrystal =>
            new(
                new State("spawn",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Orbit((3 * 5.55f) + 0.74f, 1, 5, "shtrs The Forgotten King", .2f, .5f),
                    new TimedTransition(7000, "orbit")
                ),
                new State("orbit",
                    new TossObject("shtrs Ice Portal", 5, cooldownMS: 8000),
                    new Orbit((3 * 5.55f) + 0.74f, 4, 10, "shtrs Crystal Tracker", .2f, .5f),
                    new Follow((0.4f * 5.55f) + 0.74f, 6),
                    new TimedTransition(5000, "ThrowPortal")
                ),
                new State("ThrowPortal",
                    new Orbit((3 * 5.55f) + 0.74f, 4, 10, "shtrs Crystal Tracker", .2f, .5f),
                    new Follow((1.4f * 5.55f) + 0.74f, 6),
                    new TossObject("shtrs Ice Portal", 5, cooldownMS: 8000),
                    new TimedTransition(8000, "orbit")
                )
            );

        [CharacterBehavior("shtrs Crystal Tracker")]
        public static State shtrsCrystalTracker =>
            new(
                new Follow((1 * 5.55f) + 0.74f, 10, 1)
            );

        [CharacterBehavior("shtrs king timer")]
        public static State ShtrsKingTimer =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("wait",
                    new EntityNotWithinTransition("shtrs The Forgotten King", 100, targetStates: "death")
                ),
                new State("timer1",
                    new TimedTransition(28000, "heheh")
                ),
                new State("heheh",
                    new Order(60, "shtrs The Forgotten King", "heheh"),
                    new TimedTransition(1, "wait")
                ),
                new State("death",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs king lava1")]
        public static State ShtrsKingLava1 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("wait",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invisible)
                ),
                new State("lava",
                    new ChangeGround(["Dark Cobblestone"], ["Hot Lava"], 20)
                ),
                new State("death",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs king lava2")]
        public static State ShtrsKingLava2 =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("wait",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invisible)
                ),
                new State("lava",
                    new ChangeGround(["Dark Cobblestone"], ["Hot Lava"], 20)
                ),
                new State("death",
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs The Cursed Crown")]
        public static State ShtrsTheCursedCrown =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityWithinTransition("Start", radius: 5)
                ),
                new State("Start",
                    new Order(20, "shtrs Royal Guardian L", "1st"),
                    new EntityNotWithinTransition("shtrs Royal Guardian L", 100, targetStates: "Open")
                ),
                new State("Open",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new MoveTo(0, -12, (0.5f * 5.55f) + 0.74f, true),
                    new TimedTransition(4000, "WADAFAK")
                ),
                new State("WADAFAK",
                    new TransformOnDeath("shtrs The Forgotten King"),
                    new Suicide()
                )
            );

        #endregion

        #region MISC

        [CharacterBehavior("shtrs Chest Spawner 1")]
        public static State ShtrsChestSpawner1 =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("shtrs Bridge Sentinel", 500, targetStates: "Open")
                ),
                new State("Open",
                    new TransformOnDeath("shtrs Loot Balloon Bridge"),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs Chest Spawner 2")]
        public static State ShtrsChestSpawner2 =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition("shtrs Twilight Archmage", 500, targetStates: "Open")
                ),
                new State("Open",
                    new TransformOnDeath("shtrs Loot Balloon Mage"),
                    new Suicide()
                )
            );

        [CharacterBehavior("shtrs blobomb maker")]
        public static State ShtrsBlobombMaker =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible)
                ),
                new State("Spawn",
                    new Spawn("shtrs Blobomb", cooldownMs: 2000),
                    new TimedTransition(8000, "Idle")
                )
            );

        [CharacterBehavior("shtrs Lava Souls maker")]
        public static State ShtrsLavaSoulsMaker =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible)
                ),
                new State("Spawn",
                    new Spawn("shtrs Lava Souls", maxSpawnsPerReset: 1, cooldownMs: 8000),
                    new TimedTransition(8000, "Spawn2")
                ),
                new State("Spawn2",
                    new Spawn("shtrs Lava Souls", maxSpawnsPerReset: 1, cooldownMs: 8000),
                    new TimedTransition(8000, "Spawn")
                )
            );

        [CharacterBehavior("shtrs Chest Spawner 3")]
        public static State ShtrsChestSpawner3 =>
            new(
                new State("Idle",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntitiesNotWithinTransition(30, "Open", "shtrs The Forgotten King")
                ),
                new State("Open",
                    new TransformOnDeath("shtrs Loot Balloon King"),
                    new Suicide()
                )
            );

        #endregion
    }
}