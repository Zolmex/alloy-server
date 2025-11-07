namespace TestPacketGen;

public class UnitTest1
{
    [Fact]
    public Task Test1()
    {
        var source = @"
using Common.Utilities.Net;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct Text(string Name, int ObjId, int NumStars, byte BubbleTime, string Recipent, string Txt) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.TEXT;
}";
        return TestHelper.Verify(source);
    }
}
