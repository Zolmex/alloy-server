#region

using Common.API.Models.SubscriptionModels;
using System.Net.Http;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Subscription model for player death.
    /// </summary>
    public class PlayerDeathSubscriptionRequest : APISubscription, IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Players/DeathSubscribe";

        /// <inheritdoc/>
        public HttpMethod Method => HttpMethod.Post;
    }
}