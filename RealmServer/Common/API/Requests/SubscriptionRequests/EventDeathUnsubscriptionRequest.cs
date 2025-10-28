#region

using System.Net.Http;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Unsubscription model for events dying.
    /// </summary>
    public class EventDeathUnsubscriptionRequest : EventLifecycleSubscriptionRequest, IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Events/EventDeathUnsubscribe";

        /// <inheritdoc/>
        public HttpMethod Method => HttpMethod.Post;
    }
}