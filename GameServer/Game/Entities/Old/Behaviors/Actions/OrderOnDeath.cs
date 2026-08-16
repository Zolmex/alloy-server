using GameServer.Game.Entities.Events;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record OrderOnDeath : BehaviorScript {
    private readonly string _children;
    private readonly float _range;
    private readonly string _targetState;

    public OrderOnDeath(float range, string children, string targetState) {
        _range = range;
        _children = children;
        _targetState = targetState;
    }

    public override void Start(ref EntityView host) {
        host.Events.OnDeath.Subscribe(OnDeath);
    }

    private void OnDeath(ref DeathEvent evt) {
        ref var stats = ref evt.World.EntityStats.Get(evt.HostId);
        foreach (var id in evt.World.Map.GetEntitiesByName(stats.Pos, _children, _range)) {
            ref var behavior = ref evt.World.EntityBehaviors.Get(id);
            behavior.TransitionTo(_targetState, ref GameLogic.WorldTime);
        }
    }
}