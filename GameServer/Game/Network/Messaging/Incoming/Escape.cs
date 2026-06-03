using Common.Network;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Worlds;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.ESCAPE)]
public record Escape : IIncomingPacket {
    public async Task Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        if (user.GameInfo.World.Id == World.NEXUS_ID) {
            user.SendInfo("You're already in the Nexus!");
            return;
        }

        user.ReconnectTo(RealmManager.Worlds[World.NEXUS_ID]);
    }

    public void Read(ref SpanReader rdr) { }
}