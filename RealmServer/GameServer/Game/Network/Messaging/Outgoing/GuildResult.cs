#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.GUILDRESULT)]
    public class GuildResult : IOutgoingPacket
    {
        public const string SUCCESS = "Success!";

        public bool Success { get; }
        public string ErrorText { get; }

        public static void Write(NetworkHandler network, bool success, string errorText)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(success);
                wtr.WriteUTF(errorText);

                state.PacketEnd(begin, PacketId.GUILDRESULT);
            }
        }

        public override string ToString()
        {
            var type = typeof(GuildResult);
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