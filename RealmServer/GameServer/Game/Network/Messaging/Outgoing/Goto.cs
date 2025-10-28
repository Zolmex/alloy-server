#region

using Common;
using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.GOTO)]
    public class Goto : IOutgoingPacket
    {
        public WorldPosData Pos { get; }

        public static void Write(NetworkHandler network, WorldPosData pos)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                pos.Write(wtr);

                state.PacketEnd(begin, PacketId.GOTO);
            }
        }

        public override string ToString()
        {
            var type = typeof(Goto);
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