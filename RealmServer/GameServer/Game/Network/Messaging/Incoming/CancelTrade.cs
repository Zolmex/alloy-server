#region

using Common.Network;
using GameServer.Game.Entities;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.CANCELTRADE)]
public partial record CancelTrade : IIncomingPacket
{
    public void Handle(User user)
    {
        var player = user.GameInfo.Player;
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        player.OnTradeDone(Player.TradeResult.Canceled);
    }

    public void Read(ref SpanReader rdr)
    { }
}