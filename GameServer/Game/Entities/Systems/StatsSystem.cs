using Arch.System;
using Common.Game;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Systems;

public partial class StatsSystem(World world) : BaseSystem<World, RealmTime>(world) {

    public void Tick(ref RealmTime time) {
        ProcessQuery(World.Ecs, ref time);
    }
    
    [Query(Parallel = true)]
    public void Process([Data] ref RealmTime time, ref Stats stats) {
        stats.Update();
    }
}