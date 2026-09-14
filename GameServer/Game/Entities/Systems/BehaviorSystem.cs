using Arch.Core;
using Arch.System;
using Collections.Pooled;
using Common.Game;
using Common.Resources.Xml;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public partial class BehaviorSystem(World world) : BaseSystem<World, RealmTime>(world) {

    private readonly PooledDictionary<Entity, BehaviorController> _behaviorControllers = [];
    
    [Query]
    public void Tick([Data] ref RealmTime realmTime, Entity entity, ref Behavior behavior, ref ObjectType objType) {
        var desc = XmlLibrary.ObjectDescs[objType];
        if (!_behaviorControllers.TryGetValue(entity, out var behaviorController))
            behaviorController = _behaviorControllers[entity] = new BehaviorController(World, entity, desc.ObjectId);
        
        behaviorController.Tick(ref realmTime);
    }
}