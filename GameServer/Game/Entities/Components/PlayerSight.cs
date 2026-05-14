using System;
using System.Buffers;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct PlayerSight : IIdentifiable, IDisposable {
    public int Id { get; set; }

    public HashSet<int> VisibleEntities = [];
    public PooledList<ObjectStatusData> Statuses = new(50);
    public BitArray2D VisibleTiles;
    public BitArray2D DiscoveredTiles;

    public PlayerSight(World world, ref Entity en) {
        Id = en.Id;
        DiscoveredTiles = new BitArray2D(world.Map.Data.Width, world.Map.Data.Height);
        VisibleTiles = new BitArray2D(world.Map.Data.Width, world.Map.Data.Height);
    }
    
    public void Dispose() {
        VisibleEntities.Clear();
        Statuses.Dispose();
        VisibleTiles.Dispose();
        DiscoveredTiles.Dispose();
    }
}