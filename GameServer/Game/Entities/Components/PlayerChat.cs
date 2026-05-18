using System;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct PlayerChat : IEntityIdentifiable, IDisposable {
    private const int TextCooldown = 500;
    
    public EntityId Id { get; set; }

    private readonly World _world;
    private readonly EntityId _playerId;

    private long _lastMessageSent;
    
    public PlayerChat(World world, ref Entity en) {
        Id = en.Id;
        _world = world;
        _playerId = en.Id;
    }

    public bool ValidateSpeak(RealmTime time, string text) {
        var user = _world.PlayerToUser[_playerId];
        if (user.GameInfo.Account.IsAdmin)
            return true;

        // If desired, word filter goes here

        if (time.TotalElapsedMs - _lastMessageSent < TextCooldown)
            return false;

        _lastMessageSent = time.TotalElapsedMs;
        return true;
    }
    
    public void Dispose() {
        
    }
}