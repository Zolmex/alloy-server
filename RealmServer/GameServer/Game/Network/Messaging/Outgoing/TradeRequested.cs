using Common.Utilities;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.TRADEREQUESTED)]
    public class TradeRequested : IOutgoingPacket
    {
        public static void Write(NetworkHandler network, string name)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();
                wtr.WriteUTF(name);
                state.PacketEnd(begin, PacketId.TRADEREQUESTED);
            }
        }
    }
}