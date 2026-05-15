using System.Collections;
using System.Collections.Generic;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Systems;

public class EntityManager : ManagerBase<Entity> {

    public int Count => _idCounter;
    
    private readonly Stack<int> _freeIds;
    
    private int _idCounter;

    public EntityManager(World world, int capacity) : base(world, capacity) {
        _freeIds = new Stack<int>(capacity);
    }

    public override ref Entity Add(ref Entity elem) {
        elem.Id = _freeIds.Count > 0 ? _freeIds.Pop() : _idCounter++;
        return ref Set.Add(ref elem);
    }

    public override void Remove(int id) {
        Set.Remove(id, out _);
        _freeIds.Push(id);
    }

    public override void Tick(ref RealmTime time) {
    }
}