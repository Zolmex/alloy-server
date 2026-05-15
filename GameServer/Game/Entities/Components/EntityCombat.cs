using System;
using System.Collections.Immutable;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components.Data;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityCombat : IIdentifiable, IDisposable {
    public int Id { get; set; }

    public int TotalDamageReceived;
    public readonly SparseSet<DamageRecord> DamageRecords;
    
    private readonly World _world;
    
    public EntityCombat(World world, ref Entity en) {
        Id = en.Id;
        _world = world;
        DamageRecords = new SparseSet<DamageRecord>(world.Entities.Count);
    }

    public void Damage(int fromId, int damage) { // Applies damage directly, perform any modifications to the amount before calling this
        TotalDamageReceived += damage;

        ref var record = ref DamageRecords.GetOrAdd(fromId, out var added);
        if (added) {
            record = new DamageRecord(fromId, damage);
        } else {
            record.DamageDealt += damage;
        }
    }

    public void DamageWithText(int fromId, int damage) {
        Damage(fromId, damage);
        var user = _world.PlayerToUser[Id];
        user.SendPacket(new Notification(Id, "-" + damage, 0xFF0000, 24));
    }

    public void Tick(ref RealmTime time) {
        ref var stats = ref _world.EntityStats.Get(Id);
        var hp = stats.GetInt(StatType.HP);
        var newHp = Math.Max(0, hp - TotalDamageReceived);
        stats.Set(StatType.HP, newHp);
        
        TotalDamageReceived = 0;
    }

    public void Dispose() {
    }
}