using System.Diagnostics;
using Arch.Core;
using Arch.System;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network.Messaging.Outgoing;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public readonly record struct DamageRecord(Entity From, Entity Target, int Damage) {
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

        foreach (var record in records) {
            combat.ApplyDamage(record.Value.Damage);
        }
        
        combat.Tick(World, entity, ref stats);
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
        if (!_records.TryGetValue(record.Target, out var records))
            _records[record.Target] = records = new Dictionary<Entity, DamageRecord>();
        
        if (!records.TryGetValue(record.From, out var prev)) {
            records[record.From] = record;
            SendNotification(record.Target, record.Damage);
            return;
        }

        records[record.From] = prev + record;
        SendNotification(record.Target, record.Damage);
    }

    private void SendNotification(Entity entity, int damage) {
        var user = world.Users[entity];
        user.SendPacket(new Notification((EntityId)entity, "-" + damage, 0xFF0000, 24));
    }
}