using System.Collections;
using System.Collections.Generic;
using Common.Game;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities;

public class EntityManager {
    public int Count => Set.Count;
    
    public readonly SparseSet<Entity> Set;
    
    private readonly Stack<int> _freeIds;
    
    private int _idCounter;

    public EntityManager(int capacity) {
        Set = new SparseSet<Entity>(capacity);
        _freeIds = new Stack<int>(capacity);
    }

    public ref Entity Add(ref Entity elem) {
        elem.Id = _freeIds.Count > 0 ? _freeIds.Pop() : _idCounter++;
        return ref Set.Add(ref elem);
    }

    public virtual void Remove(int id) {
        Set.Remove(id);
        _freeIds.Push(id);
    }

    public ref Entity Get(int id) => ref Set.Get(id);
    
    public SparseEnumerator<Entity> GetEnumerator() => new(Set);
}