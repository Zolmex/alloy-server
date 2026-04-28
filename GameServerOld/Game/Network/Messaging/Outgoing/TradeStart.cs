#region

using Common.Network;
using Common.Structs;

#endregion

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct TradeStart(TradeItem[] MyItems, TradeItem[] TheirItems, string Name) : IOutgoingPacket {
    public PacketId ID => PacketId.TRADESTART;

    public void Write(ref SpanWriter wtr) {
        wtr.Write((byte)MyItems.Length);
        foreach (var item in MyItems)
            item.Write(ref wtr);
        wtr.WriteUTF(Name);
        wtr.Write((byte)TheirItems.Length);
        foreach (var item in TheirItems)
            item.Write(ref wtr);
    }

    public static TradeStart Read(NetworkReader rdr) {
        return new TradeStart();
    }
}