using Common.Network;
using Common.Projectiles.ProjectilePaths;
using Common.Structs;
using Common.Utilities.Collections;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct EnemyShoot(
    ushort FirstBulletId,
    EntityId OwnerId,
    byte ProjId,
    WorldPosData StartPos,
    float Angle,
    int Damage,
    byte NumShots,
    float AngleInc,
    ProjectilePath Path) : IOutgoingPacket {
    public PacketId ID => PacketId.ENEMYSHOOT;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(FirstBulletId);
        wtr.Write(OwnerId.Value);
        wtr.Write(ProjId);
        wtr.Write(StartPos);
        wtr.Write(Angle);
        wtr.Write(Damage);
        wtr.Write(NumShots);
        wtr.Write(AngleInc);
        Path.Write(ref wtr);
    }
}