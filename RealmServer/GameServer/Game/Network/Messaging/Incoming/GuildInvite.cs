#region

using Common;
using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.GUILDINVITE)]
public partial record GuildInvite : IIncomingPacket
{
    public string TargetName;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        // var acc = user.Account; // TODO: fix
        // if (acc.GuildRank < (int)GuildRank.Officer) // Make sure we can invite members
        //     return;
        //
        // var target = user.GameInfo.Player.World.GetPlayerByName(TargetName);
        // if (target == null)
        //     return;
        //
        // if (target.GuildName != null) // Make sure we don't invite players from other guilds or from our own
        //     return;
        //
        // target.GuildInvite(user, acc.GuildName);
    }

    public void Read(NetworkReader rdr)
    {
        TargetName = rdr.ReadUTF();
    }
}