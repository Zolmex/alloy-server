using System.Numerics;
using Arch.Core;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Utilities;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Components;

public struct ProjectileData : IEntityIdentifiable {
    public EntityId Id { get; set; }
    public long EndTime => StartTime + LifetimeMs;
    
    public Entity Owner;
    public ushort LocalId;
    public long StartTime;
    public ProjectilePath Path;
    public float Angle;
    public int Damage;
    public int LifetimeMs;
    public bool MultiHit;
    public byte HitCount;

    public Vector2 PositionAt(ref RealmTime time) {
        return Path.PositionAt((int)(time.TotalElapsedMs - StartTime), LocalId, Angle);
    }
    
    public bool IsDead(ref RealmTime time) {
        if (time.TotalElapsedMs >= EndTime)
            return true;
        return !MultiHit && HitCount != 0;
    }
}