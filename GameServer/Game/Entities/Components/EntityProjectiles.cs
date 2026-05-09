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

    public readonly PooledList<int> TargetIds = new(); // Possible hit targets
    
    private readonly World _world;
    private readonly PooledList<int> _projectileIds = new(); // Stores global Ids
    private long _timeToClear;
    
    public EntityProjectiles(World world, ref Entity en) {
        Id = en.Id;
        _world = world;
    }

    public void Tick(ref RealmTime time) {
        if (time.TotalElapsedMs >= _timeToClear)
            ClearTargets();
    }

    public void ScheduleClear(long timeToClear) {
        _timeToClear = timeToClear;
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

    public void AddTarget(int targetId) {
        TargetIds.Add(targetId);
    }
    
    public void ClearTargets() {
        TargetIds.Clear();
    }
    
    public void Dispose() {
        _projectileIds.Dispose();
        TargetIds.Dispose();
    }
}