#region

using Common.API.Models.SubscriptionModels;
using Common.Enums;

#endregion

namespace Common.API.Requests.SubscriptionRequests
{
    /// <summary>
    /// Generic class for all event lifecycle subscription requests
    /// </summary>
    public class EventLifecycleSubscriptionRequest : APISubscription
    {
        /// <summary>
        /// Gets or sets the event lifecycle to subscribe to.
        /// </summary>
        public SubscribableEvent Event { get; set; }
    }
}