#region

using Common;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library;

public partial class BehaviorLib
{
    [CharacterBehavior("DpsDummy0def")]
    public static State DpsDummy0def =>
        new(
            new State("start",
                new HpLessTransition(0.99999999999F, "TEST")
            ),
            new State("TEST",
                new Taunt("TEST STARTED!", 99999999),
                new TimedTransition(10000, "HEAL")
            ),
            new State("HEAL",
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                new HealSelf(1000000, 10000000),
                new TimedTransition(2000, "start")
            )
        );

    [CharacterBehavior("DpsDummy40def")]
    public static State DpsDummy40def =>
        new(
            new State("start",
                new HpLessTransition(0.99999999999F, "TEST")
            ),
            new State("TEST",
                new Taunt("TEST STARTED!", 99999999),
                new TimedTransition(10000, "HEAL")
            ),
            new State("HEAL",
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                new HealSelf(1000000, 10000000),
                new TimedTransition(2000, "start")
            )
        );

    [CharacterBehavior("DpsDummy100def")]
    public static State DpsDummy100def =>
        new(
            new State("start",
                new HpLessTransition(0.99999999999F, "TEST")
            ),
            new State("TEST",
                new Taunt("TEST STARTED!", 99999999),
                new TimedTransition(10000, "HEAL")
            ),
            new State("HEAL",
                new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                new HealSelf(1000000, 10000000),
                new TimedTransition(2000, "start")
            )
        );
}