#region

using Common;
using Common.Database;
using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.CHANGEGUILDRANK)]
public record ChangeGuildRank : IIncomingPacket {
    public string TargetName;
    public int TargetRank;

    public void Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        var acc = user.Account;
        if (acc.GuildMember == null || acc.GuildMember?.GuildRank < (int)GuildRank.Officer)
            return;

        var plr = user.GameInfo.Player;
        var target = plr.World.GetPlayerByName(TargetName);
        var targetAcc = target.User.Account;
        if (targetAcc.GuildMember == null || targetAcc.GuildMember.GuildId != acc.GuildMember.GuildId ||
            targetAcc.GuildMember.GuildRank >= acc.GuildMember.GuildRank)
            return;

        targetAcc.GuildMember.GuildRank = (short)TargetRank;
        target.GuildRank = TargetRank;
        _ = DbClient.FlushAsync(targetAcc);
    }

    public void Read(ref SpanReader rdr) {
        TargetName = rdr.ReadUTF();
        TargetRank = rdr.ReadInt32();
    }
}