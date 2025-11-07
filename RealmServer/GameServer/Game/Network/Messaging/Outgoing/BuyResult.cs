#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct BuyResult(int Result, string ResultString) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.BUYRESULT;
    public const int SUCCESS = 0;
    public const int ERROR_DIALOG = 1;
    public const int ERROR_TEXT = 2;
}