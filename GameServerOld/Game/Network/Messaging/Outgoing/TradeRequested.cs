using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct TradeRequested(string name) : IOutgoingPacket {
    public PacketId ID => PacketId.TRADEREQUESTED;

    public void Write(ref SpanWriter wtr) {
        wtr.WriteUTF(name);
    }
}