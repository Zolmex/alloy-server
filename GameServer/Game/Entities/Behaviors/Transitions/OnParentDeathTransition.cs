using Common.Game;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class OnParentDeathInfo {
    public bool ParentDead;
}

public class OnParentDeathTransition : BehaviorTransition {
    public OnParentDeathTransition(string targetState) {
        RegisterTargetStates(targetState);
    }

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<OnParentDeathInfo>(this);
        ref var parentStats = ref host.World.EntityStats.Get(host.Behavior.ParentId);
        ref var parentEvents = ref host.World.EntityEvents.Get(host.Behavior.ParentId);
        state.ParentDead = parentStats.Id == EntityId.Null;
        if (!state.ParentDead)
            parentEvents.OnDeath.Subscribe((ref evt) => state.ParentDead = true);
    }

    public override string Tick(ref EntityView host, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<OnParentDeathInfo>(this);
        return state.ParentDead ? GetTargetState() : null;
    }
}