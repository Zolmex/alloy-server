#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.DEATH)]
    public class Death : IOutgoingPacket
    {
        public int AccountId { get; }
        public int CharId { get; }
        public int Killer { get; }

        public static void Write(NetworkHandler network, int accountId, int charId, string killer)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(accountId);
                wtr.Write(charId);
                wtr.WriteUTF(killer);

                state.PacketEnd(begin, PacketId.DEATH);
            }
        }

        public override string ToString()
        {
            var type = typeof(Death);
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