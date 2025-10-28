#region

using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.DamageSources;
using GameServer.Game.Entities;
using GameServer.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.DAMAGECOUNTERUPDATE)]
    public class DamageCounterUpdate : IOutgoingPacket
    {
        public static void WriteClose(NetworkHandler network)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();
                wtr.Write(-1);
                state.PacketEnd(begin, PacketId.DAMAGECOUNTERUPDATE);
            }
        }

        public static void Write(NetworkHandler network, int targetId, int playerDamage, List<KeyValuePair<Player, int>> topDamagers)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(targetId);

                if (targetId == -1)
                {
                    state.PacketEnd(begin, PacketId.DAMAGECOUNTERUPDATE);
                    return;
                }

                wtr.Write((uint)playerDamage);

                wtr.Write((byte)topDamagers.Count);
                foreach (var (player, damage) in topDamagers)
                {
                    wtr.Write(player.Id);
                    wtr.Write((uint)damage);
                }

                state.PacketEnd(begin, PacketId.DAMAGECOUNTERUPDATE);
            }
        }
    }
}