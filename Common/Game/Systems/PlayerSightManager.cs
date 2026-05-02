using System;
using System.Collections.Generic;
using Common.Game.Components;
using Common.Utilities.Collections;

namespace Common.Game.Systems;

public class PlayerSightManager(int capacity) : ManagerBase<PlayerSightComponent>(capacity) {
    
    public override void Tick(ref RealmTime time) {
        for (var i = 0; i < _set.Count; i++) {
            ref var sight = ref _set.GetAt(i);
            
        }
    }
}