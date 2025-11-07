#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct StatsApplyResult(int Success) : IOutgoingPacket
{
}