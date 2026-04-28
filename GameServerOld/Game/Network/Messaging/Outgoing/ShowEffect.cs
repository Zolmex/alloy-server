#region

using Common.Network;
using Common.Structs;

#endregion

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct ShowEffect(
    byte EffectType,
    int TargetId,
    int Color,
    float EffectParam,
    WorldPosData Pos1,
    WorldPosData Pos2) : IOutgoingPacket {
    public PacketId ID => PacketId.SHOWEFFECT;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(EffectType);
        wtr.Write(TargetId);
        wtr.Write(Color);
        wtr.Write(EffectParam);
        wtr.Write(Pos1);
        wtr.Write(Pos2);
    }
}