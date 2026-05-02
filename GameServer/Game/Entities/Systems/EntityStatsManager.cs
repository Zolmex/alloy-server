using System;
using System.Collections.Generic;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Systems;

public class EntityStatsManager(int capacity) : ManagerBase<StatsComponent>(capacity) {
    
    public override void Remove(int id) {
        ref var elem = ref _set.Get(id);
        elem.Dispose();
        
        base.Remove(id);
    }

    public override void Tick(ref RealmTime time) {
        for (var i = 0; i < _set.Count; i++) {
            ref var stats = ref _set.GetAt(i);
            var publicCount = 0;
            var privateCount = 0;
            for (var j = 0; j < StatsComponent.STAT_COUNT; j++) {
                if (stats.PublicMask.IsSet(j)) {
                    publicCount++;
                }
                if (stats.PrivateMask.IsSet(j)) {
                    privateCount++;
                }
            }
            stats.ClearMasks();
        }
    }
}