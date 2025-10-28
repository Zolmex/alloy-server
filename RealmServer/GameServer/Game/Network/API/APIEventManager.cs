#region

using Common.API.Helpers;
using Common.API.Requests;
using Common.Enums;
using Common.StorageClasses;
using GameServer.Game.Entities;
using GameServer.Game.Worlds;

#endregion

namespace GameServer.Game.Network.API
{
    /// <summary>
    /// Class for managing all of the events that are sent out to the API.
    /// </summary>
    public class APIEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="APIEventManager"/> class
        /// </summary>
        public APIEventManager()
        {
            World.OnLootDrop += OnLootDrop;
            World.OnAddEntity += OnAddEntity;
            World.OnRemoveEntity += OnRemoveEntity;
            Player.OnDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath(Player player, string killer)
        {
            var playerDeath = new PlayerDeathRequest(player.Name, killer);
            APIHelper.SendRequestAuth(playerDeath);
        }

        private void OnRemoveEntity(Entity entity)
        {
            if (SubscribableEventMapping.IsSubscribableEvent(entity.Name))
            {
                var eventDeath = new EventDeathRequest(entity.Name, SubscribableEventMapping.NameToSubscribableEvent[entity.Name]);
                APIHelper.SendRequestAuth(eventDeath);
            }
        }

        private void OnAddEntity(Entity entity)
        {
            if (SubscribableEventMapping.IsSubscribableEvent(entity.Name))
            {
                var eventSpawn = new EventSpawnRequest(entity.Name, SubscribableEventMapping.NameToSubscribableEvent[entity.Name]);
                APIHelper.SendRequestAuth(eventSpawn);
            }
        }

        private void OnLootDrop(LootDrop drop)
        {
            var showMessage = new DropLootRequest(drop.PlayerName, drop.ItemName, drop.Rarity);
            APIHelper.SendRequestAuth(showMessage);
        }
    }
}