using System.Collections.Generic;
using Common.Game.Components;
using Common.Utilities;
using Common.Utilities.Collections;

namespace Common.Game;

public abstract class ManagerBase<T> where T : struct, IIdentifiable {

    protected readonly SparseSet<T> _set;
    
    protected ManagerBase(int capacity) {
        _set = new SparseSet<T>(capacity);
    }
    
    public void Add(ref T elem, int entityId) {
        elem.Id = entityId;
        _set.Add(ref elem);
    }

    public virtual void Remove(int id) {
        _set.Remove(id);
    }

    public ref T Get(int id) {
        return ref _set.Get(id);
    }

    public abstract void Tick(ref RealmTime time);
}