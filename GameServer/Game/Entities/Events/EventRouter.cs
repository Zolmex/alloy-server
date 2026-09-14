using Arch.Core;

namespace GameServer.Game.Entities.Events;

public partial class EventRouter { // Invoke, Add, Remove are source-generated methods
    public readonly EventBus<DamageReceivedEvent> OnDamageReceived = new();
    public readonly EventBus<DeathEvent> OnDeath = new();

    public readonly Entity Entity;

    public EventRouter(Entity entity) {
        Entity = entity;
    }
}