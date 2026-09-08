using Arch.Core;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Extensions;

public static class EntityExtensions {
    extension(Entity en) {
        public void Init(World world, WorldPosData spawnPos) {
            ref var pos = ref world.Ecs.Get<Position>(en);
            ref var objType = ref world.Ecs.Get<ObjectType>(en);
            pos.Move(spawnPos.X, spawnPos.Y);

            var desc = XmlLibrary.ObjectDescs[objType];
            if (desc.Static) {
                var tile = world.Map[(int)spawnPos.X, (int)spawnPos.Y];
                if (tile.ObjectId == EntityId.Null)
                    tile.ObjectId = new EntityId(en);
            }
        }
        
        public void Move(World world, float newX, float newY) {
            ref var pos = ref world.Ecs.Get<Position>(en);
            pos.Move(newX, newY);
        }
        
        public float GetSpeed(float speed, MapTileData tile) { // TODO: Condition effect system
            if (en.Has<PlayerType>()) {
                // if (p.HasConditionEffect(ConditionEffectIndex.Slowed))
                //     return 1;
                //
                // if (p.HasConditionEffect(ConditionEffectIndex.Speedy))
                //     speed *= 1.5f;

                var tileSpeedMult = tile.Desc.Speed; // Sink level is not supported so just use the tile speed
                return speed * tileSpeedMult;
            }

            // if (chr.HasConditionEffect(ConditionEffectIndex.Slowed))
            //     return 1;
            //
            // if (chr.HasConditionEffect(ConditionEffectIndex.Speedy))
            //     speed *= 1.5f;
            return speed;
        }
    }
}