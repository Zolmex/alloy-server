using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Extensions;

public static class EntityExtensions {
    extension(Entity en) {
        public void Move(World world, float newX, float newY) {
            ref var stats = ref world.EntityStats.Get(en.Id);
            stats.Move(newX, newY);
        }

        public void Init(World world, WorldPosData spawnPos) {
            ref var stats = ref world.EntityStats.Get(en.Id);
            stats.Move(spawnPos.X, spawnPos.Y);
            
            if (en.Desc.Static) {
                var tile = world.Map[(int)spawnPos.X, (int)spawnPos.Y];
                if (tile.ObjectId == EntityId.Null)
                    tile.ObjectId = en.Id;
            }
        }
    }
}