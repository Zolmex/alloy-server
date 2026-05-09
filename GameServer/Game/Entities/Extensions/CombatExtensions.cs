using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Structs;
using GameServer.Game.Entities.Projectiles;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Extensions;

public static class CombatExtensions {
    extension(World world) {
        public void ShootProjectiles(WorldPosData startPos,
            int ownerId,
            ushort containerType,
            byte propsId,
            float angle,
            int minDamage,
            int maxDamage,
            byte numShots,
            float angleInc,
            ProjectilePath path,
            ref RealmTime time) {
            var damage = Random.Shared.Next(minDamage, maxDamage);
            ushort? firstId = null;
            int lifetimeMs = 0;
            for (var i = 0; i < numShots; i++) {
                var proj = new Projectile(world, startPos, ownerId, containerType, propsId, angle, damage, path,
                    ref time);
                world.Projectiles.Add(ref proj);

                ref var ownerProjectiles = ref world.EntityProjectiles.Get(ownerId);
                var localProjId = (ushort)ownerProjectiles.Add(proj.Id);
                proj.SetLocalId(localProjId);
                firstId ??= localProjId;
                lifetimeMs = proj.Props.LifetimeMS;
            }

            ref var enProjs = ref world.EntityProjectiles.Get(ownerId);
            enProjs.ScheduleClear(time.TotalElapsedMs + lifetimeMs);
            foreach (var plrId in world.Map.GetPlayersWithin(startPos, 20f)) {
                enProjs.AddTarget(plrId); // Cache for hit validation
                
                var user = world.PlayerToUser[plrId];
                user.SendPacket(new EnemyShoot(
                    firstId ?? 0,
                    ownerId,
                    propsId,
                    startPos,
                    angle,
                    damage,
                    numShots,
                    angleInc,
                    path));
            }
        }
    }
}