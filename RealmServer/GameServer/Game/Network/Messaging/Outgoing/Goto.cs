#region

using Common;
using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct Goto(WorldPosData Pos) : IOutgoingPacket
{
    public PacketId ID => PacketId.GOTO;

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(Pos);
    }
}