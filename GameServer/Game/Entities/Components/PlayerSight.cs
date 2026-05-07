using System;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public struct PlayerSight : IEntityComponent {
    public int Id { get; set; }

    public HashSet<IntPoint> VisibleTiles = [];
    public HashSet<IntPoint> DiscoveredTiles = [];
    public HashSet<int> VisibleEntities = [];
    public List<ObjectStatusData> Statuses = [];

    public PlayerSight(ref Entity en) {
        
    }
    
    public void Dispose() {
        VisibleTiles.Clear();
        DiscoveredTiles.Clear();
        VisibleEntities.Clear();
        Statuses.Clear();
    }
}