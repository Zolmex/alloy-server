using System.Numerics;
using System.Runtime.CompilerServices;
using Arch.Core;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Components;

[InlineArray(ProjectileData.MAX_HITS)]
public struct HitBuffer {
    private EntityId _;
}

public struct ProjectileData : IEntityIdentifiable {
    public const int MAX_HITS = 10;

    public static ProjectileData Null = new();
    
    public EntityId Id { get; set; }
    public readonly long EndTime => StartTime + LifetimeMs;
    
    public Entity Owner;
    public ushort LocalId;
    public long StartTime;
    public WorldPosData StartPos;
    public ProjectilePath Path;
    public float Angle;
    public int Damage;
    public int LifetimeMs;
    public bool MultiHit;
    public HitBuffer Hits;
    public byte HitCount;

    public Vector2 PositionAt(ref RealmTime time) {
        return (Vector2)StartPos + Path.PositionAt((int)(time.TotalElapsedMs - StartTime), LocalId, Angle);
    }
    
    public readonly bool IsDead(ref RealmTime time) {
        if (time.TotalElapsedMs >= EndTime)
            return true;
        return !MultiHit && HitCount != 0;
    }

    public bool TryMarkHit(Entity target) {
        var targetId = (EntityId)target;
        for (var i = 0; i < HitCount; i++)
            if (Hits[i] == targetId)
                return false;

        if (HitCount < MAX_HITS)
            Hits[HitCount++] = targetId;
        return true;
    }
}