using Common.Utilities.Net;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct TradeChanged(bool[] Offer) : IOutgoingPacket
{
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Offer.Length);
        foreach (var item in Offer)
            wtr.Write(item);
    }
}
