using System;
using System.Buffers;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityProjectiles : IEntityIdentifiable, IDisposable {
    public const int MAX_PROJECTILES = 2000; // Maximum amount of concurrent projectiles
    
    public EntityId Id { get; set; }

    public readonly PooledList<EntityId> TargetIds = new(); // Possible hit targets
    
    private readonly World _world;
    private readonly EntityId[] _localToGlobalIds = ArrayPool<EntityId>.Shared.Rent(2000); // Stores global Ids
    private long _timeToClear;
    private ushort _nextProjId;
    
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
    
    public ushort Add(EntityId globalId) { // Returns local projectile id
        if (_nextProjId >= MAX_PROJECTILES)
            _nextProjId = 0;
        
        var ret = _nextProjId++;
        _localToGlobalIds[ret] = globalId;
        return ret;
    }

    public void Remove(int localId) {
        _localToGlobalIds[localId] = EntityId.Null;
    }

    public EntityId GetGlobalId(int localId) {
        return _localToGlobalIds[localId];
    }

    public void AddTarget(EntityId targetId) {
        TargetIds.Add(targetId);
    }
    
    public void ClearTargets() {
        TargetIds.Clear();
    }
    
    public void Dispose() {
        ArrayPool<EntityId>.Shared.Return(_localToGlobalIds);
        TargetIds.Dispose();
    }
}