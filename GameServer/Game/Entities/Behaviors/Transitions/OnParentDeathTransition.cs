using GameServer.Game.Entities.Types;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class OnParentDeathInfo {
    public bool ParentDead;
}

public class OnParentDeathTransition : BehaviorTransition {
    public OnParentDeathTransition(string targetState) {
        RegisterTargetStates(targetState);
    }

    public override void Start(CharacterEntity host) {
        var state = host.ResolveResource<OnParentDeathInfo>(this);
        state.ParentDead = host.Parent.Dead;
        host.Parent.DeathEvent += parent => state.ParentDead = parent.Dead;
    }

    public override string Tick(CharacterEntity host, RealmTime time) {
        var state = host.ResolveResource<OnParentDeathInfo>(this);
        return state.ParentDead ? GetTargetState() : null;
    }
}