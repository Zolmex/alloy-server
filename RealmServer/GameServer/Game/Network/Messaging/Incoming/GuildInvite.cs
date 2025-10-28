#region

using Common;
using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.GUILDINVITE)]
    public class GuildInvite : IIncomingPacket
    {
        public string TargetName;

        public void Read(NetworkReader rdr)
        {
            TargetName = rdr.ReadUTF();
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var acc = user.Account;
            if (acc.GuildRank < (int)GuildRank.Officer) // Make sure we can invite members
                return;

            var target = user.GameInfo.Player.World.GetPlayerByName(TargetName);
            if (target == null)
                return;

            if (target.GuildName != null) // Make sure we don't invite players from other guilds or from our own
                return;

            target.GuildInvite(user, acc.GuildName);
        }

        public override string ToString()
        {
            var type = typeof(GuildInvite);
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