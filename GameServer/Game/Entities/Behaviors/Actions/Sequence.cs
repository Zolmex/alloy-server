using Common.Game;
using GameServer.Game.Entities.Old;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class SequenceInfo {
    public int Index;
}

public record Sequence : BehaviorScript {
    private readonly BehaviorScript[] _behaviors;

    public Sequence(params BehaviorScript[] behaviors) {
        _behaviors = behaviors;
    }

    public override void Start(BehaviorController controller) {
        var state = host.Behavior.Resources.ResolveResource<SequenceInfo>(this);
        state.Index = 0;

        foreach (var behav in _behaviors)
            behav.Start(ref host);
    }

    public override BehaviorTickState Tick(BehaviorController controller, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<SequenceInfo>(this);
        var status = _behaviors[state.Index].Tick(ref host, ref time);
        if (status == BehaviorTickState.BehaviorActive ||
            status == BehaviorTickState.BehaviorFailed) {
            state.Index++;
            if (state.Index == _behaviors.Length)
                state.Index = 0;
        }

        return BehaviorTickState.BehaviorActive;
    }
}