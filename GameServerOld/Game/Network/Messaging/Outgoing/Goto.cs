#region

using Common.Network;
using Common.Structs;

#endregion

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct Goto(WorldPosData Pos) : IOutgoingPacket {
    public PacketId ID => PacketId.GOTO;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(Pos);
    }
}