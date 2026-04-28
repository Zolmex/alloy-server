using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct InvResult(int Result) : IOutgoingPacket {
    public PacketId ID => PacketId.INVRESULT;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(Result);
    }
}