using System.Collections.Concurrent;
using System.Collections.Immutable;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Projectiles;

public class ProjectileManager : ManagerBase<Projectile> {

    private readonly World _world;
    private readonly Stack<int> _freeIds;
    private readonly Queue<int> _pendingRemove = [];
    
    private int _idCounter; // First element starts at id = 1
    
    public ProjectileManager(World world, int capacity) : base(world, capacity) {
        _world = world;
        _freeIds = new Stack<int>(capacity);
    }

    public override ref Projectile Add(ref Projectile elem) {
        elem.Id = _freeIds.Count > 0 ? _freeIds.Pop() : ++_idCounter;
        return ref Set.Add(ref elem);
    }

    public override void Remove(int id) {
        Set.Remove(id, out var elem);
        if (elem.Id != 0)
            elem.Dispose();
        _freeIds.Push(id);
    }

    public override void Tick(ref RealmTime time) {
        while (_pendingRemove.TryDequeue(out var projId))
            Remove(projId);
        
        foreach (ref var proj in this) {
            if (proj.Tick(ref time)) {
                ref var ownerProjs = ref _world.EntityProjectiles.Get(proj.OwnerId);
                if (ownerProjs.Id != 0)
                    ownerProjs.Remove(proj.LocalId);
                _pendingRemove.Enqueue(proj.Id);
            }
        }
    }
}