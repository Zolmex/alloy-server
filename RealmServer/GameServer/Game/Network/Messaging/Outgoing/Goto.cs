#region

using Common;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Goto(WorldPosData Pos) : IOutgoingPacket<Goto>
{ }