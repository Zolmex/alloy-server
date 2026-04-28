using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct BuyResult(int Result, string ResultString) : IOutgoingPacket {
    public const int SUCCESS = 0;
    public const int ERROR_DIALOG = 1;
    public const int ERROR_TEXT = 2;
    public PacketId ID => PacketId.BUYRESULT;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(Result);
        wtr.WriteUTF(ResultString);
    }
}