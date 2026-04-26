using GameServer.Game.Entities.Types;

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

    public override void Start(CharacterEntity host) {
        foreach (var i in host.GetOtherEnemiesByName(_children, _range))
            i.ClassicBehavior?.TransitionTo(_targetState, RealmManager.WorldTime);
    }
}