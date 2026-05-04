#region

using Common.Network;
using Common.Structs;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.MOVE)]
public record Move : IIncomingPacket {
    public WorldPosData Pos;

    public async Task Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        ref var player = ref user.GameInfo.Player;
        player.Move(Pos.X, Pos.Y);
    }

    public void Read(ref SpanReader rdr) {
        Pos = WorldPosData.Read(ref rdr);
    }
}