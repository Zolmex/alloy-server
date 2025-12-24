namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Notification(int ObjectId, string Txt, int Color, int Size = 24, bool IsDamage = false) : IOutgoingPacket<Notification>
{ }