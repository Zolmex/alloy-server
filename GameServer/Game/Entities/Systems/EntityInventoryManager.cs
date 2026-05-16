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

public class EntityInventoryManager(World world, int capacity) : ManagerBase<EntityInventory>(world, capacity) {

    public override void Tick(ref RealmTime time) {
        foreach (ref var inv in this) {
            inv.Tick(ref time);
        }
    }
}