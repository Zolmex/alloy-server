using System;
using System.Buffers;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public struct PlayerSight : IIdentifiable {
    public int Id { get; set; }

    public HashSet<IntPoint> VisibleTiles = [];
    public HashSet<IntPoint> DiscoveredTiles = [];
    public HashSet<int> VisibleEntities = [];
    public ObjectStatusData[] Statuses = ArrayPool<ObjectStatusData>.Shared.Rent(50);

    public PlayerSight(ref Entity en) {
        
    }
    
    public void Dispose() {
        VisibleTiles.Clear();
        DiscoveredTiles.Clear();
        VisibleEntities.Clear();
        ArrayPool<ObjectStatusData>.Shared.Return(Statuses);
    }
}