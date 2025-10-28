#region

using Common;
using Common.Database;
using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.CHANGEGUILDRANK)]
    public class ChangeGuildRank : IIncomingPacket
    {
        public string TargetName;
        public int TargetRank;

        public void Read(NetworkReader rdr)
        {
            TargetName = rdr.ReadUTF();
            TargetRank = rdr.ReadInt32();
        }

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

        public override string ToString()
        {
            var type = typeof(ChangeGuildRank);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }

            return ret;
        }
    }
}