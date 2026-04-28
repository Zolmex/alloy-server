using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Incoming;

[Packet(PacketId.TRADEREQUEST)]
public record TradeRequest : IIncomingPacket {
    public string Name;

    public void Handle(User user) {
        var player = user.GameInfo.Player;
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        player.TradeRequest(Name);
    }

    public void Read(ref SpanReader rdr) {
        Name = rdr.ReadUTF();
    }
}