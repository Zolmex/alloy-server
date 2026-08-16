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

    public override void Start(ref EntityView host) {
        foreach (var id in host.World.Map.GetEntitiesByName(host.Stats.Pos, _children, _range)) {
            ref var behavior = ref host.World.EntityBehaviors.Get(id);
            behavior.TransitionTo(_targetState, ref GameLogic.WorldTime);
        }
    }
}