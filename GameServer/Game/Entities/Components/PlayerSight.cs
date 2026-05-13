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

    public BitArray2D DiscoveredTiles;
    public IntPoint[] VisibleTiles = ArrayPool<IntPoint>.Shared.Rent(2000);
    public HashSet<int> VisibleEntities = [];
    public ObjectStatusData[] Statuses = ArrayPool<ObjectStatusData>.Shared.Rent(50);

    public PlayerSight(World world, ref Entity en) {
        Id = en.Id;
        DiscoveredTiles = new BitArray2D(world.Map.Data.Width, world.Map.Data.Height);
    }
    
    public void Dispose() {
        DiscoveredTiles.Dispose();
        VisibleEntities.Clear();
        ArrayPool<IntPoint>.Shared.Return(VisibleTiles);
        ArrayPool<ObjectStatusData>.Shared.Return(Statuses);
    }
}