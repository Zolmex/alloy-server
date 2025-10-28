#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.RECONNECT)]
    public class Reconnect : IOutgoingPacket
    {
        public int GameId { get; }

        public static void Write(NetworkHandler network, int gameId)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(gameId);

                state.PacketEnd(begin, PacketId.RECONNECT);
            }
        }

        public override string ToString()
        {
            var type = typeof(Reconnect);
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