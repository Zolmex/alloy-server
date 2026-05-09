using System;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityProjectiles : IIdentifiable, IDisposable {
    public int Id { get; set; }

    private readonly World _world;
    private readonly PooledList<int> _projectileIds = new(); // Stores global Ids
    
    public EntityProjectiles(World world, ref Entity en) {
        Id = en.Id;
        _world = world;
    }

    public int Add(int projId) { // Returns local projectile id
        return _projectileIds.Add(projId);
    }

    public void Remove(int projId) {
        _projectileIds.Remove(projId);
    }

    public int GetGlobalId(int localProjId) {
        return _projectileIds.GetAt(localProjId);
    }
    
    public void Dispose() {
        _projectileIds.Dispose();
    }
}