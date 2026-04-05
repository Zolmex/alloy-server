using Common.Network;
using System.IO;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct TradeAccepted(bool[] MyOffer, bool[] TheirOffer) : IOutgoingPacket
{
    public PacketId ID => PacketId.TRADEACCEPTED;
    
    public void Write(ref SpanWriter wtr)
    {
        wtr.Write((byte)MyOffer.Length);
        foreach (var item in MyOffer)
            wtr.Write(item);
        wtr.Write((byte)TheirOffer.Length);
        foreach (var item in TheirOffer)
            wtr.Write(item);
    }

    public static TradeAccepted Read(NetworkReader rdr)
    {
        return new TradeAccepted();
    }
}