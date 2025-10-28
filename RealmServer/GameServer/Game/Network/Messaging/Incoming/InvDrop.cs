#region

using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using Common.Utilities.Net;
using static GameServer.Game.Entities.Inventory.EntityInventory;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.INVDROP)]
    public class InvDrop : IIncomingPacket
    {
        public byte SlotId;

        public void Read(NetworkReader rdr)
        {
            SlotId = rdr.ReadByte();
        }

        public void Handle(User user)
        {
            var player = user.GameInfo.Player;
            var itemToDrop = player.Inventory.RemoveItem(SlotId); // If the slot is 254 or 255 it removes from the stack
            if (itemToDrop == null)
                return;

            user.GameInfo.World.CreateContainerAt(player.Position.X, player.Position.Y, new Item[1] { itemToDrop }, BagType.Purple, player.User.Account.AccountId);
        }

        public override string ToString()
        {
            var type = typeof(InvDrop);
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
}