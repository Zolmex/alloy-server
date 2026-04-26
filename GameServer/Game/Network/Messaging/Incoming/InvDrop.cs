#region

using Common.Network;
using Common.Resources.Xml.Descriptors;
using static GameServer.Game.Entities.Inventory.EntityInventory;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.INVDROP)]
public record InvDrop : IIncomingPacket {
    public byte SlotId;

    public void Handle(User user) {
        var player = user.GameInfo.Player;
        var itemToDrop = player.Inventory.RemoveItem(SlotId); // If the slot is 254 or 255 it removes from the stack
        if (itemToDrop == null)
            return;

        user.GameInfo.World.CreateContainerAt(player.Position.X, player.Position.Y, new Item[1] { itemToDrop },
            BagType.Purple, player.User.Account.Id);
    }

    public void Read(ref SpanReader rdr) {
        SlotId = rdr.ReadByte();
    }
}