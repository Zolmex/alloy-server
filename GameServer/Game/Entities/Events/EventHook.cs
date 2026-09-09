
using Arch.Core;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Events;

public enum EventType {
    Death,
    DamageReceived
}

public delegate void EventAction(Entity entity);

public class EventHook {
    public readonly EventType Type;
    public readonly Entity Target;
    public readonly EventAction Action;
    public bool AutoRemove;

    public EventHook(EventType type, Entity target, EventAction action, bool once = false) {
        Type = type;
        Target = target;
        Action = action;
        AutoRemove = once;
    }

    public virtual bool TryTrigger(World world) {
        return AutoRemove;
    }
}