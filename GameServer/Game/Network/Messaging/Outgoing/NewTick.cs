using System.Collections.Generic;
using Common.Network;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network.Messaging;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly struct NewTick : IOutgoingPacket {
    public PacketId ID => PacketId.NEWTICK;

    private readonly ObjectStatusData[] _statuses;
    private readonly int _count;

    public NewTick(ObjectStatusData[] statuses, int count) {
        _statuses = statuses;
        _count = count;
    }
    
    public void Write(ref SpanWriter wtr) {
        wtr.Write((short)_count);
        for (var i = 0; i < _count; i++)
            _statuses[i].WriteForNewTick(ref wtr);
    }
}