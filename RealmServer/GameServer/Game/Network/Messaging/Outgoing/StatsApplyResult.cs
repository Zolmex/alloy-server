#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.STATSAPPLYRESULT)]
    public class StatsApplyResult : IOutgoingPacket
    {
        public int Success { get; }

        public static void Write(NetworkHandler network, bool success)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(success);

                state.PacketEnd(begin, PacketId.STATSAPPLYRESULT);
            }
        }

        public override string ToString()
        {
            var type = typeof(StatsApplyResult);
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