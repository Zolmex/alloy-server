using Arch.Core;
using Collections.Pooled;
using Common.Structs;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Components;

public struct PlayerSight {
    public float Radius;
}

public class PlayerSightState : IDisposable {
    public Entity Owner;
    public HashSet<Entity> VisibleEntities = [];
    public PooledList<ObjectStatusData> Statuses = new(50);
    public BitArray2D VisibleTiles;
    public BitArray2D DiscoveredTiles;
    
    public PlayerSightState(Entity owner, int width, int height) {
        Owner = owner;
        VisibleTiles = new BitArray2D(width, height);
        DiscoveredTiles = new BitArray2D(width, height);
    }

    public void Dispose() {
        VisibleTiles.Dispose();
        DiscoveredTiles.Dispose();
    }
}