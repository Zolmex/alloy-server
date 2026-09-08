using Common.Utilities.Collections;
using GameServer.Game.Entities.Old;

namespace GameServer.Game.Entities.Components;

public struct PlayerSight {
    public float Radius;
}

public class PlayerSightState : IDisposable
{
    public HashSet<Entity> VisibleEntities = [];
    public BitArray2D VisibleTiles;
    public BitArray2D DiscoveredTiles;
    
    public PlayerSightState(int width, int height) {
        VisibleTiles = new BitArray2D(width, height);
        DiscoveredTiles = new BitArray2D(width, height);
    }

    public void Dispose() {
        VisibleTiles.Dispose();
        DiscoveredTiles.Dispose();
    }
}