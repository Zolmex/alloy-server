using Common.Game;
using GameServer.Game.Entities;
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

    public override void Start(ref EntityView host) {
        var dmgTakenInfo = host.Behavior.Resources.ResolveResource<DamageTakenRecord>(this);
        host.Events.OnDamageReceived.Subscribe(dmgTakenInfo.OnEntityDamaged);
    }

    public override string Tick(ref EntityView host, ref RealmTime time) {
        var dmgTakenInfo = host.Behavior.Resources.ResolveResource<DamageTakenRecord>(this);
        if (dmgTakenInfo.DamageTaken >= _damage)
            return GetTargetState();

        return null;
    }

    public override void End(ref EntityView host, ref RealmTime time) {
        var dmgTakenInfo = host.Behavior.Resources.ResolveResource<DamageTakenRecord>(this);
        host.Events.OnDamageReceived.Unsubscribe(dmgTakenInfo.OnEntityDamaged);
    }
}