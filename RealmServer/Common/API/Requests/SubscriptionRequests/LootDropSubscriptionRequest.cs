#region

using Common.API.Models.SubscriptionModels;
using Common.Enums;
using System.Net.Http;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Subscription model for loot drop.
    /// </summary>
    public class LootDropSubscriptionRequest : APISubscription, IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Loot/LootDropSubscribe";

        /// <inheritdoc/>
        public HttpMethod Method => HttpMethod.Post;

        /// <summary>
        /// Gets or sets the rarity to subscribe to.
        /// </summary>
        public LootDropRarity LootDropRarity { get; set; }
    }
}