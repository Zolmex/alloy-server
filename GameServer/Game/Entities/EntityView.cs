using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities;

public readonly ref struct EntityView {
    public readonly World World;
    public readonly ref Entity Entity;
    public readonly ref StatsComponent Stats;
    public readonly ref PlayerSightComponent PlayerSight;

    public EntityView(World world, in Entity entity) {
        World = world;
        Entity = entity;
        Stats = world.EntityStats.Get(entity.Id);
        PlayerSight = world.PlayerSights.Get(entity.Id);
    }
}