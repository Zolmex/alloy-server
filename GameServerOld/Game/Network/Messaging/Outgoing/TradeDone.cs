#region

using Common.Network;
using GameServerOld.Game.Entities.Types;

#endregion

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct TradeDone(Player.TradeResult Result) : IOutgoingPacket {
    public PacketId ID => PacketId.TRADEDONE;

    public void Write(ref SpanWriter wtr) {
        wtr.Write((byte)Result);
    }

    public static TradeDone Read(NetworkReader rdr) {
        return new TradeDone();
    }
}