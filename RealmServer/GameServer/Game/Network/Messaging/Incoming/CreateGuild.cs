#region

using Common.Database;
using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.CREATEGUILD)]
    public class CreateGuild : IIncomingPacket
    {
        public string Name;

        public void Read(NetworkReader rdr)
        {
            Name = rdr.ReadUTF();
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var result = DbClient.CreateGuild(user.Account, Name);
            var player = user.GameInfo.Player; // Update the values for the player
            player.GuildName = user.Account.GuildName;
            player.GuildRank = user.Account.GuildRank;

            GuildResult.Write(user.Network, result == GuildResult.SUCCESS, result);
        }

        public override string ToString()
        {
            var type = typeof(CreateGuild);
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