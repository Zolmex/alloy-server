#region

using Common;
using Common.Network;
using Common.ProjectilePaths;
using System.IO;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct EnemyShoot(
    ushort BulletId,
    int OwnerId,
    ushort ProjType,
    float X,
    float Y,
    float Angle,
    short Damage,
    byte NumShots,
    float AngleInc,
    ProjectilePath Path,
    float Lifetime,
    bool MultiHit,
    bool PassesCover,
    bool ArmorPiercing,
    int Size,
    (ConditionEffectIndex, int)[] Effects,
    int PropId = -1) : IOutgoingPacket
{
    public PacketId ID => PacketId.ENEMYSHOOT;
    
    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(BulletId);
        wtr.Write(OwnerId);
        wtr.Write(X);
        wtr.Write(Y);
        wtr.Write(Angle);
        wtr.Write(Damage);
        wtr.Write(PropId); // -1 means we're sending custom properties, otherwise use xml properties
        if (PropId == -1)
        {
            // Write custom projectile properties
            wtr.Write(ProjType);
            Path.Write(ref wtr);
            wtr.Write(Lifetime);
            wtr.Write(MultiHit);
            wtr.Write(PassesCover);
            wtr.Write(ArmorPiercing);
            wtr.Write(Size);
            wtr.Write((ushort)(Effects?.Length ?? 0));
            if (Effects != null)
                foreach (var eff in Effects)
                {
                    wtr.Write((ushort)eff.Item1);
                }
        }

        if (NumShots > 1)
        {
            wtr.Write(NumShots);
            wtr.Write(AngleInc);
        }
    }

    public static EnemyShoot Read(NetworkReader rdr)
    {
        return new EnemyShoot();
    }
}