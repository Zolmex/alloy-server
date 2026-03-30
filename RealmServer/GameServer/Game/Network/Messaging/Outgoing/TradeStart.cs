#region

using Common;
using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct TradeStart(TradeItem[] MyItems, TradeItem[] TheirItems, string Name) : IOutgoingPacket
{
    public PacketId ID => PacketId.TRADESTART;
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)MyItems.Length);
        foreach (var item in MyItems)
            item.Write(wtr);
        wtr.Write(Name);
        wtr.Write((byte)TheirItems.Length);
        foreach (var item in TheirItems)
            item.Write(wtr);
    }

    public static TradeStart Read(NetworkReader rdr)
    {
        return new TradeStart();
    }
}