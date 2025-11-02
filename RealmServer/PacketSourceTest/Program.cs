using GameServer.Game.Network.Messaging.Outgoing;

namespace PacketSourceTest;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var test = new Text("a", 0, 1, 2, "nekoT", "This is a test");
        Console.WriteLine(test);
    }
}
