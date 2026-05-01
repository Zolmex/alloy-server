using Common.Game;
using Common.Utilities.Collections;

namespace GameServer.Game.Worlds;

public class EntityManager {

    public int Count => _entities.Count;
    
    private readonly World _world;
    private readonly SparseSet<Entity> _entities;
    private readonly Stack<int> _freeIds;

    private int _idCounter;

    public EntityManager(World world) {
        _world = world;
        
        var capacity = world.Config.Name == "Realm" ? 50_000 : 5_000;
        _entities = new SparseSet<Entity>(capacity);
        _freeIds = new Stack<int>(capacity);
    }

    public void Add(ref Entity en) {
        en.Id = _freeIds.Count > 0 ? _freeIds.Pop() : _idCounter++;
        _entities.Add(ref en);
    }

    public void Remove(int id) {
        _entities.Remove(id);
        _freeIds.Push(id);
    }

    public ref Entity Get(int id) {
        return ref _entities.Get(id);
    }
}