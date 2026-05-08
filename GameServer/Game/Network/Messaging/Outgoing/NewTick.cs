using System.Collections.Generic;
using Common.Network;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network.Messaging;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly ref struct NewTick : IOutgoingPacket {
    public PacketId ID => PacketId.NEWTICK;

    private readonly Span<ObjectStatusData> _statuses;

    public NewTick(Span<ObjectStatusData> statuses) {
        _statuses = statuses;
    }
    
    public void Write(ref SpanWriter wtr) {
        wtr.Write((short)_statuses.Length);
        foreach (var status in _statuses)
            status.Write(ref wtr);
    }
}