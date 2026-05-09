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

public struct Projectile : IIdentifiable, IDisposable {
    public const float HIT_DIST_SQR = 0.5f * 0.5f;
    
    public int Id { get; set; }

    public readonly WorldPosData StartPos;
    public readonly int OwnerId;
    public readonly ushort ContainerType;
    public readonly byte ProjectilePropsId;
    public readonly float Angle;
    public readonly int Damage;
    public readonly ProjectilePath Path;
    public readonly long StartTime;
    public readonly long EndTime;
    public readonly PooledList<int> Hit = [];
    
    public ushort LocalId;

    public ProjectileProps Props => XmlLibrary.ObjectDescs[ContainerType].Projectiles[ProjectilePropsId].Props;

    private readonly World _world;
    
    public Projectile(World world, WorldPosData startPos, int ownerId, ushort containerType, byte propsId, float angle, int damage, ProjectilePath path, ref RealmTime time) {
        _world = world;
        StartPos = startPos;
        OwnerId = ownerId;
        ContainerType = containerType;
        ProjectilePropsId = propsId;
        Angle = angle.Deg2Rad();
        Damage = damage;
        Path = path;
        StartTime = time.TotalElapsedMs;
        EndTime = StartTime + Props.LifetimeMS;
    }

    public void SetLocalId(ushort localId) {
        LocalId = localId;
    }

    public bool Tick(ref RealmTime time) {
        if (time.TotalElapsedMs >= EndTime)
            return true;

        var pos = StartPos + Path.PositionAt((int)(time.TotalElapsedMs - StartTime), LocalId, Angle);
        var targets = _world.EntityProjectiles.Get(OwnerId).TargetIds;
        foreach (var targetId in targets) {
            ref var targetStats = ref _world.EntityStats.Get(targetId);
            if (targetStats.DistSqr(pos.X, pos.Y) <= HIT_DIST_SQR)
                TryHitEntity(targetId);
        }
        return false;
    }

    public void TryHitEntity(int enId) {
        if (Hit.Count != 0 && !Props.MultiHit) {
            return;
        }

        var totalDamage = Damage; // TODO: Condition effects checks + other damage alterations
        ref var combat = ref _world.EntityCombat.Get(enId);
        combat.Damage(totalDamage);
        Hit.Add(enId, true);
    }

    public void Dispose() {
        Hit.Dispose();
    }
}