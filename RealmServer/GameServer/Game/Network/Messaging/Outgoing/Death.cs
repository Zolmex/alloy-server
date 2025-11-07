#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Death(int AccountId, int CharId, string Killer) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.DEATH;
}