#region

using System.Net.Http;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Subscription model for events dying.
    /// </summary>
    public class EventDeathSubscriptionRequest : EventLifecycleSubscriptionRequest, IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Events/EventDeathSubscribe";

        /// <inheritdoc/>
        public HttpMethod Method => HttpMethod.Post;
    }
}