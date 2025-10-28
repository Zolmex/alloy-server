#region

using Common;
using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.TRADESTART)]
    public class TradeStart : IOutgoingPacket
    {
        public static void Write(NetworkHandler network, TradeItem[] myItems, TradeItem[] theirItems, string name)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write((byte)myItems.Length);
                foreach (var item in myItems)
                    item.Write(wtr);
                wtr.WriteUTF(name);
                wtr.Write((byte)theirItems.Length);
                foreach (var item in theirItems)
                    item.Write(wtr);

                state.PacketEnd(begin, PacketId.TRADESTART);
            }
        }
    }
}