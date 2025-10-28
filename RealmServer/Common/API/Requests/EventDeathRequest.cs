#region

using Common.Enums;
using System.Net.Http;
using System.Text.Json.Serialization;

#endregion

namespace Common.API.Requests
{
    /// <summary>
    /// Request sent to the API to trigger an event death event.
    /// </summary>
    public class EventDeathRequest : IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Events/EventDeath";

        /// <inheritdoc/>
        [JsonIgnore]
        public HttpMethod Method => HttpMethod.Put;

        /// <summary>
        /// Gets or sets the name of the event dying.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the event dying.
        /// </summary>
        public SubscribableEvent Event { get; set; }

        /// <summary>
        /// Initialzes a new instance of the <see cref="EventDeathRequest"/> class.
        /// </summary>
        public EventDeathRequest(string eventName, SubscribableEvent subscribableEvent)
        {
            EventName = eventName;
            Event = subscribableEvent;
        }

        /// <summary>
        /// Initialzes a new instance of the <see cref="EventDeathRequest"/> class.
        /// Needed for deserialization.
        /// /// </summary>
        public EventDeathRequest()
        { }
    }
}