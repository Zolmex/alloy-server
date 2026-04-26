using Common.Network;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.GOTOACK)]
public record GotoAck : IIncomingPacket {
    public void Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        var plr = user.GameInfo.Player;
        if (!plr.Teleporting)
            return;

        plr.FinishTeleport();
    }

    public void Read(ref SpanReader rdr) { }
}