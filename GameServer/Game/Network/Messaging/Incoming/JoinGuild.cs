using Common.Network;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.JOINGUILD)]
public record JoinGuild : IIncomingPacket {
    public string GuildName;

    public void Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        var acc = user.Account;
        if (acc.GuildMember != null)
            return;

        var player = user.GameInfo.Player;
        player.JoinGuild(GuildName);
    }

    public void Read(ref SpanReader rdr) {
        GuildName = rdr.ReadUTF();
    }
}