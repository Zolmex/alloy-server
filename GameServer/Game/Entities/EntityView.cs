using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities;

public readonly ref struct EntityView {
    public readonly World World;
    public readonly ref Entity Entity;
    public readonly ref EntityStats EntityStats;
    public readonly ref PlayerSight PlayerSight;
    public readonly ref PlayerChat PlayerChat;

    public EntityView(World world, ref Entity entity) {
        World = world;
        Entity = ref entity;
        EntityStats = ref world.EntityStats.Get(entity.Id);
        PlayerSight = ref world.PlayerSights.Get(entity.Id);
        PlayerChat = ref world.PlayerChat.Get(entity.Id);
    }
}