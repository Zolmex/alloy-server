#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct CreateSuccess(int ObjectId, int CharId) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.CREATESUCCESS;
}