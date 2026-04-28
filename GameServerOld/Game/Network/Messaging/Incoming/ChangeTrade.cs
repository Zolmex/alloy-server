using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Incoming;

[Packet(PacketId.CHANGETRADE)]
public record ChangeTrade : IIncomingPacket {
    public bool[] Offer;

    public void Handle(User user) {
        var player = user.GameInfo.Player;
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        player.ChangeTrade(Offer);
    }

    public void Read(ref SpanReader rdr) {
        Offer = new bool[rdr.ReadByte()];
        for (var i = 0; i < Offer.Length; i++)
            Offer[i] = rdr.ReadBoolean();
    }
}