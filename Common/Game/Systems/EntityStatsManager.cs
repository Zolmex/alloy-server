using System;
using System.Collections.Generic;
using Common.Game.Components;
using Common.Utilities.Collections;

namespace Common.Game.Systems;

public class EntityStatsManager(int capacity) : ManagerBase<StatsComponent>(capacity) {
    
    public void Tick(ref RealmTime time) {
        for (var i = 0; i < _set.Count; i++) {
            ref var stats = ref _set.GetAt(i);

            var maxHp = stats.GetInt(StatType.MaxHP); // DEBUG
            stats.Set(StatType.MaxHP, maxHp + 1);
            
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
            
            if (stats.Id == 1) // DEBUG
                Console.WriteLine($"[{stats.Id}]: Public: {publicCount} | Private: {privateCount}");
        }
    }
}