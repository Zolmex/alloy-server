#region

using Common.API.Models.SubscriptionModels;
using Common.Enums;
using System.Net.Http;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Unsubscription model for loot drop.
    /// </summary>
    public class LootDropUnsubscriptionRequest : APISubscription, IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Loot/LootDropUnsubscribe";

        /// <inheritdoc/>
        public HttpMethod Method => HttpMethod.Post;

        /// <summary>
        /// Gets or sets the rarity to unsubscribe from.
        /// </summary>
        public LootDropRarity LootDropRarity { get; set; }
    }
}