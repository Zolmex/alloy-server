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
    private readonly EntityStatsManager _statsManager;
    private readonly int _playerId;

    public NewTick(Span<ObjectStatusData> statuses, EntityStatsManager statsManager, int playerId) {
        _statuses = statuses;
        _statsManager = statsManager;
        _playerId = playerId;
    }
    
    public void Write(ref SpanWriter wtr) {
        var begin = wtr.Position;
        wtr.Write((short)0); // Placeholder

        var count = 0;
        foreach (var status in _statuses) {
            ref var stats = ref _statsManager.Get(status.ObjectId);
            var mask = status.ObjectId == _playerId ? stats.PrivateMask : stats.PublicMask;
            if (stats.StatUpdates.IsEmpty())
                continue;

            status.Write(stats.Pos, ref wtr, ref mask, ref stats.StatUpdates);
            count++;
        }
        
        var end = wtr.Position;
        wtr.Position = begin;
        wtr.Write((short)count);
        wtr.Position = end;
    }
}