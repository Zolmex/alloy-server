using System.Numerics;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Projectiles;

public struct Projectile : IEntityIdentifiable, IDisposable {
    public const float HIT_DIST_SQR = 0.5f * 0.5f;
    
    public EntityId Id { get; set; }

    public readonly WorldPosData StartPos;
    public readonly EntityId OwnerId;
    public readonly long StartTime;
    public readonly PooledList<EntityId> Hit = [];
    
    public ProjectilePath Path;
    public float Angle;
    public int Damage;
    public int LifetimeMs;
    public long EndTime;
    public bool MultiHit;
    
    public ushort LocalId;

    private readonly World _world;
    
    public Projectile(World world, WorldPosData startPos, EntityId ownerId, ref RealmTime time) {
        _world = world;
        StartPos = startPos;
        OwnerId = ownerId;
        StartTime = time.TotalElapsedMs;
    }

    public void SetProps(ProjectilePath path, float angle, int damage, int lifetimeMs, bool multiHit) {
        Angle = angle.Deg2Rad();
        Damage = damage;
        Path = path;
        LifetimeMs = lifetimeMs;
        EndTime = StartTime + lifetimeMs;
        MultiHit = multiHit;
    }

    public void SetLocalId(ushort localId) {
        LocalId = localId;
    }

    public bool Tick(ref RealmTime time) {
        if (time.TotalElapsedMs >= EndTime)
            return true;

        if (!MultiHit && Hit.Count != 0)
            return false;

        var pos = StartPos + Path.PositionAt((int)(time.TotalElapsedMs - StartTime), LocalId, Angle);
        var targets = _world.EntityProjectiles.Get(OwnerId);
        if (targets.Id == EntityId.Null)
            return true;
        
        foreach (var targetId in targets.TargetIds) {
            if (!MultiHit && Hit.Count != 0)
                break;

            ref var targetStats = ref _world.EntityStats.Get(targetId);
            if (targetStats.DistSqr(pos.X, pos.Y) <= HIT_DIST_SQR)
                TryHitEntity(targetId);
        }
        return false;
    }

    public void TryHitEntity(EntityId enId) {
        if (Hit.Add(enId, true) == -1)
            return;

        var totalDamage = Damage; // TODO: Condition effects checks + other damage alterations
        ref var combat = ref _world.EntityCombat.Get(enId);
        combat.Damage(OwnerId, totalDamage);
    }

    public void Dispose() {
        Hit.Dispose();
    }
}