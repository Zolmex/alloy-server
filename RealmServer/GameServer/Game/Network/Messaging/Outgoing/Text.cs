#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.TEXT)]
    public class Text : IOutgoingPacket
    {
        public string Name { get; }
        public int ObjectId { get; }
        public int NumStars { get; }
        public byte BubbleTime { get; }
        public string Recipent { get; }
        public string Txt { get; }

        public static void Write(NetworkHandler network, string name, int objId, int numStars, byte bubbleTime, string recipent, string text)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.WriteUTF(name);
                wtr.Write(objId);
                wtr.Write(numStars);
                wtr.Write(bubbleTime);
                wtr.WriteUTF(recipent);
                wtr.WriteUTF(text);

                state.PacketEnd(begin, PacketId.TEXT);
            }
        }

        public override string ToString()
        {
            var type = typeof(Text);
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