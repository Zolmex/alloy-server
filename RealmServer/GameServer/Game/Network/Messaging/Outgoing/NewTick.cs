#region

using Common;
using Common.Utilities;
using System.Collections.Generic;
using System.IO;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.NEWTICK)]
    public class NewTick : IOutgoingPacket
    {
        public ObjectStatusData[] Statuses { get; }

        public static void Write(NetworkHandler network, Dictionary<int, ObjectStatusData> statuses)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                var updateCount = 0;
                wtr.Write((short)0); // Placeholder

                using (TimedLock.Lock(statuses))
                {
                    foreach (var status in statuses.Values)
                    {
                        if (!status.Update)
                            continue;

                        updateCount++;
                        using (TimedLock.Lock(status.Stats))
                        {
                            status.Write(wtr);
                            status.Stats.Clear();
                        }
                    }
                }

                var end = wtr.BaseStream.Position;

                wtr.BaseStream.Seek(begin + 5, SeekOrigin.Begin); // Go back to beginning (+ 5 bytes cus of header) and write the actual update count
                wtr.Write((short)updateCount);
                wtr.BaseStream.Seek(end, SeekOrigin.Begin); // Go to the end of the packet body

                state.PacketEnd(begin, PacketId.NEWTICK);
            }
        }

        public override string ToString()
        {
            var type = typeof(NewTick);
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