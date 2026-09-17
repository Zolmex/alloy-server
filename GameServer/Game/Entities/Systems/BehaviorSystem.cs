using Arch.Core;
using Arch.System;
using Collections.Pooled;
using Common.Game;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public partial class BehaviorSystem(World world) : BaseSystem<World, RealmTime>(world) {

    public IEnumerable<BehaviorController> Controllers => _behaviorControllers.Values;
    
    private readonly PooledDictionary<Entity, BehaviorController> _behaviorControllers = [];
    
    [Query]
    public void Tick([Data] ref RealmTime realmTime, Entity entity, ref Behavior behavior) {
        if (!_behaviorControllers.TryGetValue(entity, out var behaviorController))
            return;
        
        behaviorController.Tick(ref realmTime);
    }

    public void Add(Entity entity, ObjectDesc desc) {
        var controller = _behaviorControllers[entity] = new BehaviorController(World, entity, desc.ObjectId);
        controller.Load();
    }
    
    public void Remove(Entity entity) {
        _behaviorControllers.Remove(entity);
    }

    public BehaviorController Get(Entity entity) {
        if (!_behaviorControllers.TryGetValue(entity, out var behaviorController))
            return null;
        return behaviorController;
    }
}