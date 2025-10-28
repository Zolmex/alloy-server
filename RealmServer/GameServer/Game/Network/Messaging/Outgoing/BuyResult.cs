#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.BUYRESULT)]
    public class BuyResult : IOutgoingPacket
    {
        public const int SUCCESS = 0;
        public const int ERROR_DIALOG = 1;
        public const int ERROR_TEXT = 2;

        public int Result { get; }
        public string ResultString { get; }

        public static void Write(NetworkHandler network, int result, string resultString)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(result);
                wtr.WriteUTF(resultString);

                state.PacketEnd(begin, PacketId.BUYRESULT);
            }
        }

        public override string ToString()
        {
            var type = typeof(BuyResult);
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