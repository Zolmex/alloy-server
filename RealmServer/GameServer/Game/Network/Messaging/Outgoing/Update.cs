#region

using Common;
using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Worlds;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Update(List<WorldTile> Tiles, List<ObjectData> NewEntities, List<ObjectDropData> OldEntities, Dictionary<int, ObjectStatusData> Updates) : IOutgoingPacket
{
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((short)Tiles.Count);
        for (var i = 0; i < Tiles.Count; i++)
            Tiles[i].Write(wtr);
        wtr.Write((short)NewEntities.Count);
        for (var i = 0; i < NewEntities.Count; i++)
            NewEntities[i].Write(wtr);
        wtr.Write((short)OldEntities.Count);
        for (var i = 0; i < OldEntities.Count; i++)
        {
            OldEntities[i].Write(wtr);
            using (TimedLock.Lock(Updates))
                Updates.Remove(OldEntities[i].ObjectId);
        }
    }
}