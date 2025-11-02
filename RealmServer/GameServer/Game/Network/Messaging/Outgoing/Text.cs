#region

using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct Text(string Name, int ObjId, int NumStars, byte BubbleTime, string Recipent, string Txt) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.TEXT;
    public readonly void Write(NetworkWriter wtr)
    {
        wtr.WriteUTF(Name);
        wtr.Write(ObjId);
        wtr.Write(NumStars);
        wtr.Write(BubbleTime);
        wtr.WriteUTF(Recipent);
        wtr.WriteUTF(Txt);
    }
}