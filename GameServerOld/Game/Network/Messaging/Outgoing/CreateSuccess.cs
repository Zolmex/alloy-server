using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct CreateSuccess(int ObjectId, int CharId) : IOutgoingPacket {
    public PacketId ID => PacketId.CREATESUCCESS;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectId);
        wtr.Write(CharId);
    }
}