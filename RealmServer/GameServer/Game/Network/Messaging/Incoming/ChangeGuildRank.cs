#region

using Common;
using Common.Database;
using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.CHANGEGUILDRANK)]
public partial record ChangeGuildRank : IIncomingPacket
{
    public string TargetName;
    public int TargetRank;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        var acc = user.Account;
        if (acc.GuildId == 0 || acc.GuildRank < (int)GuildRank.Officer)
            return;

        var plr = user.GameInfo.Player;
        var target = plr.World.GetPlayerByName(TargetName);
        var targetAcc = target.User.Account;
        if (targetAcc.GuildId != acc.GuildId || targetAcc.GuildRank >= acc.GuildRank)
            return;

        targetAcc.GuildRank = TargetRank;
        target.GuildRank = TargetRank;
        DbClient.Save(targetAcc);
    }

    public void Read(NetworkReader rdr)
    {
        TargetName = rdr.ReadUTF();
        TargetRank = rdr.ReadInt32();
    }
}