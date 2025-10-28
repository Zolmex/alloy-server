#region

using Common.Enums;
using System.Net.Http;
using System.Text.Json.Serialization;

#endregion

namespace Common.API.Requests
{
    /// <summary>
    /// Request sent to the API to trigger a drop loot event.
    /// </summary>
    public class DropLootRequest : IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Loot/DropLoot";

        /// <inheritdoc/>
        [JsonIgnore]
        public HttpMethod Method => HttpMethod.Put;

        /// <summary>
        /// Gets or sets the player name for the loot drop.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the item name for the loot drop.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the loot drop rarity.
        /// </summary>
        public LootDropRarity Rarity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropLootRequest"/> class.
        /// </summary>
        /// <param name="playerName">Player name.</param>
        /// <param name="itemName">Item name.</param>
        public DropLootRequest(string playerName, string itemName, LootDropRarity rarity)
        {
            PlayerName = playerName;
            ItemName = itemName;
            Rarity = rarity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropLootRequest"/> class.
        /// Needed for deserialization.
        /// </summary>
        public DropLootRequest()
        { }
    }
}