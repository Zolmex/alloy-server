using Common.Game;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class TimedInfo {
    public int PeriodLeft;
    public bool Start;
}

public record Timed : BehaviorScript {
    private readonly BehaviorScript[] _behaviors;
    private readonly int _period;

    public Timed(int period, params BehaviorScript[] behaviors) {
        _period = period;
        _behaviors = behaviors;
    }

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<TimedInfo>(this);
        state.PeriodLeft = _period;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<TimedInfo>(this);

        if (state.PeriodLeft > 0) {
            state.PeriodLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        if (state.PeriodLeft <= 0) {
            if (!state.Start) {
                state.Start = true;
                foreach (var behavior in _behaviors)
                    behavior.Start(ref host);
            }

            foreach (var behavior in _behaviors)
                behavior.Tick(ref host, ref time);
        }

        return BehaviorTickState.BehaviorActive;
    }
}