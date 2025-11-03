namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Text(string Name, int ObjId, int NumStars, byte BubbleTime, string Recipent, string Txt) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.TEXT;

    /*    public void Write(NetworkWriter wtr)
        {
            throw new NotImplementedException();
        }*/
}