using Common.Network;
using Common.Structs;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.MOVE)]
public record Move : IIncomingPacket {
    public WorldPosData Pos;

    public async Task Handle(User user) {
        if (user.Session.State != SessionState.Playing)
            return;

        ref var pos = ref user.Session.World.Ecs.Get<Position>(user.Session.Player);
        pos.Move(Pos.X, Pos.Y);
    }

    public void Read(ref SpanReader rdr) {
        Pos = WorldPosData.Read(ref rdr);
    }
}