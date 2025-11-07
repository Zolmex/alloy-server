#region

using Common;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct TradeStart(TradeItem[] MyItems, TradeItem[] TheirItems, string Name) : IOutgoingPacket
{
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)MyItems.Length);
        foreach (var item in MyItems)
            item.Write(wtr);
        wtr.WriteUTF(Name);
        wtr.Write((byte)TheirItems.Length);
        foreach (var item in TheirItems)
            item.Write(wtr);
    }
}