using Arch.Core;
using Arch.System;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using ArchWorld = Arch.Core.World;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public partial class PlayerSightSystem : BaseSystem<World, RealmTime> {

    private readonly ArchWorld _archWorld;
    
    public PlayerSightSystem(World world, ArchWorld archWorld) : base(world) {
        _archWorld = archWorld;
    }
    
    [Query]
    public void Process([Data] ref RealmTime time, Entity entity, ref PlayerSight sight) {
        var user = World.Users[new EntityId(entity)];
        ProcessUpdate(user, ref playerStats, ref sight);
        ProcessNewtick(user, ref sight);
    }
}