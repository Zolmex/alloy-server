using GameServer.Game.Entities.Old;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record Order : BehaviorScript {
    private readonly string _children;
    private readonly float _range;
    private readonly string _targetState;

    public Order(float range, string children, string targetState) {
        _range = range;
        _children = children;
        _targetState = targetState;
    }

    public override void Start(ref EntityContext host) {
        foreach (var entity in host.World.Map.GetEntitiesByName(host.Position.Pos, _children, _range)) {
            var behaviorController = host.World.BehaviorSystem.Get(entity);
            behaviorController.TransitionTo(_targetState, ref GameLogic.WorldTime);
        }
    }
}