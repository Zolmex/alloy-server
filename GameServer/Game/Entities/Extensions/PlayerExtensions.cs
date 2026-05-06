using Common;
using Common.Database.Models;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Extensions;

public static class PlayerExtensions {
    extension(ref Entity player) {
        public void Init(World world, Account acc, Character chr) {
            ref var stats = ref world.EntityStats.Get(player.Id);
            stats.Set(StatType.Name, acc.Name);
        }
        
        public void MoveToSpawn(World world) {
            var spawnTile = world.Map.Regions[TileRegion.Spawn].RandomElement();
            player.Move(spawnTile.X, spawnTile.Y);
        }
        
        public void Speak(World world, string message) {
            ref var stats = ref world.EntityStats.Get(player.Id);
            foreach (var (otherId, otherUser) in world.PlayerToUser) {
                ref var otherPlayer = ref world.Entities.Get(otherId);
                if (otherPlayer.DistSqr(player) < PlayerSightManager.SIGHT_RADIUS_SQR)
                    otherUser.SendPacket(new Text(
                        stats.GetString(StatType.Name),
                        player.Id,
                        stats.GetInt(StatType.NumStars), 
                        5, 
                        null,
                        message
                        ));
            }
        }
    }
}