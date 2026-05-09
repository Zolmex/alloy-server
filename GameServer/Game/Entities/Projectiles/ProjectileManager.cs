using System.Collections.Immutable;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Projectiles;

public class ProjectileManager : ManagerBase<Projectile> {

    public ImmutableDictionary<Guid, ProjectilePath> Paths = [];
    
    private readonly World _world;
    private readonly Stack<int> _freeIds;
    
    private int _idCounter; // First element starts at id = 1
    
    public ProjectileManager(World world, int capacity) : base(world, capacity) {
        _world = world;
        _freeIds = new Stack<int>(capacity);
    }
    
    public override ref Projectile Add(ref Projectile elem) {
        elem.Id = _freeIds.Count > 0 ? _freeIds.Pop() : ++_idCounter;
        return ref Set.Add(ref elem);
    }

    public override void Remove(int id) {
        Set.Remove(id, out var elem);
        _freeIds.Push(id);
        elem.Dispose();
    }

    public void RegisterPath(ProjectilePath path) {
        Paths = Paths.Add(path.Id, path);
    }
    
    public void DiscardPath(Guid id) {
        Paths = Paths.Remove(id);
    }

    public override void Tick(ref RealmTime time) {
        foreach (ref var proj in this) {
            if (proj.Tick(ref time)) {
                ref var ownerProjs = ref _world.EntityProjectiles.Get(proj.OwnerId);
                ownerProjs.Remove(proj.Id);
                Remove(proj.Id);
            }
        }
    }
}