#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Reconnect(int GameId) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.RECONNECT;

}