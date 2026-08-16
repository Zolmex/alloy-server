using System.Collections;
using System.Collections.Generic;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Systems;

public class EntityManager : ManagerBase<Entity> {

    public int Count => _idxCounter;
    
    private readonly Stack<int> _freeIdxs;
    
    private int _idxCounter;

    public EntityManager(World world, int capacity) : base(world, capacity) {
        _freeIdxs = new Stack<int>(capacity);
    }

    public override ref Entity Add(ref Entity elem) {
        int index = _freeIdxs.Count > 0 ? _freeIdxs.Pop() : ++_idxCounter;
        elem.Id = new EntityId(index, Set.MoveNextGeneration(index));
        return ref Set.Add(ref elem);
    }

    public override void Remove(EntityId id) {
        Set.Remove(id, out _);
        _freeIdxs.Push(id.Index);
    }

    public override void Tick(ref RealmTime time) {
    }
}