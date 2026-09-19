using Arch.Core;
using Common.Game;
using Common.Utilities;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Components;

public struct ProjectileData : IEntityIdentifiable {
    public EntityId Id { get; set; }
    public long EndTime => StartTime + LifetimeMs;
    
    public Entity Owner;
    public int OwnerAccId;
    public long StartTime;
    // TODO: Path data
    public float Angle;
    public int Damage;
    public int LifetimeMs;
    public bool MultiHit;
    public byte HitCount;

    public bool IsDead(ref RealmTime time) {
        if (time.TotalElapsedMs >= EndTime)
            return true;
        return !MultiHit && HitCount != 0;
    }
}