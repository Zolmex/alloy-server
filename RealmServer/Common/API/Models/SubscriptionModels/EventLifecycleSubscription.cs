#region

using Common.API.Requests.SubscriptionRequests;
using Common.Enums;
using System;

#endregion

namespace Common.API.Models.SubscriptionModels
{
    /// <summary>
    /// Class for storing all data required for an event lifecycle subscription.
    /// </summary>
    public class EventLifecycleSubscription : APISubscription
    {
        /// <summary>
        /// Gets or sets the event that is subscribed to.
        /// </summary>
        public SubscribableEvent Event { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLifecycleSubscription"/> class.
        /// </summary>
        /// <param name="request">HTTP Request for an event lifecycle subscription.</param>
        public EventLifecycleSubscription(EventLifecycleSubscriptionRequest request)
        {
            CallbackUrl = request.CallbackUrl;
            Event = request.Event;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (EventLifecycleSubscription)obj;
            return CallbackUrl == other.CallbackUrl &&
                   Event == other.Event;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(CallbackUrl, Event);
        }
    }
}