#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.INVITEDTOGUILD)]
    public class InvitedToGuild : IOutgoingPacket
    {
        public string PlayerName { get; }
        public string GuildName { get; }

        public static void Write(NetworkHandler network, string playerName, string guildName)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.WriteUTF(playerName);
                wtr.WriteUTF(guildName);

                state.PacketEnd(begin, PacketId.INVITEDTOGUILD);
            }
        }

        public override string ToString()
        {
            var type = typeof(InvitedToGuild);
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