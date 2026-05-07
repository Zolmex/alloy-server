using Common.Game;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class TimedTransitionInfo {
    public int TimeLeft;
}

public class TimedTransition : BehaviorTransition {
    private readonly int _timeDefault;

    public TimedTransition(int time, string targetState) {
        RegisterTargetStates(targetState);
        _timeDefault = time;
    }

    public TimedTransition(int time, TransitionType transitionType = TransitionType.Random,
        params string[] targetStates)
        : base(transitionType) {
        RegisterTargetStates(targetStates);
        _timeDefault = time;
    }

    public override void Start(ref EntityView host) {
        base.Start(ref host);
        var state = host.Behavior.Resources.ResolveResource<TimedTransitionInfo>(this);
        state.TimeLeft = _timeDefault;
    }

    public override string Tick(ref EntityView host, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<TimedTransitionInfo>(this);
        state.TimeLeft -= time.ElapsedMsDelta;

        if (state.TimeLeft <= 0) {
            state.TimeLeft = _timeDefault;
            return GetTargetState();
        }

        return null;
    }
}