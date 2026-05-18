using Common.Network;
using Common.Utilities.Collections;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct Text(string Name, EntityId ObjId, int NumStars, byte BubbleTime, string Recipent, string Txt)
    : IOutgoingPacket {
    public PacketId ID => PacketId.TEXT;

    public void Write(ref SpanWriter wtr) {
        wtr.WriteUTF(Name);
        wtr.Write(ObjId.Value);
        wtr.Write(NumStars);
        wtr.Write(BubbleTime);
        wtr.WriteUTF(Recipent);
        wtr.WriteUTF(Txt);
    }
}