#region

using Common.Database;
using Common.Network;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.CREATEGUILD)]
public partial record CreateGuild : IIncomingPacket
{
    public string Name;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        var result = DbClient.CreateGuild(user.Account, Name);
        var player = user.GameInfo.Player; // Update the values for the player
        player.GuildName = user.Account.GuildName;
        player.GuildRank = user.Account.GuildRank;

        user.SendPacket(new GuildResult(result == GuildResult.SUCCESS, result));
    }

    public void Read(NetworkReader rdr)
    {
        Name = rdr.ReadUTF();
    }
}