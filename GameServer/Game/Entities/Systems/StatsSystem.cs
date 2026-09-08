using Arch.System;
using Common.Game;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Systems;

public partial class StatsSystem(World world) : BaseSystem<World, RealmTime>(world) {
    [Query(Parallel = true)]
    public void Tick([Data] ref RealmTime time, ref Stats stats) {
        stats.Update();
    }
}