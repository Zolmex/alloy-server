#region

using Common.Network;
using GameServerOld.Game.Entities.Types;

#endregion

namespace GameServerOld.Game.Network.Messaging.Incoming;

[Packet(PacketId.CANCELTRADE)]
public record CancelTrade : IIncomingPacket {
    public void Handle(User user) {
        var player = user.GameInfo.Player;
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        player.OnTradeDone(Player.TradeResult.Canceled);
    }

    public void Read(ref SpanReader rdr) { }
}