#region

using Common.Utilities.Net;
using GameServer.Game.Entities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct TradeDone(Player.TradeResult Result) : IOutgoingPacket
{
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Result);
    }
}