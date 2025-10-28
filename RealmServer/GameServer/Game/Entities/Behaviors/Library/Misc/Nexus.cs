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
        [CharacterBehavior("DpsDummy0def")]
        public static State DpsDummy0def =>
            new State(
                new State("start",
                    new HpLessTransition(0.99999999999F, "TEST")
                ),
                new State("TEST",
                    new Taunt("TEST STARTED!", coolDownMS: 99999999),
                    new TimedTransition(10000, "HEAL")
                ),
                new State("HEAL",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new HealSelf(coolDown: 1000000, amount: 10000000),
                    new TimedTransition(2000, "start")
                )
            );
        [CharacterBehavior("DpsDummy40def")]
        public static State DpsDummy40def =>
            new State(
                new State("start",
                    new HpLessTransition(0.99999999999F, "TEST")
                ),
                new State("TEST",
                    new Taunt("TEST STARTED!", coolDownMS: 99999999),
                    new TimedTransition(10000, "HEAL")
                ),
                new State("HEAL",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new HealSelf(coolDown: 1000000, amount: 10000000),
                    new TimedTransition(2000, "start")
                )
            );
        [CharacterBehavior("DpsDummy100def")]
        public static State DpsDummy100def =>
            new State(
                new State("start",
                    new HpLessTransition(0.99999999999F, "TEST")
                ),
                new State("TEST",
                    new Taunt("TEST STARTED!", coolDownMS: 99999999),
                    new TimedTransition(10000, "HEAL")
                ),
                new State("HEAL",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new HealSelf(coolDown: 1000000, amount: 10000000),
                    new TimedTransition(2000, "start")
                )
            );
    }
}