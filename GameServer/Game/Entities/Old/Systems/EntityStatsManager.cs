using System;
using System.Collections.Generic;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Old.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Old.Systems;

public class EntityStatsManager(World world, int capacity) : ManagerBase<EntityStats>(world, capacity) {
    
    public override void Tick(ref RealmTime time) {
        foreach (ref var stats in this){
            stats.Tick();
        }
    }
}