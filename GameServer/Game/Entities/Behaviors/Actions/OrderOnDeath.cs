
using GameServer.Game.Entities.Components;
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

    public override void Start(ref EntityContext host) {
        host.World.EventSystem.Subscribe(host.Entity, OnDeath);
    }

    private void OnDeath(ref DeathEvent evt) {
        ref var pos = ref evt.World.Ecs.Get<Position>(evt.Entity);
        foreach (var entity in evt.World.Map.GetEntitiesByName(pos.Pos, _children, _range)) {
            var behavior = evt.World.BehaviorSystem.Get(entity);
            behavior.TransitionTo(_targetState, ref GameLogic.WorldTime);
        }
    }
}