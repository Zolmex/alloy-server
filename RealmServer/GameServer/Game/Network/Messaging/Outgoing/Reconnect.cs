namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Reconnect(int GameId) : IOutgoingPacket<Reconnect>
{ }