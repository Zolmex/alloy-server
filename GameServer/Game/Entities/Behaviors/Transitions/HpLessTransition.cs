using Common;
using Common.Game;
using GameServer.Game.Entities.Old;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class HpLessTransition : BehaviorTransition {
    private readonly float _threshold;

    public HpLessTransition(float threshold, string targetState, TransitionType transitionType = TransitionType.Random)
        : base(transitionType) {
        RegisterTargetStates(targetState);
        _threshold = threshold;
    }

    public override string Tick(BehaviorController controller, ref RealmTime time) {
        var transition = (float)host.Stats.GetInt(StatType.HP) / host.Stats.GetInt(StatType.MaxHP) < _threshold;
        return transition ? GetTargetState() : null;
    }
}