using Arch.Core;
using Common.Game;
using GameServer.Game.Entities.Events;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class OnParentDeathInfo {
    public bool ParentDead;
}

public class OnParentDeathTransition : BehaviorTransition {
    public OnParentDeathTransition(string targetState) {
        RegisterTargetStates(targetState);
    }

    public override void Start(ref EntityContext host) {
        var state = host.BehavController.Resources.ResolveResource<OnParentDeathInfo>(this);
        state.ParentDead = host.Behavior.Parent == Entity.Null;
        if (!state.ParentDead)
            host.World.EventSystem.Subscribe(host.Entity, (ref DeathEvent _) => state.ParentDead = true);
    }

    public override string Tick(ref EntityContext host, ref RealmTime time) {
        var state = host.BehavController.Resources.ResolveResource<OnParentDeathInfo>(this);
        return state.ParentDead ? GetTargetState() : null;
    }
}