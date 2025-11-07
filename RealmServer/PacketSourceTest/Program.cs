using GameServer.Game.Network.Messaging;

namespace PacketSourceTest;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(typeof(IOutgoingPacket).AssemblyQualifiedName);
    }
}
