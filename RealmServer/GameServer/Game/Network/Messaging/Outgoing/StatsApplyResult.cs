using Common.Network;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct StatsApplyResult(bool Success) : IOutgoingPacket
{
    public PacketId ID => PacketId.STATSAPPLYRESULT;

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(Success);
    }
}