namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct CreateSuccess(int ObjectId, int CharId) : IOutgoingPacket
{
    public PacketId ID => PacketId.CREATESUCCESS;
}