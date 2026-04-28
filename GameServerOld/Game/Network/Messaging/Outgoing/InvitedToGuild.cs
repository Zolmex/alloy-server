using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct InvitedToGuild(string PlayerName, string GuildName) : IOutgoingPacket {
    public PacketId ID => PacketId.INVITEDTOGUILD;

    public void Write(ref SpanWriter wtr) {
        wtr.WriteUTF(PlayerName);
        wtr.WriteUTF(GuildName);
    }
}