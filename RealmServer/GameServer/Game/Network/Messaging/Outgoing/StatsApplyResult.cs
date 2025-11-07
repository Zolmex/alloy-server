#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct StatsApplyResult(bool Success) : IOutgoingPacket
{
}