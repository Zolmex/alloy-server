#region

using Common.API.Models.SubscriptionModels;
using System.Net.Http;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Unsubscription model for player death.
    /// </summary>
    public class PlayerDeathUnsubscriptionRequest : APISubscription, IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Players/DeathUnsubscribe";

        /// <inheritdoc/>
        public HttpMethod Method => HttpMethod.Post;
    }
}