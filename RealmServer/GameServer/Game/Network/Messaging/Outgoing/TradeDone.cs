#region

using Common.Utilities;
using GameServer.Game.Entities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.TRADEDONE)]
    public class TradeDone : IOutgoingPacket
    {
        public static void Write(NetworkHandler network, Player.TradeResult result)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();
                wtr.Write((byte)result);
                state.PacketEnd(begin, PacketId.TRADEDONE);
            }
        }
    }
}