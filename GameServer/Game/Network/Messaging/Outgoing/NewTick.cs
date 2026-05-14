using System.Collections.Generic;
using Common.Network;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network.Messaging;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly struct NewTick : IOutgoingPacket {
    public PacketId ID => PacketId.NEWTICK;

    private readonly PooledList<ObjectStatusData> _statuses;

    public NewTick(PooledList<ObjectStatusData> statuses) {
        _statuses = statuses;
    }
    
    public void Write(ref SpanWriter wtr) {
        var span = _statuses.AsSpan();
        wtr.Write((short)span.Length);
        for (int i = 0; i < span.Length; i++)
        {
            ref readonly var status = ref span[i];
            status.WriteForNewTick(ref wtr);
        }
    }
}