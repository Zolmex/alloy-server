using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Extensions;

public static class EntityExtensions {
    extension(Entity en) {
        public void Move(World world, float newX, float newY) {
            ref var stats = ref world.EntityStats.Get(en.Id);
            stats.Move(newX, newY);
        }
    }
}