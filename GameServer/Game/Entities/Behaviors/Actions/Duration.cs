using Common.Game;
using GameServer.Game.Entities.Old;

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

    public override void Start(ref EntityContext host) {
        var state = host.BehavController.Resources.ResolveResource<DurationInfo>(this);
        state.TimeLeft = _duration;
        _behavior.Start(ref host);
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var state = host.BehavController.Resources.ResolveResource<DurationInfo>(this);

        if (state.TimeLeft <= 0) return BehaviorTickState.BehaviorFailed;

        _behavior.Tick(ref host, ref time);
        state.TimeLeft -= time.ElapsedMsDelta;
        return BehaviorTickState.BehaviorActive;
    }
}