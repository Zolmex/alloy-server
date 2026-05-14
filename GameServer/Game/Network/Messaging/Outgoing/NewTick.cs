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
        wtr.Write((short)_statuses.Count);
        foreach (var status in _statuses)
            status.WriteForNewTick(ref wtr);
    }
}