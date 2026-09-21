using Common;
using Common.Network;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Old;
using GameServer.Utilities;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.INVDROP)]
public record InvDrop : IIncomingPacket {
    public byte SlotId;

    public async Task Handle(User user) {
        var world = user.Session.World;
        GameLogic.Enqueue(() => {
            ref var playerInv = ref world.Ecs.Get<Inventory>(user.Session.Player);
            var item = playerInv[SlotId];
            if (item == null)
                return;

            var bag = world.EnterWorld(XmlLibrary.ObjectDescs[InventoryUtils.GetBagIdFromType(BagType.Pink)]);
            ref var bagInv = ref world.Ecs.Get<Inventory>(bag);
            bagInv.SetItem(0, item);
            
            playerInv.SetItem(SlotId, null);
        });
    }

    public void Read(ref SpanReader rdr) {
        SlotId = rdr.ReadByte();
    }
}