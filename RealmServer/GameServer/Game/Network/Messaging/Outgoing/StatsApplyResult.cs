namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct StatsApplyResult(bool Success) : IOutgoingPacket
{
    public PacketId ID => PacketId.STATSAPPLYRESULT;
}