namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Death(int AccountId, int CharId, string Killer) : IOutgoingPacket
{
    public PacketId ID => PacketId.DEATH;
}