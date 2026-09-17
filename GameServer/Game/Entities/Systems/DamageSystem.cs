using System.Diagnostics;
using Arch.Core;
using Arch.System;
using Common;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public record struct DamageRecord(Entity From, Entity Target, int Damage) {

    public void Reset() {
        Damage = 0;
    }
    
    public static DamageRecord operator +(DamageRecord left, DamageRecord right)
        => left with { Damage = left.Damage + right.Damage };

    public static DamageRecord operator +(DamageRecord record, int damage)
        => record with { Damage = record.Damage + damage };
}

public partial class DamageSystem(World world) : BaseSystem<World, RealmTime>(world) {

    private readonly Dictionary<Entity, Dictionary<Entity, DamageRecord>> _records = [];

    [Query(Parallel = true)] // Keep Parallel as long as you don't modify _records or its entries at all
    public void Process(Entity entity, ref Combat combat, ref Stats stats) {
        if (!_records.TryGetValue(entity, out var records))
            return;

        foreach (var (en, record) in records) {
            combat.DamageReceived += record.Damage;
            record.Reset();
        }
        
        var hp = stats.GetInt(StatType.HP);
        var newHp = hp - combat.DamageReceived;
        stats.Set(StatType.HP, newHp);

        if (newHp <= 0)
            Death(entity);
        
        combat.DamageReceived = 0;
    }
    
    private void Death(Entity entity) {
        if (World.Ecs.Has<PlayerTag>(entity)) {
            // TODO: Spawn gravestone, announce death, register death in database
            World.Users[entity].Disconnect(reason: DisconnectReason.Death);
            return;
        }
        World.LeaveWorld(entity);
    }

    public void Damage(DamageRecord record) {
        if (!_records.TryGetValue(record.Target, out var records))
            _records[record.Target] = records = new Dictionary<Entity, DamageRecord>();
        
        if (!records.TryGetValue(record.From, out var prev)) {
            records[record.From] = record;
            return;
        }

        records[record.From] = prev + record;
    }
    
    public void DamageWithText(DamageRecord record) {
        Damage(record);
        var user = World.Users[record.Target];
        user.SendPacket(new Notification((EntityId)record.Target, "-" + record.Damage, 0xFF0000, 24));
    }
}