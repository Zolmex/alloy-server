using Common.Network;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct TradeChanged(bool[] Offer) : IOutgoingPacket {
    public PacketId ID => PacketId.TRADECHANGED;

    public void Write(ref SpanWriter wtr) {
        wtr.Write((byte)Offer.Length);
        foreach (var item in Offer)
            wtr.Write(item);
    }

    public static TradeChanged Read(NetworkReader rdr) {
        return new TradeChanged();
    }
}