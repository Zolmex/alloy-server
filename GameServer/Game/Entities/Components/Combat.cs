using Arch.Core;
using Common;
using Common.Game;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Components;

public struct Combat {
    
    public int DamageReceived;
    
    public void Tick(World world, Entity entity, ref Stats stats) {
        var hp = stats.GetInt(StatType.HP);
        var newHp = hp - DamageReceived;
        stats.Set(StatType.HP, newHp);

        if (newHp <= 0)
            Death(world, entity);
        
        DamageReceived = 0;
    }
    
    public void ApplyDamage(int damage) {
        DamageReceived += damage;
    }
    
    private void Death(World world, Entity entity) {
        if (world.Ecs.Has<PlayerTag>(entity)) {
            // TODO: Spawn gravestone, announce death, register death in database
            world.Users[entity].Disconnect(reason: DisconnectReason.Death);
            return;
        }
        world.LeaveWorld(entity);
    }
}