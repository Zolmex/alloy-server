#region

using Common.Utilities.Net;
using GameServer.Game.Entities;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;


public readonly partial record struct DamageCounterUpdate(int TargetId, int PlayerDamage, List<KeyValuePair<Player, int>> TopDamagers) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.DAMAGECOUNTERUPDATE;
    public static void WriteClose(NetworkWriter wtr)
    {
        wtr.Write(-1);
    }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(TargetId);

        if (TargetId == -1)
        {
            return;
        }

        wtr.Write((uint)PlayerDamage);

        wtr.Write((byte)TopDamagers.Count);
        foreach (var (player, damage) in TopDamagers)
        {
            wtr.Write(player.Id);
            wtr.Write((uint)damage);
        }
    }
}