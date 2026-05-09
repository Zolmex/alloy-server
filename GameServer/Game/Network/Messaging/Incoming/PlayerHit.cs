using Common.Network;
using Common.Utilities;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERHIT)]
public record PlayerHit : IIncomingPacket {
    private static readonly Logger _log = new(typeof(PlayerHit));

    public int OwnerId;
    public ushort ProjectileId;

    public async Task Handle(User user) {
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        ref var entityProjectiles = ref user.GameInfo.World.EntityProjectiles.Get(OwnerId);
        if (entityProjectiles.Id == 0) {
            _log.Debug($"DEAD PROJECTILE OWNER {OwnerId}");
            return;
        }

        var projId = entityProjectiles.GetGlobalId(ProjectileId);
        if (projId == 0) {
            _log.Debug($"DEAD PROJECTILE {ProjectileId}");
            return;
        }
        
        ref var proj = ref user.GameInfo.World.Projectiles.Get(projId);
        proj.TryHitEntity(user.GameInfo.PlayerId);
    }

    public void Read(ref SpanReader rdr) {
        OwnerId = rdr.ReadInt32();
        ProjectileId = rdr.ReadUInt16();
    }
}