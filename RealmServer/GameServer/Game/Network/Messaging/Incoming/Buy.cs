#region

using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.BUY)]
public partial record Buy : IIncomingPacket
{
    public int ObjectId;

    //public void Read(NetworkReader rdr)
    //{
    //    ObjectId = rdr.ReadInt32();
    //}

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        if (!user.GameInfo.Player.World.Entities.TryGetValue(ObjectId, out var en) || en is not SellableObject merch)
            return;

        var result = merch.Purchase(user.GameInfo.Player);
        user.SendPacket(new BuyResult(result == SellableObject.SUCCESS ? BuyResult.SUCCESS : BuyResult.ERROR_DIALOG, result));
    }
}