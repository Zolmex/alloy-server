#region

using System.Net.Http;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Subscription model for events spawning.
    /// </summary>
    public class EventSpawnSubscriptionRequest : EventLifecycleSubscriptionRequest, IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Events/EventSpawnSubscribe";

        /// <inheritdoc/>
        public HttpMethod Method => HttpMethod.Post;
    }
}