using Arch.Bus;
using Common.Game;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class DamageTakenRecord {
    public int DamageTaken;

    public void OnEntityDamaged(ref DamageReceivedEvent evt) {
        DamageTaken += evt.Damage;
    }
}

public class DamageTakenTransition : BehaviorTransition {
    private readonly int _damage;

    public DamageTakenTransition(int damage, string targetState) {
        RegisterTargetStates(targetState);
        _damage = damage;
    }

    public override void Start(ref EntityContext host) {
        var dmgTakenInfo = host.BehavController.Resources.ResolveResource<DamageTakenRecord>(this);
        host.World.EventSystem.Subscribe(host.Entity, dmgTakenInfo.OnEntityDamaged);
    }

    public override string Tick(ref EntityContext host, ref RealmTime time) {
        var dmgTakenInfo = host.BehavController.Resources.ResolveResource<DamageTakenRecord>(this);
        if (dmgTakenInfo.DamageTaken >= _damage)
            return GetTargetState();

        return null;
    }

    public override void End(ref EntityContext host, ref RealmTime time) {
        var dmgTakenInfo = host.BehavController.Resources.ResolveResource<DamageTakenRecord>(this);
        host.World.EventSystem.Unsubscribe(host.Entity, dmgTakenInfo.OnEntityDamaged);
    }
}