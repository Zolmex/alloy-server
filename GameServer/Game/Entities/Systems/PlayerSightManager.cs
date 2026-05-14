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

    private PooledList<MapTileData> _newTiles = new(50);
    private PooledList<ObjectData> _newEntities = new(50);
    private PooledList<ObjectDropData> _dropEntities = new(50);
    private PooledList<IntPoint> _forcedTileUpdates = [];
    private PooledList<int> _removedEntities = new(50);

    public override void Tick(ref RealmTime time) {
        foreach (ref var sight in this) {
            ref var playerStats = ref _world.EntityStats.Get(sight.Id);
            if (playerStats.Id == 0)
                continue;

            var user = _world.PlayerToUser[playerStats.Id];
            ProcessUpdate(user, ref playerStats, ref sight);
            ProcessNewtick(user, ref sight);
        }

        _forcedTileUpdates.Clear();
    }

    public void TileUpdate(IntPoint pos) {
        _forcedTileUpdates.Add(pos);
    }

    private void ProcessUpdate(User user, ref EntityStats playerStats, ref PlayerSight sight) {
        GetNewTiles(ref playerStats, ref sight);
        ProcessEntities(ref playerStats, ref sight);

        if (_newTiles.Count == 0 && _newEntities.Count == 0 && _dropEntities.Count == 0)
            return;

        user.SendPacket(new Update(_newTiles.AsSpan(), _newEntities.AsSpan(), _dropEntities.AsSpan()));
    }

    private void GetNewTiles(ref EntityStats playerStats, ref PlayerSight sight) {
        _newTiles.Reset(); // Starts counting at 0

        var pX = (int)playerStats.Pos.X;
        var pY = (int)playerStats.Pos.Y;
        var width = _world.Map.Data.Width;
        var height = _world.Map.Data.Height;

        sight.VisibleTiles.Clear();
        switch (_world.Config.Blocksight) {
            case World.UNBLOCKED_SIGHT:
                for (var y = pY - SIGHT_RADIUS; y <= pY + SIGHT_RADIUS; y++)
                    for (var x = pX - SIGHT_RADIUS; x <= pX + SIGHT_RADIUS; x++)
                        if (x >= 0 && x < width && y >= 0 && y < height &&
                            playerStats.TileDistSqr(x, y) <= SIGHT_RADIUS_SQR) {
                            AddVisibleTile(x, y, ref sight);
                        }

                break;
            case World.LINE_OF_SIGHT:
                // Always add origin
                AddVisibleTile(pX, pY, ref sight);

                // Scan all 8 octants
                for (var octant = 0; octant < 8; octant++) {
                    Scan(1, 0.0f, 1.0f, octant, pX, pY, width, height, ref sight);
                }
                break;
        }
    }

    private void Scan(int row, float startSlope, float endSlope, int octant, int px, int py,
        int width, int height, ref PlayerSight sight) {
        if (startSlope >= endSlope || row > SIGHT_RADIUS)
            return;

        var nextStartSlope = startSlope;
        var prevTileBlocked = false;
        for (var col = 0; col <= row; col++) {
            // Calculate slopes for the edges of the current tile
            var leftSlope = (col - 0.5f) / row;
            var rightSlope = (col + 0.5f) / row;

            // Skip tiles outside the current frustum
            if (rightSlope < startSlope)
                continue;

            if (leftSlope > endSlope)
                break;

            var (wx, wy) = OctantTransform(octant, px, py, row, col);
            // Bounds and Distance Check
            if (wx >= 0 && wx < width && wy >= 0 && wy < height) {
                float dx = wx - px;
                float dy = wy - py;
                if ((dx * dx + dy * dy) <= SIGHT_RADIUS_SQR) {
                    AddVisibleTile(wx, wy, ref sight);
                }
            }

            var currentBlocked = (wx < 0 || wx >= width || wy < 0 || wy >= height) || _world.Map[wx, wy].BlocksSight;
            if (prevTileBlocked && !currentBlocked) {
                // Transition from blocked to transparent: update the start slope for this frustum
                nextStartSlope = leftSlope;
            }
            else if (!prevTileBlocked && currentBlocked && col > 0) {
                // Transition from transparent to blocked: split the frustum and scan the "branch"
                Scan(row + 1, nextStartSlope, leftSlope, octant, px, py, width, height, ref sight);
            }

            prevTileBlocked = currentBlocked;
        }

        // If the last tile in the row was transparent, keep scanning the next row
        if (!prevTileBlocked) {
            Scan(row + 1, nextStartSlope, endSlope, octant, px, py, width, height, ref sight);
        }
    }

    private void AddVisibleTile(int x, int y, ref PlayerSight sight) {
        var tile = _world.Map[x, y];
        sight.VisibleTiles.Add(tile.Pos);
        if (_forcedTileUpdates.Contains(tile.Pos) || sight.DiscoveredTiles.Add(tile.Pos)) {
            _newTiles.Add(tile);
        }
    }

    private void ProcessEntities(ref EntityStats playerStats, ref PlayerSight sight) {
        _newEntities.Reset();
        _dropEntities.Reset();
        _removedEntities.Reset();

        foreach (var enId in sight.VisibleEntities) {
            ref var stats = ref _world.EntityStats.Get(enId);
            if (IsInSight(_world.Config.Blocksight, ref sight, ref stats)) {
                continue;
            }

            _removedEntities.Add(enId);
            _dropEntities.Add(new ObjectDropData() {
                ObjectId = enId
            });
        }

        foreach (var enId in _removedEntities) {
            sight.VisibleEntities.Remove(enId);
        }

        sight.Statuses.Reset();
        foreach (ref var en in _world.Map.GetEntitiesWithin(playerStats.Pos, SIGHT_RADIUS_SQR)) {
            ref var stats = ref _world.EntityStats.Get(en.Id);
            if (!IsInSight(_world.Config.Blocksight, ref sight, ref stats)) {
                continue;
            }

            if (stats.StatUpdateCount != 0 || stats.PositionUpdate) { // Track status for newtick
                sight.Statuses.Add(new ObjectStatusData {
                    ObjectId = en.Id,
                    Pos = stats.Pos,
                    StatUpdates = stats.StatUpdates,
                    StatCount = stats.StatUpdateCount,
                    PrivacyMask = en.Id == playerStats.Id ? stats.PrivateMask : stats.PublicMask
                });
            }

            if (sight.VisibleEntities.Add(en.Id)) {
                _newEntities.Add(new ObjectData() {
                    ObjectType = en.ObjectType,
                    Status = new ObjectStatusData() {
                        ObjectId = en.Id,
                        Pos = stats.Pos,
                        Stats = stats.Stats,
                        StatCount = EntityStats.STAT_COUNT,
                        PrivacyMask = en.Id == playerStats.Id ? stats.PrivateMask : stats.PublicMask,
                    }
                });
            }
        }
    }

    private void ProcessNewtick(User user, ref PlayerSight sight) {
        user.SendPacket(new NewTick(sight.Statuses));
    }

    private static bool IsInSight(int blocksight, ref PlayerSight sight, ref EntityStats stats) {
        switch (blocksight) {
            case World.UNBLOCKED_SIGHT:
                return true; // This entity is already in SIGHT_RADIUS
            case World.LINE_OF_SIGHT:
                return sight.VisibleTiles.Contains(stats.Tile.Pos);
            default:
                return true;
        }
    }

    private static (int wx, int wy) OctantTransform(int octant, int px, int py, int row, int col) =>
        octant switch {
            0 => (px + col, py - row),
            1 => (px + row, py - col),
            2 => (px + row, py + col),
            3 => (px + col, py + row),
            4 => (px - col, py + row),
            5 => (px - row, py + col),
            6 => (px - row, py - col),
            7 => (px - col, py - row),
            _ => (px, py)
        };
}