using System.Collections.Generic;
using Common.Game;
using Common.Game.Components;
using Common.Game.Systems;
using Common.Utilities.Collections;

namespace Common.Game;

public class EntityManager {
    protected readonly SparseSet<Entity> _set;
    protected readonly Stack<int> _freeIds;
    
    private int _idCounter;
    
    public EntityManager(int capacity) {
        _set = new SparseSet<Entity>(capacity);
        _freeIds = new Stack<int>(capacity);
    }
    
    public virtual void Add(ref Entity elem) {
        elem.Id = _freeIds.Count > 0 ? _freeIds.Pop() : _idCounter++;
        _set.Add(ref elem);
    }

    public virtual void Remove(int id) {
        _set.Remove(id);
        _freeIds.Push(id);
    }

    public ref Entity Get(int id) {
        return ref _set.Get(id);
    }
}