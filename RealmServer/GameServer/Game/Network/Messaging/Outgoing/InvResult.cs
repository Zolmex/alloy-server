#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.INVRESULT)]
    public class InvResult : IOutgoingPacket
    {
        public int Result { get; }

        public static void Write(NetworkHandler network, int result)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(result);

                state.PacketEnd(begin, PacketId.INVRESULT);
            }
        }

        public override string ToString()
        {
            var type = typeof(InvResult);
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