using System;
using System.Collections.Generic;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Systems;

public class PlayerSightManager(World world, int capacity) : ManagerBase<PlayerSightComponent>(world, capacity) {

    private const int SIGHT_RADIUS = 20;
    private const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;
    
    public override void Tick(ref RealmTime time) {
        for (var i = 0; i < _set.Count; i++) {
            ref var sight = ref _set.GetAt(i);
            var newTiles = GetNewTiles(ref sight);
            var newEntities = GetNewEntities(ref sight);
        }
    }
    
    private List<MapTileData> GetNewTiles(ref PlayerSightComponent sight) {
        ref var plr = ref _world.Entities.Get(sight.Id);
        var ret = new List<MapTileData>();
        
        sight.VisibleTiles.Clear();
        switch (_world.Config.Blocksight) {
            case World.UNBLOCKED_SIGHT:
                var pX = (int)plr.Pos.X;
                var pY = (int)plr.Pos.Y;
                var width = _world.Map.Width;
                var height = _world.Map.Height;
                for (var y = pY - SIGHT_RADIUS; y <= pY + SIGHT_RADIUS; y++)
                    for (var x = pX - SIGHT_RADIUS; x <= pX + SIGHT_RADIUS; x++)
                        if (x >= 0 && x < width && y >= 0 && y < height &&
                            plr.TileDistSqr(x, y) <= SIGHT_RADIUS_SQR) {
                            var tile = _world.Map.Tiles[x, y];

                            sight.VisibleTiles.Add(tile);
                            if (sight.DiscoveredTiles.Add(tile)) // This is a newly discovered tile
                                ret.Add(tile);
                        }

                break;
        }

        return ret;
    }

    private List<ObjectData> GetNewEntities(ref PlayerSightComponent sight) {
        var ret = new List<ObjectData>();
        
        
        
        return ret;
    }
}