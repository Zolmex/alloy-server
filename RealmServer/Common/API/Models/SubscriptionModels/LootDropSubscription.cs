#region

using Common.API.Requests.SubscriptionRequests;
using Common.Enums;
using System;

#endregion

namespace Common.API.Models.SubscriptionModels
{
    /// <summary>
    /// Class for storing all data required for a loot drop subscription.
    /// </summary>
    public class LootDropSubscription : APISubscription
    {
        /// <summary>
        /// Gets or sets the rarity that is subscribed to.
        /// </summary>
        public LootDropRarity Rarity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LootDropSubscription"/> class.
        /// </summary>
        /// <param name="request">HTTP Request for a loot drop subscription.</param>
        public LootDropSubscription(LootDropSubscriptionRequest request)
        {
            CallbackUrl = request.CallbackUrl;
            Rarity = request.LootDropRarity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LootDropSubscription"/> class.
        /// </summary>
        /// <param name="request">HTTP Request for a loot drop unsubscription.</param>
        public LootDropSubscription(LootDropUnsubscriptionRequest request)
        {
            CallbackUrl = request.CallbackUrl;
            Rarity = request.LootDropRarity;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (LootDropSubscription)obj;
            return CallbackUrl == other.CallbackUrl &&
                   Rarity == other.Rarity;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(CallbackUrl, Rarity);
        }
    }
}