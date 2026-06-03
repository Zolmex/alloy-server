using Common.Network;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct Reconnect(int GameId) : IOutgoingPacket {
    public PacketId ID => PacketId.RECONNECT;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(GameId);
    }
}