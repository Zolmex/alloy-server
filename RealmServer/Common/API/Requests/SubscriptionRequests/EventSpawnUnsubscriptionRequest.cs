#region

using System.Net.Http;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Unsubscription model for events spawning.
    /// </summary>
    public class EventSpawnUnsubscriptionRequest : EventLifecycleSubscriptionRequest, IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Events/EventSpawnUnsubscribe";

        /// <inheritdoc/>
        public HttpMethod Method => HttpMethod.Post;
    }
}