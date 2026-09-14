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

    public override void Start(BehaviorController controller) {
        var dmgTakenInfo = controller.Resources.ResolveResource<DamageTakenRecord>(this);
        controller.World.EventSystem.Subscribe(controller.Host, dmgTakenInfo.OnEntityDamaged);
    }

    public override string Tick(BehaviorController controller, ref RealmTime time) {
        var dmgTakenInfo = controller.Resources.ResolveResource<DamageTakenRecord>(this);
        if (dmgTakenInfo.DamageTaken >= _damage)
            return GetTargetState();

        return null;
    }

    public override void End(BehaviorController controller, ref RealmTime time) {
        var dmgTakenInfo = controller.Resources.ResolveResource<DamageTakenRecord>(this);
        controller.World.EventSystem.Unsubscribe(controller.Host, dmgTakenInfo.OnEntityDamaged);
    }
}