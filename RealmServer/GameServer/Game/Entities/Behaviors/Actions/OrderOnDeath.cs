using GameServer.Game.Entities.Types;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record OrderOnDeath : BehaviorScript
{
    private readonly string _children;
    private readonly float _range;
    private readonly string _targetState;

    public OrderOnDeath(float range, string children, string targetState)
    {
        _range = range;
        _children = children;
        _targetState = targetState;
    }

    public override void Start(CharacterEntity host)
    {
        host.DeathEvent += OnDeath;
    }

    private void OnDeath(Entity en)
    {
        foreach (var i in (en as CharacterEntity).GetOtherEnemiesByName(_children, _range))
            i.ClassicBehavior?.TransitionTo(_targetState, RealmManager.WorldTime);
    }
}