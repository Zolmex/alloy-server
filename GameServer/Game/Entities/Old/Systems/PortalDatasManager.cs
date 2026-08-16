using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Systems;

public class PortalDatasManager(World world, int capacity) : ManagerBase<PortalData>(world, capacity) {

    private long _nextTick; // Tick portals every second
    
    public override void Tick(ref RealmTime time) {
        if (time.TotalElapsedMs < _nextTick)
            return;
        
        foreach (ref var portal in this) {
            portal.Tick(ref time);
        }

        _nextTick = time.TotalElapsedMs + 1000;
    }
}