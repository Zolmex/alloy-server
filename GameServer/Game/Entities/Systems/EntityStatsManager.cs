using System;
using System.Collections.Generic;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Systems;

public class EntityStatsManager(World world, int capacity) : ManagerBase<EntityStats>(world, capacity) {
    
    public override void Remove(int id) {
        ref var elem = ref Set.Get(id);
        if (elem.Id != 0)
            elem.Dispose();
        
        base.Remove(id);
    }

    public override void Tick(ref RealmTime time) {
        foreach (ref var stats in this){
            stats.Tick();
        }
    }
}