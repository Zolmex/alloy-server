using Common.Network;
using Common.Utilities;
using Common.Utilities.Collections;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERHIT)]
public record PlayerHit : IIncomingPacket {
    private static readonly Logger _log = new(typeof(PlayerHit));

    public EntityId OwnerId;
    public ushort ProjectileId;

    public async Task Handle(User user) {
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        ref var entityProjectiles = ref user.GameInfo.World.EntityProjectiles.Get(OwnerId);
        if (entityProjectiles.Id == EntityId.Null) {
            _log.Debug($"DEAD PROJECTILE OWNER {OwnerId}");
            return;
        }

        var projId = entityProjectiles.GetGlobalId(ProjectileId);
        if (projId == EntityId.Null) {
            _log.Debug($"DEAD PROJECTILE {ProjectileId}");
            return;
        }
        
        ref var proj = ref user.GameInfo.World.Projectiles.Get(projId);
        if (proj.Id == EntityId.Null)
            return;
        
        proj.TryHitEntity(user.GameInfo.PlayerId);
    }

    public void Read(ref SpanReader rdr) {
        OwnerId = EntityId.Read(ref rdr);
        ProjectileId = rdr.ReadUInt16();
    }
}