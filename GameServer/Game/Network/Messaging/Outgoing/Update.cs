using System.Collections.Generic;
using Common.Network;
using Common.Resources.World;
using Common.Structs;
using GameServer.Game.Network.Messaging;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly ref struct Update : IOutgoingPacket {
    public PacketId ID => PacketId.UPDATE;

    private readonly Span<MapTileData> _tiles;
    private readonly int _tileCount;
    private readonly Span<ObjectData> _newEntities;
    private readonly int _newEntityCount;
    private readonly Span<ObjectDropData> _oldEntities;
    private readonly int _dropCount;
    
    public Update(
        Span<MapTileData> tiles, int newTileCount,
        Span<ObjectData> newEntities, int newEntityCount,
        Span<ObjectDropData> oldEntities, int dropCount) {
        _tiles = tiles;
        _tileCount = newTileCount;
        _newEntities = newEntities;
        _newEntityCount = newEntityCount;
        _oldEntities = oldEntities;
        _dropCount = dropCount;
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write((short)_tileCount);
        for (var i = 0; i < _tileCount; i++)
            _tiles[i].Write(ref wtr);
        wtr.Write((short)_newEntityCount);
        for (var i = 0; i < _newEntityCount; i++)
            _newEntities[i].Write(ref wtr);
        wtr.Write((short)_dropCount);
        for (var i = 0; i < _dropCount; i++)
            _oldEntities[i].Write(ref wtr);
    }
}