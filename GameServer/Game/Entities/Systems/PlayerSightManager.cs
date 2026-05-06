using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Systems;

public class PlayerSightManager(World world, int capacity) : ManagerBase<PlayerSightComponent>(world, capacity) {

    public const int SIGHT_RADIUS = 20;
    public const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;
    
    public override void Tick(ref RealmTime time) {
        var newTiles = new List<MapTileData>(50);
        var newEntities = new List<ObjectData>(50);
        for (var i = 0; i < _set.Count; i++) {
            ref var sight = ref _set.GetAt(i);
            ref var player = ref _world.Entities.Get(sight.Id);
            if (player.Id == 0)
                continue;
            
            var user = _world.PlayerToUser[player.Id];
            ProcessUpdate(user, ref player, ref sight, newTiles, newEntities);
            ProcessNewtick(user, ref player, ref sight);
        }
    }

    private void ProcessUpdate(User user, ref Entity player, ref PlayerSightComponent sight, List<MapTileData> newTiles, List<ObjectData> newEntities) {
        GetNewTiles(ref player, ref sight, newTiles);
        GetNewEntities(ref player, ref sight, newEntities);

        if (newTiles.Count == 0 && newEntities.Count == 0)
            return;
        
        user.SendPacket(new Update(CollectionsMarshal.AsSpan(newTiles), CollectionsMarshal.AsSpan(newEntities),
            Span<ObjectDropData>.Empty));
        newTiles.Clear();
    }
    
    private void GetNewTiles(ref Entity player, ref PlayerSightComponent sight, List<MapTileData> newTiles) {
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

                            sight.VisibleTiles.Add(tile.Pos);
                            if (sight.DiscoveredTiles.Add(tile.Pos)) // This is a newly discovered tile
                                newTiles.Add(tile);
                        }

                break;
        }
    }

    private void GetNewEntities(ref Entity player, ref PlayerSightComponent sight, List<ObjectData> newEntities) {
        // TODO: Implement map chunk system for efficient spatial queries
        for (var i = 0; i < _world.Entities.Count; i++) {
            ref var en = ref _world.Entities.Get(i);
            if (en.DistSqr(player) >= SIGHT_RADIUS_SQR)
                continue;

            if (sight.VisibleEntities.Add(en.Id)) {
                ref var stats = ref _world.EntityStats.Get(en.Id);
                newEntities.Add(new ObjectData() {
                    ObjectType = en.ObjectType,
                    PrivateMask = en.Id == player.Id ? stats.PrivateMask : stats.PublicMask,
                    Status = new ObjectStatusData() {
                        ObjectId = en.Id,
                        Pos = en.Pos,
                        Stats = stats.Stats
                    }
                });
                
                sight.Statuses.Add(new ObjectStatusData() {
                    ObjectId = stats.Id,
                    Pos = en.Pos,
                    Stats = stats.Stats
                });
            }
        }
    }

    private void ProcessNewtick(User user, ref Entity player, ref PlayerSightComponent sight) {
        user.SendPacket(new NewTick(CollectionsMarshal.AsSpan(sight.Statuses), _world.EntityStats, player.Id));
    }
}