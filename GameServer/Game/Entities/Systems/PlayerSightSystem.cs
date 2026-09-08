using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Arch.Core;
using Arch.LowLevel;
using Arch.System;
using Collections.Pooled;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Utilities;
using ArchWorld = Arch.Core.World;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public partial class PlayerSightSystem(World world) : BaseSystem<World, RealmTime>(world) {
    private struct Frustum
    {
        public int Row;
        public float Start;
        public float End;
    }
    
    public const int SIGHT_RADIUS = 20;
    public const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;
    
    private readonly PooledList<MapTileData> _newTiles = new(50);
    private readonly PooledList<ObjectData> _newEntities = new(50);
    private UnsafeList<ObjectDropData> _dropEntities = new(50);
    private UnsafeList<IntPoint> _forcedTileUpdates = new(50);
    private UnsafeList<Entity> _removedEntities = new(50);

    private readonly PooledDictionary<Entity, ObjectData> _entityDataCache = new(200);
    private readonly PooledDictionary<Entity, ObjectStatusData> _entityStatusCache = new(200);
    private readonly PooledDictionary<Entity, PlayerSightState> _sightStates = [];
    
    public override void BeforeUpdate(in RealmTime t) {
        _entityDataCache.Clear();
        _entityStatusCache.Clear();
    }

    public override void AfterUpdate(in RealmTime t) {
        _forcedTileUpdates.Clear();
    }

    [Query]
    public void Process([Data] ref RealmTime time, Entity entity, ref Position pos, ref PlayerSight sight) {
        var user = World.Users[entity];
        if (!_sightStates.TryGetValue(entity, out var sightState))
            sightState = _sightStates[entity] = new PlayerSightState(entity, World.Map.Data.Width, World.Map.Data.Height);
        
        ProcessUpdate(user, ref pos, sightState);
        ProcessNewtick(user, sightState);
    }
    
    private void ProcessUpdate(User user, ref Position pos, PlayerSightState sight) {
        GetNewTiles(ref pos, sight);
        ProcessEntities(ref pos, sight);

        if (_newTiles.Count == 0 && _newEntities.Count == 0 && _dropEntities.Count == 0)
            return;

        user.SendPacket(new Update(_newTiles.Span, _newEntities.Span, _dropEntities.AsSpan()));
    }
    
    private void GetNewTiles(ref Position pos, PlayerSightState sight) {
        _newTiles.Clear();

        var pX = (int)pos.Pos.X;
        var pY = (int)pos.Pos.Y;
        var width = World.Map.Data.Width;
        var height = World.Map.Data.Height;

        sight.VisibleTiles.Clear();
        switch (World.Config.Blocksight) {
            case World.UNBLOCKED_SIGHT:
                for (var y = pY - SIGHT_RADIUS; y <= pY + SIGHT_RADIUS; y++)
                    for (var x = pX - SIGHT_RADIUS; x <= pX + SIGHT_RADIUS; x++)
                        if (x >= 0 && x < width && y >= 0 && y < height &&
                            pos.TileDistSqr(x, y) <= SIGHT_RADIUS_SQR) {
                            AddVisibleTile(x, y, sight);
                        }

                break;
            case World.LINE_OF_SIGHT:
                // Always add origin
                AddVisibleTile(pX, pY, sight);

                // Scan all 8 octants
                for (var octant = 0; octant < 8; octant++) {
                    Scan(1, 0.0f, 1.0f, octant, pX, pY, width, height, sight);
                }
                break;
        }
    }

    private void Scan(int row, float startSlope, float endSlope, int octant, int px, int py,
        int width, int height, PlayerSightState sight) {
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
                        AddVisibleTile(wx, wy, sight);
                    }
                }

                bool currentBlocked = (wx < 0 || wx >= width || wy < 0 || wy >= height) || World.Map[wx, wy].BlocksSight;

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

    private void AddVisibleTile(int x, int y, PlayerSightState sight) {
        var tile = World.Map[x, y];
        sight.VisibleTiles.Add(tile.Pos);
        if (_forcedTileUpdates.Contains(tile.Pos) || sight.DiscoveredTiles.Add(tile.Pos)) {
            _newTiles.Add(tile);
        }
    }

    private void ProcessEntities(Entity owner, ref Position ps, PlayerSightState sight) {
        _newEntities.Clear();
        _dropEntities.Clear();
        _removedEntities.Clear();

        foreach (var en in sight.VisibleEntities) {
            ref var pos = ref World.Ecs.Get<Position>(en);
            if (IsVisible(World.Config.Blocksight, sight, en, ref pos))
                continue;

            _removedEntities.Add(en);
            _dropEntities.Add(new ObjectDropData() {
                ObjectId = (EntityId)en
            });
        }

        foreach (var enId in _removedEntities) {
            sight.VisibleEntities.Remove(enId);
        }

        sight.Statuses.Clear();
        foreach (var en in World.Map.GetEntitiesWithin(ps.Pos, SIGHT_RADIUS_SQR)) {
            ref var stats = ref World.Ecs.Get<Stats>(en);
            ref var pos = ref World.Ecs.Get<Position>(en);
            if (IsVisible(World.Config.Blocksight, sight, en, ref pos))
                continue;

            if (stats.StatUpdateCount != 0 || pos.PositionUpdate) { // Track status for newtick
                ObjectStatusData status;
                if (!_entityStatusCache.TryGetValue(en, out status))
                {
                    status = new ObjectStatusData
                    {
                        ObjectId = en.Id,
                        Pos = pos.Pos,
                        StatUpdates = stats.StatUpdates,
                        StatCount = stats.StatUpdateCount,
                        PrivacyMask = stats.PublicMask
                    };
                    _entityStatusCache[en] = status;
                }

                if (en == owner)
                    status.PrivacyMask = stats.PrivateMask;
                sight.Statuses.Add(status);
            }

            if (sight.VisibleEntities.Add(en)) {
                ObjectData objData;
                if (!_entityDataCache.TryGetValue(en, out objData))
                {
                    objData = new ObjectData()
                    {
                        ObjectType = en.ObjectType,
                        Status = new ObjectStatusData()
                        {
                            ObjectId = en.Id,
                            Pos = pos.Pos,
                            Stats = stats.Values,
                            StatCount = StatData.STAT_COUNT,
                            PrivacyMask = stats.PublicMask, // Set mask as public initially, reset when saving
                        }
                    };
                    _entityDataCache[en] = objData;
                }

                if (en == owner)
                    objData.Status.PrivacyMask = stats.PrivateMask;
                _newEntities.Add(objData);
            }
        }
    }

    private void ProcessNewtick(User user, PlayerSightState sight) {
        user.SendPacket(new NewTick(sight.Statuses));
    }

    private bool IsVisible(int blocksight, PlayerSightState sight, Entity en, ref Position pos) {
        if (World.Ecs.Has<Inventory>(en)) {
            var inv = World.Ecs.Get<Inventory>(en);
            var user = World.Users[sight.Owner];
            if (!inv.OwnedBy(user.Session.Account.Id))
                return false;
        }

        return blocksight switch {
            World.UNBLOCKED_SIGHT => true,
            World.LINE_OF_SIGHT => !sight.VisibleTiles.Contains(pos.Pos.ToIntPoint()),
            _ => true
        };;
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