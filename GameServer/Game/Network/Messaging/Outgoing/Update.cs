using System.Collections.Generic;
using Common.Network;
using Common.Resources.World;
using Common.Structs;
using GameServer.Game.Network.Messaging;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct Update(
    List<MapTileData> Tiles,
    List<ObjectData> NewEntities,
    List<ObjectDropData> OldEntities) : IOutgoingPacket {
    public PacketId ID => PacketId.UPDATE;

    public void Write(ref SpanWriter wtr) {
        wtr.Write((short)Tiles.Count);
        for (var i = 0; i < Tiles.Count; i++)
            Tiles[i].Write(ref wtr);
        wtr.Write((short)NewEntities.Count);
        for (var i = 0; i < NewEntities.Count; i++)
            NewEntities[i].Write(ref wtr);
        wtr.Write((short)OldEntities.Count);
        for (var i = 0; i < OldEntities.Count; i++)
            OldEntities[i].Write(ref wtr);
    }
}