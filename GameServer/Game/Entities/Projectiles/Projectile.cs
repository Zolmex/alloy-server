using System.Numerics;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Projectiles;

public struct Projectile : IIdentifiable, IDisposable {
    public int Id { get; set; }

    public readonly WorldPosData StartPos;
    public readonly int OwnerId;
    public readonly ushort ContainerType;
    public readonly byte ProjectilePropsId;
    public readonly float Angle;
    public readonly short Damage;
    public readonly Guid ProjectilePathId;
    public readonly long StartTime;
    public readonly long EndTime;

    public ProjectileProps Props => XmlLibrary.ObjectDescs[ContainerType].Projectiles[ProjectilePropsId].Props;
    public ProjectilePath Path => _world.Projectiles.Paths[ProjectilePathId];

    private readonly World _world;
    
    public Projectile(World world, WorldPosData startPos, int ownerId, ushort containerType, byte propsId, float angle, short damage, Guid projectilePathId, ref RealmTime time) {
        _world = world;
        StartPos = startPos;
        OwnerId = ownerId;
        ContainerType = containerType;
        ProjectilePropsId = propsId;
        Angle = angle;
        Damage = damage;
        ProjectilePathId = projectilePathId;
        StartTime = time.TotalElapsedMs;
        EndTime = StartTime + Props.LifetimeMS;
    }

    public bool Tick(ref RealmTime time) {
        if (time.TotalElapsedMs >= EndTime)
            return true;
        
        // Hit validation? idk projectile logic goes here
        return false;
    }

    public void TryHitEntity(ref Entity en) {
        var totalDamage = Damage; // TODO: Condition effects checks + other damage alterations
        ref var combat = ref _world.EntityCombat.Get(en.Id);
        combat.Damage(totalDamage);
    }

    public void Dispose() {
        // TODO release managed resources here
    }
}