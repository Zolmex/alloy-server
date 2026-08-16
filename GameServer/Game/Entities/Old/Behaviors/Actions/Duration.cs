using Common.Game;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class DurationInfo {
    public int TimeLeft;
}

public record Duration : BehaviorScript {
    private readonly BehaviorScript _behavior;
    private readonly int _duration;

    public Duration(BehaviorScript behavior, int duration) {
        _behavior = behavior;
        _duration = duration;
    }

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<DurationInfo>(this);
        state.TimeLeft = _duration;
        _behavior.Start(ref host);
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<DurationInfo>(this);

        if (state.TimeLeft <= 0) return BehaviorTickState.BehaviorFailed;

        _behavior.Tick(ref host, ref time);
        state.TimeLeft -= time.ElapsedMsDelta;
        return BehaviorTickState.BehaviorActive;
    }
}