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
    public readonly ref EntityEvents Events;
    public readonly ref EntityCombat Combat;
    
    public readonly ref PlayerSight PlayerSight;
    public readonly ref PlayerChat PlayerChat;

    public EntityView(World world, int id) {
        World = world;
        Id = id;
        Entity = ref world.Entities.Get(id);
        
        Behavior = ref world.EntityBehaviors.Get(id);
        Stats = ref world.EntityStats.Get(id);
        Events = ref world.EntityEvents.Get(id);
        Combat = ref world.EntityCombat.Get(id);
        
        PlayerSight = ref world.PlayerSights.Get(id);
        PlayerChat = ref world.PlayerChat.Get(id);
    }
}