#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.CREATESUCCESS)]
    public class CreateSuccess : IOutgoingPacket
    {
        public int ObjectId { get; }
        public int CharId { get; }

        public static void Write(NetworkHandler network, int objectId, int charId)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(objectId);
                wtr.Write(charId);

                state.PacketEnd(begin, PacketId.CREATESUCCESS);
            }
        }

        public override string ToString()
        {
            var type = typeof(CreateSuccess);
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