using System.Numerics;
using Common.Game;
using GameServer.Game.Entities.Old;

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

    public override void Start(ref EntityContext host) {
        var state = host.BehavController.Resources.ResolveResource<NotMovingTransitionInfo>(this);
        state.Position = new Vector2(host.Position.Pos.X, host.Position.Pos.Y);
        state.TimeLeft = _delay;
    }

    public override string Tick(ref EntityContext host, ref RealmTime time) {
        var state = host.BehavController.Resources.ResolveResource<NotMovingTransitionInfo>(this);
        if (state.TimeLeft > 0) {
            state.TimeLeft -= time.ElapsedMsDelta;
            return null;
        }

        if (host.Position.Pos.X == state.Position.X && host.Position.Pos.Y == state.Position.Y)
            return GetTargetState();

        // Re-assign the position and reset the delay
        state.Position = new Vector2(host.Position.Pos.X, host.Position.Pos.Y);
        state.TimeLeft = _delay;
        return null;
    }
}