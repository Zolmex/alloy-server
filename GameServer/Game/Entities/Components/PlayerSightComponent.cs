using System;
using Common.Resources.World;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public struct PlayerSightComponent : IIdentifiable, IDisposable {
    public int Id { get; set; }

    public HashSet<MapTileData> VisibleTiles = [];
    public HashSet<MapTileData> DiscoveredTiles = [];
    public HashSet<int> VisibleEntities = [];

    public PlayerSightComponent() {
        
    }
    
    public void Dispose() {
        
    }
}