#region

using Common.Network;
using Common.Utilities;
using GameServer.Game.Entities;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERHIT)]
public partial record PlayerHit : IIncomingPacket
{
    private static readonly Logger _log = new(typeof(PlayerHit));

    public int OwnerId;
    public ushort ProjectileId;

    public void Handle(User user)
    {
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        if (!user.GameInfo.Player.World.Entities.TryGetValue(OwnerId, out var en) || en is not Character projOwner)
        {
            _log.Debug($"NULL PROJ OWNER {OwnerId}");
            return;
        }

        if (!projOwner.Projectiles.TryGetValue(ProjectileId, out var proj))
        {
            _log.Debug($"DEAD PROJECTILE {ProjectileId}");
            return;
        }

        proj.TryHitEntity(user.GameInfo.Player);
    }

    public void Read(NetworkReader rdr)
    {
        OwnerId = rdr.ReadInt32();
        ProjectileId = rdr.ReadUInt16();
    }
}