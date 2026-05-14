using System.Collections.Generic;
using Common.Network;
using Common.Resources.World;
using Common.Structs;
using GameServer.Game.Network.Messaging;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly ref struct Update : IOutgoingPacket {
    public PacketId ID => PacketId.UPDATE;

    private readonly Span<MapTileData> _tiles;
    private readonly Span<ObjectData> _newEntities;
    private readonly Span<ObjectDropData> _oldEntities;
    
    public Update(
        Span<MapTileData> tiles,
        Span<ObjectData> newEntities,
        Span<ObjectDropData> oldEntities) {
        _tiles = tiles;
        _newEntities = newEntities;
        _oldEntities = oldEntities;
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write((short)_tiles.Length);
        for (var i = 0; i < _tiles.Length; i++)
            _tiles[i].Write(ref wtr);
        wtr.Write((short)_newEntities.Length);
        for (var i = 0; i < _newEntities.Length; i++)
            _newEntities[i].Write(ref wtr);
        wtr.Write((short)_oldEntities.Length);
        for (var i = 0; i < _oldEntities.Length; i++)
            _oldEntities[i].Write(ref wtr);
    }
}