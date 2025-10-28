#region

using Common.Enums;

#endregion

namespace Common.StorageClasses
{
    /// <summary>
    /// Information storage class for a loot drop.
    /// </summary>
    public class LootDrop
    {
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
        /// Initializes a new instance of the <see cref="LootDrop"/> class.
        /// </summary>
        /// <param name="player">Name of the player that got the loot.</param>
        /// <param name="item">Name of the item that was dropped.</param>
        /// <param name="rarity">Rarity of the item that was dropped.
        /// </param>
        public LootDrop(string player, string item, LootDropRarity rarity)
        {
            PlayerName = player;
            ItemName = item;
            Rarity = rarity;
        }
    }
}