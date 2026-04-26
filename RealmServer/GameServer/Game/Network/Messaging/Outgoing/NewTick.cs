#region

using System.Collections.Generic;
using Common.Network;
using Common.Structs;
using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct NewTick(Dictionary<int, ObjectStatusData> Statuses) : IOutgoingPacket {
    public PacketId ID => PacketId.NEWTICK;

    public void Write(ref SpanWriter wtr) {
        var begin = wtr.Position;

        var updateCount = 0;
        wtr.Write((short)0); // Placeholder

        foreach (var status in Statuses.Values) {
            if (!status.Update)
                continue;

            updateCount++;
            status.Write(ref wtr);
            status.Stats.Clear();
        }

        var end = wtr.Position;

        wtr.Position = begin; // Go back to beginning (+ 5 bytes cus of header) and write the actual update count
        wtr.Write((short)updateCount);
        wtr.Position = end; // Go to the end of the packet body
    }

    public static NewTick Read(NetworkReader wtr) {
        return new NewTick();
    }
}