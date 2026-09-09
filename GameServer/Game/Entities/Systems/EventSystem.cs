using Arch.Core;
using Arch.System;
using Common.Game;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public class EventSystem(World world) : BaseSystem<World, RealmTime>(world) {

    private readonly Dictionary<Entity, List<EventHook>> _hooks = [];
    
    public void Tick(ref RealmTime time) {
        Parallel.ForEach(_hooks, pair => {
            var hooks = pair.Value;
            hooks.RemoveAll(hook => hook.TryTrigger(World));
        });
    }

    public void Subscribe(Entity en, EventHook hook) {
        if (!_hooks.TryGetValue(en, out var list))
            list = _hooks[en] = [];
        list.Add(hook);
    }

    public void Unsubscribe(Entity en, EventHook hook) {
        if (!_hooks.TryGetValue(en, out var list))
            return;
        list.Remove(hook);
    }
}