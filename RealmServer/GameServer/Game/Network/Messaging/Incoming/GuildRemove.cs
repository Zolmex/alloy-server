#region

using Common;
using Common.Database;
using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.GUILDREMOVE)]
public partial record GuildRemove : IIncomingPacket
{
    public string TargetName;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        // var acc = user.Account; // TODO: fix
        // if (acc.GuildRank < (int)GuildRank.Officer) // Make sure we can kick members
        //     return;
        //
        // var target = user.GameInfo.Player.World.GetPlayerByName(TargetName);
        // if (target == null)
        //     return;
        //
        // if (target.GuildName != acc.GuildName || (target.Name != acc.Name && target.GuildRank >= acc.GuildRank)) // Make sure we don't kick members of the same rank or from other guilds. Except ourselves
        //     return;
        //
        // DbClientOld.RemoveFromGuild(target.AccountId, user.Account.GuildId);
        //
        // // Update the values for the player
        // target.GuildName = null;
        // target.GuildRank = 0;
        //
        // if (target.World.DisplayName == "Guild Hall") // If player is in ghall, reconnect them to nexus
        //     target.User.ReconnectTo(RealmManager.NexusInstance);
    }

    public void Read(NetworkReader rdr)
    {
        TargetName = rdr.ReadUTF();
    }
}