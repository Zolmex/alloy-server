using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct Notification(int ObjectId, string Txt, int Color, int Size = 24, bool IsDamage = false)
    : IOutgoingPacket {
    public PacketId ID => PacketId.NOTIFICATION;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectId);
        wtr.WriteUTF(Txt);
        wtr.Write(Color);
        wtr.Write(Size);
        wtr.Write(IsDamage);
    }
}