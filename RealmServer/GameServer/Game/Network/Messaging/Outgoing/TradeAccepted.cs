using Common.Utilities;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.TRADEACCEPTED)]
    public class TradeAccepted : IOutgoingPacket
    {
        public static void Write(NetworkHandler network, bool[] myOffer, bool[] theirOffer)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write((byte)myOffer.Length);
                foreach (var item in myOffer)
                    wtr.Write(item);
                wtr.Write((byte)theirOffer.Length);
                foreach (var item in theirOffer)
                    wtr.Write(item);

                state.PacketEnd(begin, PacketId.TRADEACCEPTED);
            }
        }
    }
}