using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common;
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

public class PlayerSightManager(World world, int capacity) : ManagerBase<PlayerSight>(world, capacity) {

    public const int SIGHT_RADIUS = 20;
    public const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;
    
    private MapTileData[] _newTiles = ArrayPool<MapTileData>.Shared.Rent(50);
    private ObjectData[] _newEntities = ArrayPool<ObjectData>.Shared.Rent(50);
    
    public override void Tick(ref RealmTime time) {
        foreach (ref var sight in this) {
            ref var playerStats = ref _world.EntityStats.Get(sight.Id);
            if (playerStats.Id == 0)
                continue;
            
            var user = _world.PlayerToUser[playerStats.Id];
            ProcessUpdate(user, ref playerStats, ref sight);
            ProcessNewtick(user, playerStats.Id, ref sight);
        }
    }

    private void ProcessUpdate(User user, ref EntityStats playerEntityStats, ref PlayerSight sight) {
        var newTileCount = GetNewTiles(ref playerEntityStats, ref sight);
        var newEntityCount = GetNewEntities(ref playerEntityStats, ref sight);

        if (newTileCount == 0 && newEntityCount == 0)
            return;
        
        user.SendPacket(new Update(_newTiles.AsSpan(), newTileCount, _newEntities.AsSpan(), newEntityCount,
            Span<ObjectDropData>.Empty, 0));
    }
    
    private int GetNewTiles(ref EntityStats playerEntityStats, ref PlayerSight sight) {
        var newTileCount = 0;
        sight.VisibleTiles.Clear();
        switch (_world.Config.Blocksight) {
            case World.UNBLOCKED_SIGHT:
                var pX = (int)playerEntityStats.Pos.X;
                var pY = (int)playerEntityStats.Pos.Y;
                var width = _world.Map.Data.Width;
                var height = _world.Map.Data.Height;
                for (var y = pY - SIGHT_RADIUS; y <= pY + SIGHT_RADIUS; y++)
                    for (var x = pX - SIGHT_RADIUS; x <= pX + SIGHT_RADIUS; x++)
                        if (x >= 0 && x < width && y >= 0 && y < height &&
                            playerEntityStats.TileDistSqr(x, y) <= SIGHT_RADIUS_SQR) {
                            var tile = _world.Map[x, y];

                            sight.VisibleTiles.Add(tile.Pos);
                            if (sight.DiscoveredTiles.Add(tile.Pos)) { // This is a newly discovered tile
                                _newTiles[newTileCount++] = tile;
                                if (newTileCount >= _newTiles.Length) {
                                    var newArr = ArrayPool<MapTileData>.Shared.Rent(newTileCount * 2);
                                    _newTiles.AsSpan().CopyTo(newArr);
                                    ArrayPool<MapTileData>.Shared.Return(_newTiles);
                                    _newTiles = newArr;
                                }
                            }
                        }

                break;
        }

        return newTileCount;
    }

    private int GetNewEntities(ref EntityStats playerEntityStats, ref PlayerSight sight) {
        var newEntityCount = 0;
        foreach (ref var en in _world.Map.GetEntitiesWithin(playerEntityStats.Pos, SIGHT_RADIUS_SQR)) {
            ref var stats = ref _world.EntityStats.Get(en.Id);
            if (sight.VisibleEntities.Add(en.Id)) {
                _newEntities[newEntityCount++] = new ObjectData() {
                    ObjectType = en.ObjectType,
                    PrivateMask = en.Id == playerEntityStats.Id ? stats.PrivateMask : stats.PublicMask,
                    Status = new ObjectStatusData() {
                        ObjectId = en.Id,
                        Pos = stats.Pos,
                        Stats = stats.Stats
                    }
                };
                if (newEntityCount >= _newEntities.Length) {
                    var newArr = ArrayPool<ObjectData>.Shared.Rent(newEntityCount * 2);
                    _newEntities.AsSpan().CopyTo(newArr);
                    ArrayPool<ObjectData>.Shared.Return(_newEntities);
                    _newEntities = newArr;
                }
                
                sight.Statuses.Add(new ObjectStatusData() {
                    ObjectId = en.Id,
                    Pos = stats.Pos,
                    Stats = stats.Stats
                });
            }
        }

        return newEntityCount;
    }

    private void ProcessNewtick(User user, int playerId, ref PlayerSight sight) {
        user.SendPacket(new NewTick(CollectionsMarshal.AsSpan(sight.Statuses), _world.EntityStats, playerId));
    }
}