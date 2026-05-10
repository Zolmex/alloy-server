using System;
using System.Collections.Immutable;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityCombat : IIdentifiable, IDisposable {
    public int Id { get; set; }

    private readonly World _world;
    public int TotalDamage;
    
    public EntityCombat(World world, ref Entity en) {
        Id = en.Id;
        _world = world;
    }

    public void Damage(int damage) { // Applies damage directly, perform any modifications to the amount before calling this
        TotalDamage += damage;
    }

    public void DamageWithText(int damage) {
        Damage(damage);
        var user = _world.PlayerToUser[Id];
        user.SendPacket(new Notification(Id, "-" + damage, 0xFF0000, 24));
    }

    public void Tick(ref RealmTime time) {
        ref var stats = ref _world.EntityStats.Get(Id);
        var hp = stats.GetInt(StatType.HP);
        var newHp = Math.Max(0, hp - TotalDamage);
        stats.Set(StatType.HP, newHp);
        
        TotalDamage = 0;
    }

    public void Dispose() {
        
    }
}