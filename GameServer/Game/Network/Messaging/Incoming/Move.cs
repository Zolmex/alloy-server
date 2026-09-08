#region

using Common.Network;
using Common.Structs;
using GameServer.Game.Entities.Old.Extensions;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.MOVE)]
public record Move : IIncomingPacket {
    public WorldPosData Pos;

    public async Task Handle(User user) {
        if (user.Session.State != GameState.Playing)
            return;

        ref var player = ref user.Session.Player;
        player.Move(user.Session.World, Pos.X, Pos.Y);
    }

    public void Read(ref SpanReader rdr) {
        Pos = WorldPosData.Read(ref rdr);
    }
}