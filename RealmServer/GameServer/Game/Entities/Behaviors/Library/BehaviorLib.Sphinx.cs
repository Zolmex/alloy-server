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
        [CharacterBehavior("Grand Sphinx")]
        public static State GrandSphinx =>
            new(
                new DropPortalOnDeath("Tomb of the Ancients Portal", 1.0f),
                new State("Spawned",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Reproduce("Horrid Reaper", densityRadius: 30, maxDensity: 4, cooldownMs: 100),
                    new TimedTransition(500, "Attack1")
                ),
                new State("Attack1",
                    new Wander((0.5f * 5.55f) + 0.74f),
                    new Shoot(12, 1, cooldownMS: 800, targeted: true),
                    new Shoot(12, 3, 10, cooldownMS: 1000, targeted: true),
                    new Shoot(12, 1, 130, cooldownMS: 1000, targeted: true),
                    new Shoot(12, 1, 230, cooldownMS: 1000, targeted: true),
                    new TimedTransition(6000, "TransAttack2")
                ),
                new State("TransAttack2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Wander((0.5f * 5.55f) + 0.74f),
                    new Flash(0x00FF0C, .25, 8),
                    new Taunt("You hide behind rocks like cowards but you cannot hide from this!"),
                    new TimedTransition(2000, "Attack2")
                ),
                new State("Attack2",
                    new Wander((0.5f * 5.55f) + 0.74f),
                    new Shoot(10, 8, 10, fixedAngle: 0, rotateAngle: 70, cooldownMS: 2000,
                        projectileIndex: 1),
                    new Shoot(10, 8, 10, fixedAngle: 180, rotateAngle: 70, cooldownMS: 2000,
                        projectileIndex: 1),
                    new TimedTransition(6200, "TransAttack3")
                ),
                new State("TransAttack3",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Wander((0.5f * 5.55f) + 0.74f),
                    new Flash(0x00FF0C, .25, 8),
                    new TimedTransition(2000, "Attack3")
                ),
                new State("Attack3",
                    new Wander((0.5f * 5.55f) + 0.74f),
                    new Shoot(20, 9, 360 / 9f, fixedAngle: 360 / 9f, projectileIndex: 2, cooldownMS: 2300),
                    new TimedTransition(6000, "TransAttack1"),
                    new State("Shoot1",
                        new Shoot(20, 2, 4, 2, cooldownMS: 700, targeted: true),
                        new TimedTransition(1000, TransitionType.Random,
                            "Shoot1",
                            "Shoot2"
                        )
                    ),
                    new State("Shoot2",
                        new Shoot(20, 8, 5, 2, cooldownMS: 1100, targeted: true),
                        new TimedTransition(1000, TransitionType.Random,
                            "Shoot1",
                            "Shoot2"
                        )
                    )
                ),
                new State("TransAttack1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Wander((0.5f * 5.55f) + 0.74f),
                    new Flash(0x00FF0C, .25, 8),
                    new TimedTransition(2000, "Attack1"),
                    new HpLessTransition(0.15f, "Order")
                ),
                new State("Order",
                    new Wander((0.5f * 5.55f) + 0.74f),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new Order(30, "Horrid Reaper", "Die"),
                    new TimedTransition(1900, "Attack1")
                )
            );

        [CharacterBehavior("Horrid Reaper")]
        public static State HorridReaper =>
            new(
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new State("Move",
                    new Wander(3, distanceFromSpawn: 10),
                    new EntityNotWithinTransition("Grand Sphinx", 50, targetStates: "Die"), //Just to be sure
                    new TimedTransition(2000, TransitionType.Random, "Attack")
                ),
                new State("Attack",
                    new Shoot(10, 6, 360 / 6f, fixedAngle: 360 / 6f, cooldownMS: 700),
                    new EntityWithinTransition("Follow", radius: 2),
                    new TimedTransition(5000, TransitionType.Random, "Move")
                ),
                new State("Follow",
                    new Follow((0.7f * 5.55f) + 0.74f, 10, 3),
                    new Shoot(7, 1, cooldownMS: 700, targeted: true),
                    new TimedTransition(5000, TransitionType.Random, "Move")
                ),
                new State("Die",
                    new Taunt("OOaoaoAaAoaAAOOAoaaoooaa!!!"),
                    new Suicide(1000)
                )
            );
    }
}