#region

using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.BUY)]
public class Buy : IIncomingPacket
{
    public int ObjectId;

    public void Read(NetworkReader rdr)
    {
        ObjectId = rdr.ReadInt32();
    }

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        if (!user.GameInfo.Player.World.Entities.TryGetValue(ObjectId, out var en) || en is not SellableObject merch)
            return;

        var result = merch.Purchase(user.GameInfo.Player);
        user.SendPacket(new BuyResult(result == SellableObject.SUCCESS ? BuyResult.SUCCESS : BuyResult.ERROR_DIALOG, result));
    }

    public override string ToString()
    {
        var type = typeof(Buy);
        var props = type.GetProperties();
        var ret = $"\n";
        foreach (var prop in props)
        {
            ret += $"{prop.Name}:{prop.GetValue(this)}";
            if (!(props.IndexOf(prop) == props.Length - 1))
                ret += "\n";
        }

        return ret;
    }
}