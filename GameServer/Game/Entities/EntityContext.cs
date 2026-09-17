using Arch.Core;
using Arch.Core.Extensions;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities;

public readonly ref struct EntityContext {
    public readonly World World;
    public readonly Entity Entity;
    public readonly ObjectDesc Desc;
    public readonly BehaviorController BehavController;
    public readonly ref Behavior Behavior;
    public readonly ref Position Position;
    public readonly ref Stats Stats;
    public readonly ref Combat Combat;
    public readonly ref Flags Flags;
    
    public EntityContext(World world, Entity entity) {
        World = world;
        Entity = entity;
        Desc = XmlLibrary.ObjectDescs[world.Ecs.Get<ObjectType>(entity)];
        BehavController = world.BehaviorSystem.Get(entity);
        Behavior = world.Ecs.Get<Behavior>(entity);
        Position = world.Ecs.Get<Position>(entity);
        Stats = world.Ecs.Get<Stats>(entity);
        Combat = world.Ecs.Get<Combat>(entity);
        Flags = world.Ecs.Get<Flags>(entity);
    }
    
    public float GetSpeed(float speed) { // TODO: Condition effect system
        var tile = World.Map[(int)Position.Pos.X, (int)Position.Pos.Y];
        if (Entity.Has<PlayerTag>()) {
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