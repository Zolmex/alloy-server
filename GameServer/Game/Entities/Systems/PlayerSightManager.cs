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
    private PooledList<IntPoint> _forcedTileUpdates = [];

    public override void Tick(ref RealmTime time) {
        foreach (ref var sight in this) {
            ref var playerStats = ref _world.EntityStats.Get(sight.Id);
            if (playerStats.Id == 0)
                continue;
            
            var user = _world.PlayerToUser[playerStats.Id];
            ProcessUpdate(user, ref playerStats, ref sight, out var statusCount);
            ProcessNewtick(user, ref sight, statusCount);
        }
        _forcedTileUpdates.Clear();
    }

    public void TileUpdate(IntPoint pos) {
        _forcedTileUpdates.Add(pos);
    }

    private void ProcessUpdate(User user, ref EntityStats playerEntityStats, ref PlayerSight sight, out int statusCount) {
        var newTileCount = GetNewTiles(ref playerEntityStats, ref sight);
        var newEntityCount = GetNewEntities(ref playerEntityStats, ref sight, out statusCount);

        if (newTileCount == 0 && newEntityCount == 0)
            return;
        
        user.SendPacket(new Update(_newTiles.AsSpan(), newTileCount, _newEntities.AsSpan(), newEntityCount,
            Span<ObjectDropData>.Empty, 0));
    }
    
    private int GetNewTiles(ref EntityStats playerEntityStats, ref PlayerSight sight) {
        var newTileCount = 0;
        var visibileTileCount = 0;
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

                            sight.VisibleTiles[visibileTileCount] = tile.Pos;
                            if (_forcedTileUpdates.Contains(tile.Pos) || sight.DiscoveredTiles.Add(tile.Pos)) { // This is a newly discovered tile
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

    private int GetNewEntities(ref EntityStats playerEntityStats, ref PlayerSight sight, out int statusCount) {
        var newEntityCount = 0;
        statusCount = 0;
        foreach (ref var en in _world.Map.GetEntitiesWithin(playerEntityStats.Pos, SIGHT_RADIUS_SQR)) {
            ref var stats = ref _world.EntityStats.Get(en.Id);

            if (stats.StatUpdateCount != 0 || stats.PositionUpdate) { // Track status for newtick
                sight.Statuses[statusCount++] = new ObjectStatusData {
                    ObjectId = en.Id,
                    Pos = stats.Pos,
                    StatUpdates = stats.StatUpdates,
                    StatCount = stats.StatUpdateCount,
                    PrivacyMask = en.Id == playerEntityStats.Id ? stats.PrivateMask : stats.PublicMask
                };
                if (statusCount >= sight.Statuses.Length) {
                    var newArr = ArrayPool<ObjectStatusData>.Shared.Rent(statusCount * 2);
                    sight.Statuses.AsSpan().CopyTo(newArr);
                    ArrayPool<ObjectStatusData>.Shared.Return(sight.Statuses);
                    sight.Statuses = newArr;
                }
            }

            if (sight.VisibleEntities.Add(en.Id)) {
                _newEntities[newEntityCount++] = new ObjectData() {
                    ObjectType = en.ObjectType,
                    Status = new ObjectStatusData() {
                        ObjectId = en.Id,
                        Pos = stats.Pos,
                        Stats = stats.Stats,
                        StatCount = EntityStats.STAT_COUNT,
                        PrivacyMask = en.Id == playerEntityStats.Id ? stats.PrivateMask : stats.PublicMask,
                    }
                };
                if (newEntityCount >= _newEntities.Length) {
                    var newArr = ArrayPool<ObjectData>.Shared.Rent(newEntityCount * 2);
                    _newEntities.AsSpan().CopyTo(newArr);
                    ArrayPool<ObjectData>.Shared.Return(_newEntities);
                    _newEntities = newArr;
                }
            }
        }

        return newEntityCount;
    }

    private void ProcessNewtick(User user, ref PlayerSight sight, int statusCount) {
        user.SendPacket(new NewTick(sight.Statuses, statusCount));
    }
}