#region

using Common.Utilities.Net;
using GameServer.Game.Entities;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.CANCELTRADE)]
    public class CancelTrade : IIncomingPacket
    {
        public void Read(NetworkReader rdr)
        { }

        public void Handle(User user)
        {
            var player = user.GameInfo.Player;
            if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
                return;

            player.OnTradeDone(Player.TradeResult.Canceled);
        }
    }
}