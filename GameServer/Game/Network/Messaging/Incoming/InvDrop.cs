using Common;
using Common.Network;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities;
using GameServer.Utilities;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.INVDROP)]
public record InvDrop : IIncomingPacket {
    public byte SlotId;

    public async Task Handle(User user) {
        user.GameInfo.World.Enqueue(w => {
            ref var playerInv = ref w.EntityInventories.Get(user.GameInfo.PlayerId);
            if (playerInv.Id == 0)
                return;

            var item = playerInv[SlotId];
            if (item == null)
                return;

            var bag = new Entity(InventoryUtils.GetBagIdFromType(BagType.Pink));
            w.EnterWorld(ref bag);
            ref var bagInv = ref w.EntityInventories.Get(bag.Id);
            bagInv.SetItem(0, item);
            
            playerInv.SetItem(SlotId, null);
        });
    }

    public void Read(ref SpanReader rdr) {
        SlotId = rdr.ReadByte();
    }
}