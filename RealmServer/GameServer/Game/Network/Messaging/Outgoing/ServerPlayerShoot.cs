#region

using Common;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct ServerPlayerShoot(WorldPosData StartPos,
        float Angle, float AngleInc, int[] DamageList, float[] CritList, int ItemType = -1) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.SERVERPLAYERSHOOT;

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(StartPos);
        wtr.Write(Angle);
        wtr.Write(AngleInc);

        wtr.Write((byte)DamageList.Length);
        for (var i = 0; i < DamageList.Length; i++)
            wtr.Write(DamageList[i]);

        wtr.Write((byte)CritList.Length);
        for (var i = 0; i < CritList.Length; i++)
            wtr.Write(CritList[i]);

        wtr.Write((short)ItemType);

    }
}
