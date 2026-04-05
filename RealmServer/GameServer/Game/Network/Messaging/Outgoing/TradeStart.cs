#region

using Common;
using Common.Network;
using Common.Utilities;
using System.IO;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct TradeStart(TradeItem[] MyItems, TradeItem[] TheirItems, string Name) : IOutgoingPacket
{
    public PacketId ID => PacketId.TRADESTART;
    
    public void Write(ref SpanWriter wtr)
    {
        wtr.Write((byte)MyItems.Length);
        foreach (var item in MyItems)
            item.Write(ref wtr);
        wtr.WriteUTF(Name);
        wtr.Write((byte)TheirItems.Length);
        foreach (var item in TheirItems)
            item.Write(ref wtr);
    }

    public static TradeStart Read(NetworkReader rdr)
    {
        return new TradeStart();
    }
}