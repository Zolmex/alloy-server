using System.Collections.Generic;
using Common.Game;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities;

public abstract class ManagerBase<T> where T : struct, IIdentifiable, IDisposable {

    public readonly SparseSet<T> Set;
    protected readonly World _world;
    
    protected ManagerBase(World world, int capacity) {
        _world = world;
        Set = new SparseSet<T>(capacity, capacity);
    }
    
    public virtual ref T Add(ref T elem) {
        return ref Set.Add(ref elem);
    }

    public virtual void Remove(int id) {
        Set.Remove(id, out var elem);
        if (elem.Id != 0)
            elem.Dispose();
    }

    public ref T Get(int id) {
        return ref Set.Get(id);
    }

    public abstract void Tick(ref RealmTime time);
    
    public SparseEnumerator<T> GetEnumerator() => new(Set);
}