using System;
using System.Collections.Immutable;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Events;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityEvents : IEntityIdentifiable, IDisposable {
    public EntityId Id { get; set; }

    private readonly World _world;
    
    public EventBus<DeathEvent> OnDeath = new();
    public EventBus<DamageReceivedEvent> OnDamageReceived = new();
    
    public EntityEvents(World world, ref Entity en) {
        Id = en.Id;
        _world = world;
    }

    public void Tick(ref RealmTime time) {
        ref var combat = ref _world.EntityCombat.Get(Id);
        if (combat.TotalDamageReceived > 0) {
            var dmgEvent = new DamageReceivedEvent(_world, Id, combat.TotalDamageReceived);
            OnDamageReceived.Publish(ref dmgEvent);
        }
    }

    public void Dispose() {
        var deathEvent = new DeathEvent(_world, Id);
        OnDeath.Publish(ref deathEvent);
    }
}