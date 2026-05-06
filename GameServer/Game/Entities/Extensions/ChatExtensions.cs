using Common;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Entities.Extensions;

public static class ChatExtensions {
    extension(User user) {
        public void SendInfo(string text) {
            user.SendPacket(new Text(
                "",
                0,
                -1,
                0,
                null,
                text));
        }

        public void SendParty(string text, ref StatsComponent speaker) {
            user.SendPacket(new Text(
                speaker.GetString(StatType.Name),
                speaker.Id,
                speaker.GetInt(StatType.NumStars),
                5,
                "*Party*",
                text));
        }
        
        public void SendPartyAnnounce(string text) {
            user.SendPacket(new Text(
                null,
                0,
                -1,
                0,
                "*Party*",
                text));
        }

        public void SendError(string text) {
            user.SendPacket(new Text(
                "*Error*",
                0,
                -1,
                0,
                null,
                text));
        }

        public void SendHelp(string text) {
            user.SendPacket(new Text(
                "*Help*",
                0,
                -1,
                0,
                null,
                text));
        }

        public void SendEnemy(Entity entity, string text) {
            user.SendPacket(new Text($"#{entity.Desc.DisplayName}", entity.Id, -1, 3, null, text));
        }

        public void SendEnemy(string name, string text) {
            user.SendPacket(new Text($"#{name}", -1, -1, 3, null, text));
        }
    }
}