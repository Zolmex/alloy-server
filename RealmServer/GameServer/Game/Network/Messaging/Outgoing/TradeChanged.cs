using Common.Utilities;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.TRADECHANGED)]
    public class TradeChanged : IOutgoingPacket
    {
        public static void Write(NetworkHandler network, bool[] offer)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write((byte)offer.Length);
                foreach (var item in offer)
                    wtr.Write(item);

                state.PacketEnd(begin, PacketId.TRADECHANGED);
            }
        }
    }
}