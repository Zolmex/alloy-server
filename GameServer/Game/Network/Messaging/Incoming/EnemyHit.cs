using Arch.Core;
using Common.Network;
using Common.Structs;
using Common.Utilities.Collections;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.ENEMYHIT)]
public record EnemyHit : IIncomingPacket {
    public ushort ProjectileId;
    public EntityId TargetId;

    public async Task Handle(User user) {
        if (user.Session.State != SessionState.Playing)
            return;

        // TODO: Validate hit
        var world = user.Session.World;
        var target = world.GetEntity(TargetId);
        if (target == Entity.Null)
            return;

        var player = user.Session.Player;
        GameLogic.Enqueue(() => {
            ref var proj = ref world.ProjectileSystem.Get(player, ProjectileId);
            if (proj.IsDead(ref GameLogic.WorldTime))
                return;
            
            world.ProjectileSystem.TryHitEntity(ref proj, player, target);
        });
    }

    public void Read(ref SpanReader rdr) {
        ProjectileId = rdr.ReadUInt16();
        TargetId = EntityId.Read(ref rdr);
    }
}