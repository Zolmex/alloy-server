#region

using Common;
using Common.Network;
using Common.Utilities;
using GameServer.Game.Worlds;
using System.Collections.Generic;
using System.IO;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Update(List<WorldTile> Tiles, List<ObjectData> NewEntities, List<ObjectDropData> OldEntities, Dictionary<int, ObjectStatusData> Updates) : IOutgoingPacket
{
    public PacketId ID => PacketId.UPDATE;
    
    public void Write(ref SpanWriter wtr)
    {
        wtr.Write((short)Tiles.Count);
        for (var i = 0; i < Tiles.Count; i++)
            Tiles[i].Write(ref wtr);
        wtr.Write((short)NewEntities.Count);
        for (var i = 0; i < NewEntities.Count; i++)
            NewEntities[i].Write(ref wtr);
        wtr.Write((short)OldEntities.Count);
        for (var i = 0; i < OldEntities.Count; i++)
        {
            OldEntities[i].Write(ref wtr);
            using (TimedLock.Lock(Updates))
                Updates.Remove(OldEntities[i].ObjectId);
        }
    }

    public static Update Read(NetworkReader rdr)
    {
        return new Update();
    }
}