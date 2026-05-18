using Common.Network;
using Common.Utilities.Collections;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct CreateSuccess(EntityId ObjectId, int CharId) : IOutgoingPacket {
    public PacketId ID => PacketId.CREATESUCCESS;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectId.Value);
        wtr.Write(CharId);
    }
}