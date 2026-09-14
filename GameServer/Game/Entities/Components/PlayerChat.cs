using Arch.Core;
using Common.Game;
using GameServer.Game.Network;

namespace GameServer.Game.Entities.Components;

public struct PlayerChat {
    private const int TextCooldown = 500;
    
    private long _lastMessageSent;
    
    public bool ValidateSpeak(User user, string text, ref RealmTime time) {
        if (user.Session.Account.IsAdmin)
            return true;

        // If desired, word filter goes here

        if (time.TotalElapsedMs - _lastMessageSent < TextCooldown)
            return false;

        _lastMessageSent = time.TotalElapsedMs;
        return true;
    }
}