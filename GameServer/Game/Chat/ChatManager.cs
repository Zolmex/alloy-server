using Arch.Core;
using Common;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Old.Extensions;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Chat;

public static class ChatManager {
    private static readonly Logger _log = new(typeof(ChatManager));

    public static void Announce(string text, bool global = false) {
        var msg = $"<ANNOUNCEMENT> {text}";

        // TODO: send global announcement to all GameServer instances
        
        RealmManager.BroadcastAll(user => {
            if (user.Session.State != SessionState.Playing)
                return;
            
            user.SendInfo(msg);
        });
        _log.Debug(msg);
    }
    
    public static void ExecuteCommand(User user, string text) {
        var spaceIndex = text.IndexOf(' ');
        var command = text.Substring(0, spaceIndex == -1 ? text.Length : spaceIndex);
        var args = spaceIndex == -1 ? null : text.Substring(spaceIndex + 1);
        CommandManager.ExecuteCommand(user, command, args);
    }
    
    public static void Speak(Entity player, World world, string text) {
        ref var stats = ref world.Ecs.Get<Stats>(player);
        ref var chat = ref world.Ecs.Get<PlayerChat>(player);

        var user = world.Users[player];
        if (!chat.ValidateSpeak(user, text, ref GameLogic.WorldTime))
            return;
            
        if (text.StartsWith('/')) {
            ExecuteCommand(user, text);
            return;
        }

        world.ChatSystem.PlayerText(text);
        foreach (var otherUser in world.Users.Values) {
            otherUser.SendPacket(new Text(
                stats.GetString(StatType.Name),
                (EntityId)player,
                stats.GetInt(StatType.NumStars),
                5,
                null,
                text
            ));
        }
    }
}