using Common;
using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct ServerProjectileProps(
    ushort ContainerType,
    byte ProjId,
    string ObjectId,
    float Lifetime,
    bool MultiHit,
    bool PassesCover,
    bool ArmorPiercing,
    int Size,
    (ConditionEffectIndex, int)[] Effects) : IOutgoingPacket {
    public PacketId ID => PacketId.SERVER_PROJECTILE_PROPS;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ContainerType);
        wtr.Write(ProjId);
        wtr.WriteUTF(ObjectId);
        wtr.Write(Lifetime);
        wtr.Write(MultiHit);
        wtr.Write(PassesCover);
        wtr.Write(ArmorPiercing);
        wtr.Write(Size);
        wtr.Write((ushort)(Effects?.Length ?? 0));
        if (Effects != null)
            foreach (var eff in Effects) {
                wtr.Write((ushort)eff.Item1);
                wtr.Write(eff.Item2);
            }
    }
}