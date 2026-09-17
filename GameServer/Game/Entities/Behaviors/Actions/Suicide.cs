using GameServer.Game.Entities.Old;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record Suicide : BehaviorScript {
    private readonly int _delay;

    public Suicide(int delay = 300) {
        _delay = delay;
    }

    public override void Start(ref EntityContext host) {
        var id = host.Entity;
        host.World.AddTimedAction(_delay, w => w.LeaveWorld(id));
    }
}