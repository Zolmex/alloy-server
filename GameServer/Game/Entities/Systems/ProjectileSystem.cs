using System.Collections.Concurrent;
using Arch.System;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Systems;

public class ProjectileSystem(World world) : BaseSystem<World, RealmTime>(world) {

    private readonly SparseSet<ProjectileData> _projectiles = new(100);
    private readonly ConcurrentQueue<EntityId> _pendingRemove = [];
    
    public void Tick(ref RealmTime time) {
        while (_pendingRemove.TryDequeue(out var id))
            _projectiles.Remove(id, out _);
        
        foreach (ref var proj in _projectiles)
            if (proj.IsDead(ref time))
                _pendingRemove.Enqueue(proj.Id);
    }
    
    public void Create(ref ProjectileData data) {
        _projectiles.Push(ref data);
    }

    public void Remove(EntityId id) {
        _pendingRemove.Enqueue(id);
    }
}