using Arch.Core;
using Common.Utilities.Collections;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Network;

public partial class User {
    public void SendInfo(string text) {
        SendPacket(new Text(
            "",
            EntityId.Null,
            -1,
            0,
            null,
            text));
    }

    public void SendParty(Entity entity, string text, string name, int stars) {
        SendPacket(new Text(
            name,
            (EntityId)entity,
            stars,
            5,
            "*Party*",
            text));
    }
        
    public void SendPartyAnnounce(string text) {
        SendPacket(new Text(
            null,
            EntityId.Null,
            -1,
            0,
            "*Party*",
            text));
    }

    public void SendError(string text) {
        SendPacket(new Text(
            "*Error*",
            EntityId.Null,
            -1,
            0,
            null,
            text));
    }

    public void SendHelp(string text) {
        SendPacket(new Text(
            "*Help*",
            EntityId.Null,
            -1,
            0,
            null,
            text));
    }

    public void SendEnemy(Entity entity, string enemyName, string text) {
        SendPacket(new Text($"#{enemyName}", (EntityId)entity, -1, 3, null, text));
    }

    public void SendEnemy(string name, string text) {
        SendPacket(new Text($"#{name}", EntityId.Null, -1, 3, null, text));
    }
}