using System;
using Common.Resources.World;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public struct PlayerSightComponent : IEntityComponent {
    public int Id { get; set; }

    public HashSet<MapTileData> VisibleTiles = [];
    public HashSet<MapTileData> DiscoveredTiles = [];
    public HashSet<int> VisibleEntities = [];

    public PlayerSightComponent(ref Entity en) {
        
    }
    
    public void Dispose() {
        
    }
}