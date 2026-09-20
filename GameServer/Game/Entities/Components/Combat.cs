using Arch.Core;
using Common;
using Common.Game;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Components;

public record struct Combat() {
    public int DamageReceived;
    private ushort _nextProjectileId = 1;
    
    public int GetProjectileDamage(int minDamage, int maxDamage) {
        // TODO: Condition effects
        var dmg = Random.Shared.Next(minDamage, maxDamage);
        return dmg;
    }

    public ushort GetNextProjectileId() {
        var ret = _nextProjectileId++;
        if (ret == 0)
            ret = _nextProjectileId++;
        return ret;
    }
}