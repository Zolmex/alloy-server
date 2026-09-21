using Arch.Core;
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
        if (user.State != ConnectionState.Ready || user.Session.State != SessionState.Playing)
            return;

        var world = user.Session.World;
        var owner = world.GetEntity(OwnerId);
        if (owner == Entity.Null) {
            _log.Debug($"DEAD PROJECTILE OWNER {OwnerId}");
            return;
        }

        ref var proj = ref world.ProjectileSystem.Get(owner, ProjectileId);
        if (proj.IsDead(ref GameLogic.WorldTime)) {
            _log.Debug($"DEAD PROJECTILE {ProjectileId}");
            return;
        }
        
        world.ProjectileSystem.TryHitEntity(ref proj, owner, user.Session.Player);
    }

    public void Read(ref SpanReader rdr) {
        OwnerId = EntityId.Read(ref rdr);
        ProjectileId = rdr.ReadUInt16();
    }
}