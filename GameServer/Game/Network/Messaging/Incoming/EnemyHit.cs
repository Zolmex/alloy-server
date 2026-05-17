using Common.Network;
using Common.Structs;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.ENEMYHIT)]
public record EnemyHit : IIncomingPacket {
    public ushort ProjectileId;
    public int TargetId;

    public async Task Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        // TODO: Validate hit
        ref var targetCombat = ref user.GameInfo.World.EntityCombat.Get(TargetId);
        if (targetCombat.Id == 0)
            return;

        var plrId = user.GameInfo.PlayerId;
        user.GameInfo.World.Enqueue(w => {
            ref var enProjs = ref w.EntityProjectiles.Get(plrId);
            if (enProjs.Id == 0)
                return;
            
            ref var proj = ref w.Projectiles.Get(enProjs.GetGlobalId(ProjectileId));
            if (proj.Id == 0)
                return;
            
            proj.TryHitEntity(TargetId);
        });
    }

    public void Read(ref SpanReader rdr) {
        ProjectileId = rdr.ReadUInt16();
        TargetId = rdr.ReadInt32();
    }
}