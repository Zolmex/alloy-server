#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.JOINGUILD)]
    public class JoinGuild : IIncomingPacket
    {
        public string GuildName;

        public void Read(NetworkReader rdr)
        {
            GuildName = rdr.ReadUTF();
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var acc = user.Account;
            if (acc.GuildId != 0)
                return;

            var player = user.GameInfo.Player;
            player.JoinGuild(GuildName);
        }

        public override string ToString()
        {
            var type = typeof(JoinGuild);
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