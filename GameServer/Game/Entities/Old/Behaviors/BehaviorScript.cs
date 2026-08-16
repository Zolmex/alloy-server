using Common.Game;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors;

public record BehaviorScript : IStateChild {
    
    public enum BehaviorTickState {
        OnCooldown, // The behavior was on cooldown during the tick.
        BehaviorFailed, // The behavior failed to be ran during the tick.
        BehaviorActive, // The behavior was sucesssfully ran this tick.
        BehaviorActivate, // The behavior was successfully ran for the first time this tick.
        BehaviorDeactivate // The behavior was successfully ran, and met the deactivation condition for the behavior this tick.
    }

    public enum TargetType {
        ClosestPlayer,
        FixedAngle,
        RandomPlayerPerBehavior,
        RandomPlayerPerCycle,
        FarthestPlayer,
        Player, // The behavior will be targetting a specific player.
        Entity
    }

    // Setup any ObjectDesc data here
    public virtual void Setup(ObjectDesc desc) { }

    public virtual void Start(ref EntityView host) { }

    public virtual BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        return BehaviorTickState.OnCooldown;
    }

    public virtual void End(ref EntityView host, ref RealmTime time) { }
}