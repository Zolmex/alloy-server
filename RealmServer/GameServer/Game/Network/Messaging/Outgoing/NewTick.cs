#region

using Common;
using Common.Utilities;
using Common.Utilities.Net;
using System.Collections.Generic;
using System.IO;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct NewTick(Dictionary<int, ObjectStatusData> Statuses) : IOutgoingPacket
{
    public void Write(NetworkWriter wtr)
    {
        var begin = wtr.BaseStream.Position;

        var updateCount = 0;
        wtr.Write((short)0); // Placeholder

        using (TimedLock.Lock(Statuses))
        {
            foreach (var status in Statuses.Values)
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

        wtr.BaseStream.Seek(begin, SeekOrigin.Begin); // Go back to beginning (+ 5 bytes cus of header) and write the actual update count
        wtr.Write((short)updateCount);
        wtr.BaseStream.Seek(end, SeekOrigin.Begin); // Go to the end of the packet body
    }
}