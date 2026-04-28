using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Incoming;

[Packet(PacketId.ACCEPTTRADE)]
public record AcceptTrade : IIncomingPacket {
    public bool[] MyOffer;
    public bool[] TheirOffer;

    public void Handle(User user) {
        var player = user.GameInfo.Player;
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        player.AcceptTrade(MyOffer, TheirOffer);
    }

    public void Read(ref SpanReader rdr) {
        MyOffer = new bool[rdr.ReadByte()];
        for (var i = 0; i < MyOffer.Length; i++)
            MyOffer[i] = rdr.ReadBoolean();
        TheirOffer = new bool[rdr.ReadByte()];
        for (var i = 0; i < TheirOffer.Length; i++)
            TheirOffer[i] = rdr.ReadBoolean();
    }
}