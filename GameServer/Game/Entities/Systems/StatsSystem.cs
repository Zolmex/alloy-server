using Arch.Core;
using Arch.System;
using Common.Game;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Systems;

public partial class StatsSystem : BaseSystem<World, RealmTime> {
    public StatsSystem(World world) : base(world) { }

    [Query(Parallel = true)]
    public void UpdateStats([Data] ref RealmTime time, ref Stats stats) {
        stats.Update();
    }
}