using System.Collections.Generic;
using Common.Game;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities;

public abstract class ManagerBase<T> where T : struct, IEntityComponent {

    protected readonly SparseSet<T> _set;
    protected readonly World _world;
    
    protected ManagerBase(World world, int capacity) {
        _world = world;
        _set = new SparseSet<T>(capacity);
    }
    
    public ref T Add(ref T elem, int entityId) {
        elem.Id = entityId;
        return ref _set.Add(ref elem);
    }

    public virtual void Remove(int id) {
        _set.Remove(id);
    }

    public ref T Get(int id) {
        return ref _set.Get(id);
    }

    public abstract void Tick(ref RealmTime time);
}