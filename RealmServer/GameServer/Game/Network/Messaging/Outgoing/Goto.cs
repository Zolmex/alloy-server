#region

using Common;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Goto(WorldPosData Pos) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.GOTO;
}