#region

using Common.Network;
using GameServer.Game.Entities.Types;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct TradeDone(Player.TradeResult Result) : IOutgoingPacket {
    public PacketId ID => PacketId.TRADEDONE;

    public void Write(ref SpanWriter wtr) {
        wtr.Write((byte)Result);
    }

    public static TradeDone Read(NetworkReader rdr) {
        return new TradeDone();
    }
}