using Common.Network;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct GuildResult(bool Success, string ErrorText) : IOutgoingPacket {
    public const string SUCCESS = "Success!";
    public PacketId ID => PacketId.GUILDRESULT;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(Success);
        wtr.WriteUTF(ErrorText);
    }
}