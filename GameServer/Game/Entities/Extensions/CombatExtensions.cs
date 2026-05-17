using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Structs;
using GameServer.Game.Entities.Projectiles;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using Microsoft.VisualBasic.CompilerServices;

namespace GameServer.Game.Entities.Extensions;

public static class CombatExtensions {
    extension(World world) {
        public void EnemyShootProjectiles(WorldPosData startPos,
            int ownerId, byte propsId, float angleDeg, int damage, byte count, float angleIncDeg,
            ProjectilePath path, int lifetimeMs, bool multiHit, ref RealmTime time) {
            var firstProjId = world.SpawnProjectiles(startPos, ownerId, angleDeg, angleIncDeg, damage, count, path, lifetimeMs, multiHit, ref time);
            
            ref var enProjs = ref world.EntityProjectiles.Get(ownerId);
            enProjs.ScheduleClear(time.TotalElapsedMs + lifetimeMs);
            foreach (var plrId in world.Map.GetPlayersWithin(startPos, 20f)) {
                enProjs.AddTarget(plrId); // Cache for hit validation

                var user = world.PlayerToUser[plrId];
                user.SendPacket(new EnemyShoot(
                    firstProjId,
                    ownerId,
                    propsId,
                    startPos,
                    angleDeg,
                    damage,
                    count,
                    angleIncDeg,
                    path));
            }
        }

        public ushort SpawnProjectiles(WorldPosData startPos,
            int ownerId,
            float angleDeg,
            float angleIncDeg,
            int damage,
            byte numShots,
            ProjectilePath path,
            int lifetimeMs,
            bool multiHit,
            ref RealmTime time) {
            ushort? firstId = null;
            for (var i = 0; i < numShots; i++) {
                var proj = new Projectile(world, startPos, ownerId, ref time);
                proj.SetProps(path, angleDeg + (i * angleIncDeg), damage, lifetimeMs, multiHit);
                world.Projectiles.Add(ref proj);

                ref var ownerProjectiles = ref world.EntityProjectiles.Get(ownerId);
                var localProjId = ownerProjectiles.Add(proj.Id);
                proj.SetLocalId(localProjId);
                firstId ??= localProjId;
            }

            return firstId ?? 0;
        }
    }
}