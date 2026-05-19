using Common.Network;
using Common.Structs;
using Common.Utilities.Collections;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.ENEMYHIT)]
public record EnemyHit : IIncomingPacket {
    public ushort ProjectileId;
    public EntityId TargetId;

    public async Task Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        // TODO: Validate hit
        ref var targetCombat = ref user.GameInfo.World.EntityCombat.Get(TargetId);
        if (targetCombat.Id == EntityId.Null)
            return;

        var plrId = user.GameInfo.PlayerId;
        var world = user.GameInfo.World;
        GameLogic.Enqueue(() => {
            ref var enProjs = ref world.EntityProjectiles.Get(plrId);
            if (enProjs.Id == EntityId.Null)
                return;
            
            ref var proj = ref world.Projectiles.Get(enProjs.GetGlobalId(ProjectileId));
            if (proj.Id == EntityId.Null)
                return;
            
            proj.TryHitEntity(TargetId);
        });
    }

    public void Read(ref SpanReader rdr) {
        ProjectileId = rdr.ReadUInt16();
        TargetId = EntityId.Read(ref rdr);
    }
}