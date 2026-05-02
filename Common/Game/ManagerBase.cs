using System.Collections.Generic;
using Common.Game.Components;
using Common.Utilities;
using Common.Utilities.Collections;

namespace Common.Game;

public abstract class ManagerBase<T> where T : struct, IIdentifiable {

    protected readonly SparseSet<T> _set;
    protected readonly Stack<int> _freeIds;
    
    private int _idCounter;
    
    protected ManagerBase(int capacity) {
        _set = new SparseSet<T>(capacity);
        _freeIds = new Stack<int>(capacity);
    }
    
    public void Add(ref T elem) {
        elem.Id = _freeIds.Count > 0 ? _freeIds.Pop() : _idCounter++;
        _set.Add(ref elem);
    }

    public void Remove(int id) {
        _set.Remove(id);
        _freeIds.Push(id);
    }

    public ref T Get(int id) {
        return ref _set.Get(id);
    }
}