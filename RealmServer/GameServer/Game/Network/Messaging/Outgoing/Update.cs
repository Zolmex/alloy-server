#region

using Common;
using Common.Utilities;
using GameServer.Game.Worlds;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.UPDATE)]
    public class Update : IOutgoingPacket
    {
        public WorldTile[] NewTiles { get; }
        public ObjectData[] NewEntities { get; }
        public ObjectDropData[] OldEntities { get; }

        public static void Write(NetworkHandler network, List<WorldTile> tiles, List<ObjectData> newEntities, List<ObjectDropData> oldEntities, Dictionary<int, ObjectStatusData> updates)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write((short)tiles.Count);
                for (var i = 0; i < tiles.Count; i++)
                    tiles[i].Write(wtr);
                wtr.Write((short)newEntities.Count);
                for (var i = 0; i < newEntities.Count; i++)
                    newEntities[i].Write(wtr);
                wtr.Write((short)oldEntities.Count);
                for (var i = 0; i < oldEntities.Count; i++)
                {
                    oldEntities[i].Write(wtr);
                    using (TimedLock.Lock(updates))
                        updates.Remove(oldEntities[i].ObjectId);
                }

                state.PacketEnd(begin, PacketId.UPDATE);
            }
        }

        public override string ToString()
        {
            var type = typeof(Update);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }

            return ret;
        }
    }
}