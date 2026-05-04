using System;
using System.Collections.Generic;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Systems;

public class PlayerSightManager(World world, int capacity) : ManagerBase<PlayerSightComponent>(world, capacity) {

    private const int SIGHT_RADIUS = 20;
    private const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;
    
    public override void Tick(ref RealmTime time) {
        for (var i = 0; i < _set.Count; i++) {
            ref var sight = ref _set.GetAt(i);
            ref var player = ref _world.Entities.Get(sight.Id);
            if (player.Id == 0)
                continue;
            
            var user = _world.PlayerToUser[player.Id];
            
            var newTiles = GetNewTiles(ref player, ref sight);
            var newEntities = GetNewEntities(ref player, ref sight);
            user.SendPacket(new Update(newTiles, newEntities, new List<ObjectDropData>()));
            Console.WriteLine($"Player: {player.Pos} | Sending {newTiles.Count} tiles, {newEntities.Count} entities");
        }
    }
    
    private List<MapTileData> GetNewTiles(ref Entity player, ref PlayerSightComponent sight) {
        var ret = new List<MapTileData>();
        
        sight.VisibleTiles.Clear();
        switch (_world.Config.Blocksight) {
            case World.UNBLOCKED_SIGHT:
                var pX = (int)player.Pos.X;
                var pY = (int)player.Pos.Y;
                var width = _world.Map.Width;
                var height = _world.Map.Height;
                for (var y = pY - SIGHT_RADIUS; y <= pY + SIGHT_RADIUS; y++)
                    for (var x = pX - SIGHT_RADIUS; x <= pX + SIGHT_RADIUS; x++)
                        if (x >= 0 && x < width && y >= 0 && y < height &&
                            player.TileDistSqr(x, y) <= SIGHT_RADIUS_SQR) {
                            var tile = _world.Map.Tiles[x, y];

                            sight.VisibleTiles.Add(tile);
                            if (sight.DiscoveredTiles.Add(tile)) // This is a newly discovered tile
                                ret.Add(tile);
                        }

                break;
        }

        return ret;
    }

    private List<ObjectData> GetNewEntities(ref Entity player, ref PlayerSightComponent sight) {
        var ret = new List<ObjectData>();
        
        // TODO: Implement map chunk system for efficient spatial queries
        for (var i = 0; i < _world.Entities.Count; i++) {
            ref var en = ref _world.Entities.Get(i);
            if (en.DistSqr(player) >= SIGHT_RADIUS_SQR)
                continue;

            if (sight.VisibleEntities.Add(en.Id)) {
                ref var stats = ref _world.EntityStats.Get(en.Id);
                ret.Add(new ObjectData() {
                    ObjectType = en.ObjectType,
                    Status = new ObjectStatusData() {
                        ObjectId = en.Id,
                        Pos = en.Pos,
                        Stats = stats.Stats,
                        Force = true // Force all stats to send
                    }
                });
            }
        }
        
        return ret;
    }
}