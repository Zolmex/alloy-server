#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.NOTIFICATION)]
    public class Notification : IOutgoingPacket
    {
        public int ObjectId { get; }
        public string Txt { get; }
        public int Color { get; }
        public int Size { get; }
        public bool IsDamage { get; }

        public static void Write(NetworkHandler network, int objectId, string txt, int color, int size = 24, bool isDamage = false)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(objectId);
                wtr.WriteUTF(txt);
                wtr.Write(color);
                wtr.Write(size);
                wtr.Write(isDamage);

                state.PacketEnd(begin, PacketId.NOTIFICATION);
            }
        }

        public override string ToString()
        {
            var type = typeof(Notification);
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