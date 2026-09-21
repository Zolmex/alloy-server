using System.Collections.Concurrent;
using Arch.Core;
using Arch.System;
using Collections.Pooled;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public class ProjectileSystem(World world) : BaseSystem<World, RealmTime>(world) {

    private readonly SparseSet<ProjectileData> _projectiles = new(100);
    private readonly Dictionary<Entity, PooledDictionary<ushort, EntityId>> _ownerProjectiles = [];
    private readonly ConcurrentQueue<(Entity Owner, ushort LocalId, EntityId ProjId)> _pendingRemove = [];
    
    public void Tick(ref RealmTime time) {
        while (_pendingRemove.TryDequeue(out var remove))
            _projectiles.Remove(remove.ProjId, out _);

        foreach (ref var proj in _projectiles) {
            if (proj.IsDead(ref time)) {
                Remove((proj.Owner, proj.LocalId, proj.Id));
                continue;
            }
            
            // TODO: Hit validation goes here
            var pos = proj.PositionAt(ref time);
        }
    }
    
    public void Create(ref ProjectileData proj) {
        _projectiles.Push(ref proj);
        
        // Map projectile to owner's projectile list
        if (!_ownerProjectiles.TryGetValue(proj.Owner, out var projectiles))
            projectiles = _ownerProjectiles[proj.Owner] = new PooledDictionary<ushort, EntityId>();
        projectiles.Add(proj.LocalId, proj.Id);
    }

    public void Remove((Entity Owner, ushort LocalId, EntityId ProjId) remove) {
        _pendingRemove.Enqueue(remove);
        
        if (_ownerProjectiles.TryGetValue(remove.Owner, out var projectiles))
            projectiles.Remove(remove.LocalId);
    }

    public void RemoveOwner(Entity owner) {
        if (!_ownerProjectiles.Remove(owner, out var projectiles))
            return;
        
        foreach (var (_, projId) in projectiles)
            _projectiles.Remove(projId, out _);
        
        projectiles.Dispose();
    }

    public ref ProjectileData Get(Entity owner, ushort localId) {
        if (!_ownerProjectiles.TryGetValue(owner, out var projectiles) ||
            !projectiles.TryGetValue(localId, out var projId))
            return ref ProjectileData.Null;

        ref var proj = ref _projectiles.Get(projId);
        if (proj.LocalId == localId)
            return ref proj;
        
        return ref ProjectileData.Null;
    }

    public void TryHitEntity(ref ProjectileData proj, Entity from, Entity target) {
        if (!proj.TryMarkHit(target)) // Make sure we don't hit the same entity twice
            return;
        
        var dmg = proj.Damage; // TODO: Condition effects checks + other damage alterations
        World.DamageSystem.Damage(new DamageRecord() {
            Damage = dmg,
            From = from,
            Target = target
        });

        if (!proj.MultiHit)
            Remove((proj.Owner, proj.LocalId, proj.Id));
    }
}