using System.Numerics;
using Common.Game;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class NotMovingTransitionInfo {
    public Vector2 Position;
    public int TimeLeft;
}

public class NotMovingTransition : BehaviorTransition {
    private readonly int _delay;

    public NotMovingTransition(string targetState, int delay = 250) {
        RegisterTargetStates(targetState);
        _delay = delay;
    }

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<NotMovingTransitionInfo>(this);
        state.Position = new Vector2(host.Stats.Pos.X, host.Stats.Pos.Y);
        state.TimeLeft = _delay;
    }

    public override string Tick(ref EntityView host, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<NotMovingTransitionInfo>(this);
        if (state.TimeLeft > 0) {
            state.TimeLeft -= time.ElapsedMsDelta;
            return null;
        }

        if (host.Stats.Pos.X == state.Position.X && host.Stats.Pos.Y == state.Position.Y)
            return GetTargetState();

        // Re-assign the position and reset the delay
        state.Position = new Vector2(host.Stats.Pos.X, host.Stats.Pos.Y);
        state.TimeLeft = _delay;
        return null;
    }
}