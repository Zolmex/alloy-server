using System.Collections.Generic;
using Common.Game;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities;

public abstract class ManagerBase<T> where T : struct, IEntityIdentifiable, IDisposable {

    public readonly SparseSet<T> Set;
    protected readonly World _world;
    
    protected ManagerBase(World world, int capacity, bool proj = false) {
        _world = world;
        Set = new SparseSet<T>(capacity, capacity, proj);
    }
    
    public virtual ref T Add(ref T elem) {
        return ref Set.Add(ref elem);
    }

    public virtual void Remove(EntityId id) {
        if (Set.Remove(id, out var elem))
            elem.Dispose();
    }

    public ref T Get(EntityId id) {
        return ref Set.Get(id);
    }

    public abstract void Tick(ref RealmTime time);
    
    public SparseEnumerator<T> GetEnumerator() => new(Set);
}