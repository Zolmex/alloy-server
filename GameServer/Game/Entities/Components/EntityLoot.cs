using System;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityLoot : IIdentifiable {
    public int Id { get; set; }

    private readonly World _world;

    public EntityLoot(World world) {
        _world = world;
    }

    public void Dispose() {
        
    }
}