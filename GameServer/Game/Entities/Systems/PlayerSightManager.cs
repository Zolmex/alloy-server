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
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameServer.Game.Entities.Systems;

public class PlayerSightManager(World world, int capacity) : ManagerBase<PlayerSight>(world, capacity) {
    private struct Frustum
    {
        public int Row;
        public float Start;
        public float End;
    }

    public const int SIGHT_RADIUS = 20;
    public const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;

    private PooledList<MapTileData> _newTiles = new(50);
    private PooledList<ObjectData> _newEntities = new(50);
    private PooledList<ObjectDropData> _dropEntities = new(50);
    private PooledList<IntPoint> _forcedTileUpdates = [];
    private PooledList<EntityId> _removedEntities = new(50);

    private Dictionary<EntityId, ObjectData> _entityDataCache = new(200);
    private Dictionary<EntityId, ObjectStatusData> _entityStatusCache = new(200);

    public override void Tick(ref RealmTime time) {
        _entityDataCache.Clear();
        _entityStatusCache.Clear();

        foreach (ref var sight in this) {
            ref var playerStats = ref _world.EntityStats.Get(sight.Id);
            if (playerStats.Id == EntityId.Null)
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
        // A radius of 20 will never need more than ~20-30 stack depth
        Span<Frustum> stack = stackalloc Frustum[SIGHT_RADIUS + 5];
        int stackIdx = 0;

        // Push initial frustum
        stack[stackIdx++] = new Frustum { Row = 1, Start = 0f, End = 1f };

        while (stackIdx > 0)
        {
            var f = stack[--stackIdx];
            if (f.Row > SIGHT_RADIUS || f.Start >= f.End) continue;

            float nextStartSlope = f.Start;
            bool prevTileBlocked = false;

            for (int col = 0; col <= f.Row; col++)
            {
                float leftSlope = (col - 0.5f) / f.Row;
                float rightSlope = (col + 0.5f) / f.Row;

                if (rightSlope < f.Start) continue;
                if (leftSlope > f.End) break;

                var (wx, wy) = OctantTransform(octant, px, py, f.Row, col);

                // Inline the visibility check logic
                if (wx >= 0 && wx < width && wy >= 0 && wy < height)
                {
                    int dx = wx - px;
                    int dy = wy - py;
                    if ((dx * dx + dy * dy) <= SIGHT_RADIUS_SQR)
                    {
                        AddVisibleTile(wx, wy, ref sight);
                    }
                }

                bool currentBlocked = (wx < 0 || wx >= width || wy < 0 || wy >= height) || _world.Map[wx, wy].BlocksSight;

                if (prevTileBlocked && !currentBlocked)
                {
                    nextStartSlope = leftSlope;
                }
                else if (!prevTileBlocked && currentBlocked && col > 0)
                {
                    // Instead of recursion, push the "branch" to our stack
                    if (stackIdx < stack.Length)
                    {
                        stack[stackIdx++] = new Frustum { Row = f.Row + 1, Start = nextStartSlope, End = leftSlope };
                    }
                }
                prevTileBlocked = currentBlocked;
            }

            if (!prevTileBlocked)
            {
                stack[stackIdx++] = new Frustum { Row = f.Row + 1, Start = nextStartSlope, End = f.End };
            }
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
            if (stats.Id != EntityId.Null && IsInSight(_world.Config.Blocksight, ref sight, ref stats))
                continue;

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
            if (stats.Id == EntityId.Null || !IsInSight(_world.Config.Blocksight, ref sight, ref stats)) {
                continue;
            }

            if (stats.StatUpdateCount != 0 || stats.PositionUpdate) { // Track status for newtick
                ObjectStatusData status;
                if (!_entityStatusCache.TryGetValue(en.Id, out status))
                {
                    status = new ObjectStatusData
                    {
                        ObjectId = en.Id,
                        Pos = stats.Pos,
                        StatUpdates = stats.StatUpdates,
                        StatCount = stats.StatUpdateCount,
                        PrivacyMask = stats.PublicMask
                    };
                    _entityStatusCache[en.Id] = status;
                }

                if (en.Id == playerStats.Id)
                    status.PrivacyMask = stats.PrivateMask;
                sight.Statuses.Add(status);
            }

            if (sight.VisibleEntities.Add(en.Id)) {
                ObjectData objData;
                if (!_entityDataCache.TryGetValue(en.Id, out objData))
                {
                    objData = new ObjectData()
                    {
                        ObjectType = en.ObjectType,
                        Status = new ObjectStatusData()
                        {
                            ObjectId = en.Id,
                            Pos = stats.Pos,
                            Stats = stats.Stats,
                            StatCount = EntityStats.STAT_COUNT,
                            PrivacyMask = stats.PublicMask, // Set mask as public initially, reset when saving
                        }
                    };
                    _entityDataCache[en.Id] = objData;
                }

                if (en.Id == playerStats.Id)
                    objData.Status.PrivacyMask = stats.PrivateMask;
                _newEntities.Add(objData);
            }
        }
    }

    private void ProcessNewtick(User user, ref PlayerSight sight) {
        user.SendPacket(new NewTick(sight.Statuses));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsInSight(int blocksight, ref PlayerSight sight, ref EntityStats stats) {
        if (blocksight == World.UNBLOCKED_SIGHT)
            return true;
        return sight.VisibleTiles.Contains(stats.Tile.Pos); // Line of Sight
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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