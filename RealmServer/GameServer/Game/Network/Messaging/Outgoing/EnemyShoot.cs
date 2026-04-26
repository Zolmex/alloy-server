#region

using Common.Network;
using Common.Projectiles.ProjectilePaths;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct EnemyShoot(
    ushort FirstBulletId,
    int OwnerId,
    byte ProjId,
    float OffsetX,
    float OffsetY,
    float Angle,
    short Damage,
    byte NumShots,
    float AngleInc,
    ProjectilePath Path) : IOutgoingPacket {
    public PacketId ID => PacketId.ENEMYSHOOT;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(FirstBulletId);
        wtr.Write(OwnerId);
        wtr.Write(ProjId);
        wtr.Write(OffsetX);
        wtr.Write(OffsetY);
        wtr.Write(Angle);
        wtr.Write(Damage);
        wtr.Write(NumShots);
        wtr.Write(AngleInc);
        Path.Write(ref wtr);
    }
}