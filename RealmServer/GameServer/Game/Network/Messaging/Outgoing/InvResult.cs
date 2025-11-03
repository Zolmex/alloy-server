#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public class InvResult(int Result) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.INVRESULT;
}