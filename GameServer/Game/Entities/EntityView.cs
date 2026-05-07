using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities;

public readonly ref struct EntityView {
    public readonly World World;
    public readonly int Id;
    public readonly ref Entity Entity;
    
    public readonly ref EntityBehavior Behavior;
    public readonly ref EntityStats Stats;
    
    public readonly ref PlayerSight PlayerSight;
    public readonly ref PlayerChat PlayerChat;

    public EntityView(World world, ref Entity entity) {
        World = world;
        Id = entity.Id;
        Entity = ref entity;
        
        Behavior = ref world.EntityBehaviors.Get(entity.Id);
        Stats = ref world.EntityStats.Get(entity.Id);
        
        PlayerSight = ref world.PlayerSights.Get(entity.Id);
        PlayerChat = ref world.PlayerChat.Get(entity.Id);
    }
}