using Common.Utilities;
using GameServer.Game.Entities.Old.Extensions;
using GameServer.Game.Network;

namespace GameServer.Game.Chat;

public static class ChatManager {
    private static readonly Logger _log = new(typeof(ChatManager));

    public static void Announce(string text, bool global = false) {
        var msg = $"<ANNOUNCEMENT> {text}";

        // TODO: send global announcement to all GameServer instances
        
        RealmManager.BroadcastAll(user => {
            if (user.Session.State != GameState.Playing)
                return;
            
            user.SendInfo(msg);
        });
        _log.Debug(msg);
    }
}