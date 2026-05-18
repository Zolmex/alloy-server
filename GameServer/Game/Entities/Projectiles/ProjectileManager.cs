using System.Collections.Concurrent;
using System.Collections.Immutable;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Utilities.Collections;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Projectiles;

public class ProjectileManager : ManagerBase<Projectile> {

    private readonly World _world;
    private readonly Stack<int> _freeIdxs;
    private readonly Queue<EntityId> _pendingRemove = [];
    
    private int _idxCounter; // First element starts at id = 1
    
    public ProjectileManager(World world, int capacity) : base(world, capacity, true) {
        _world = world;
        _freeIdxs = new Stack<int>(capacity);
    }

    public override ref Projectile Add(ref Projectile elem) {
        int index = _freeIdxs.Count > 0 ? _freeIdxs.Pop() : ++_idxCounter;
        elem.Id = new EntityId(index, Set.MoveNextGeneration(index));
        return ref Set.Add(ref elem);
    }

    public override void Remove(EntityId id) {
        if (Set.Remove(id, out var elem))
            elem.Dispose();
        _freeIdxs.Push(id.Index);
    }

    public override void Tick(ref RealmTime time) {
        while (_pendingRemove.TryDequeue(out var projId))
            Remove(projId);
        
        foreach (ref var proj in this) {
            if (proj.Tick(ref time)) {
                ref var ownerProjs = ref _world.EntityProjectiles.Get(proj.OwnerId);
                if (ownerProjs.Id != EntityId.Null)
                    ownerProjs.Remove(proj.LocalId);
                _pendingRemove.Enqueue(proj.Id);
            }
        }
    }
}