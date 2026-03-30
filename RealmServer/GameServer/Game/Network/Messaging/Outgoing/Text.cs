namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Text(string Name, int ObjId, int NumStars, byte BubbleTime, string Recipent, string Txt) : IOutgoingPacket
{
    public PacketId ID => PacketId.TEXT;
}