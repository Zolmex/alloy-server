using System.ComponentModel;
using Common.Network;
using Common.Structs;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.INVSWAP)]
public record InvSwap : IIncomingPacket {
    public SlotObjectData SlotObject1;
    public SlotObjectData SlotObject2;

    public async Task Handle(User user) {
        user.GameInfo.World.EntityInventories.EnqueueSwap(user, SlotObject1, SlotObject2);
    }

    public void Read(ref SpanReader rdr) {
        SlotObject1 = SlotObjectData.Read(ref rdr);
        SlotObject2 = SlotObjectData.Read(ref rdr);
    }
}