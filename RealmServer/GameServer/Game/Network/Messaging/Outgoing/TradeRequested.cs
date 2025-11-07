namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct TradeRequested(string name) : IOutgoingPacket
{
}