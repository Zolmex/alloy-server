#region

using Common.Utilities.Net;
using GameServer.Game.Entities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct TradeDone(Player.TradeResult Result) : IOutgoingPacket<TradeDone>
{
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Result);
    }
    public static TradeDone Read(NetworkReader rdr)
    {
        return new();
    }
}