#region

using System;

#endregion

namespace Common.API.Models.SubscriptionModels
{
    /// <summary>
    /// Base class for all subscription models.
    /// </summary>
    public class APISubscription
    {
        /// <summary>
        /// Gets or sets the callback url for when a subscription condition is met.
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (LootDropSubscription)obj;
            return CallbackUrl == other.CallbackUrl;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(CallbackUrl);
        }
    }
}