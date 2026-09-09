using Arch.Core;
using Common;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Events;

public class DeathEvent(Entity target, EventAction action, bool once = false) : EventHook(EventType.Death, target, action, once) {
    public override bool TryTrigger(World world) {
        ref var stats = ref world.Ecs.Get<Stats>(Target);
        if (stats.GetInt(StatType.HP) <= 0)
            return AutoRemove;
        return false;
    }
}