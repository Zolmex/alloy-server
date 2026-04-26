using Common.Network;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct Death(int AccountId, int CharId, string Killer) : IOutgoingPacket {
    public PacketId ID => PacketId.DEATH;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(AccountId);
        wtr.Write(CharId);
        wtr.WriteUTF(Killer);
    }
}