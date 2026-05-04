using System;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public struct PlayerSightComponent : IEntityComponent {
    public int Id { get; set; }

    public HashSet<IntPoint> VisibleTiles = [];
    public HashSet<IntPoint> DiscoveredTiles = [];
    public HashSet<int> VisibleEntities = [];
    public List<ObjectStatusData> Statuses = [];

    public PlayerSightComponent(ref Entity en) {
        
    }
    
    public void Dispose() {
        
    }
}