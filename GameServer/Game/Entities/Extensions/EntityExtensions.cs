using System.Numerics;
using Arch.Core;
using Arch.Core.Extensions;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Utilities;
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
                if (tile.Object == Entity.Null)
                    tile.Object = en;
            }
        }
        
        public void Move(World world, float newX, float newY) {
            ref var pos = ref world.Ecs.Get<Position>(en);
            pos.Move(newX, newY);
        }
        
        public void MoveTowards(World world, ref RealmTime time, ref WorldPosData moveTo, float tilesPerSecond) {
            ref var pos = ref world.Ecs.Get<Position>(en);
            var angle = pos.GetAngleBetween(moveTo);
            var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            var speed = en.GetSpeed(tilesPerSecond, world.Map[(int)pos.Pos.X, (int)pos.Pos.Y]) * (time.ElapsedMsDelta / 1000f);
            dist *= speed;

            if (moveTo.DistSqr(pos.Pos) < dist.LengthSquared()) {
                // If the distance we're about to move is greater than the distance to the desired position, set position to the desired position
                en.Move(world, moveTo.X, moveTo.Y);
                return;
            }

            var newPos = pos.Pos + dist;
            en.Move(world, newPos.X, newPos.Y);
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