using System;
using System.Collections.Generic;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Systems;

public class PlayerSightManager(int capacity) : ManagerBase<PlayerSightComponent>(capacity) {
    
    public override void Tick(ref RealmTime time) {
        for (var i = 0; i < _set.Count; i++) {
            ref var sight = ref _set.GetAt(i);
            
        }
    }
}