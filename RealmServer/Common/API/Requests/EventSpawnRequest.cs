#region

using Common.Enums;
using System.Net.Http;
using System.Text.Json.Serialization;

#endregion

namespace Common.API.Requests
{
    /// <summary>
    /// Request sent to the API to trigger an event spawn event.
    /// </summary>
    public class EventSpawnRequest : IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Events/EventSpawn";

        /// <inheritdoc/>
        [JsonIgnore]
        public HttpMethod Method => HttpMethod.Put;

        /// <summary>
        /// Gets or sets the name of the event being spawned.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the event being spawned.
        /// </summary>
        public SubscribableEvent Event { get; set; }

        /// <summary>
        /// Initialzes a new instance of the <see cref="EventSpawnRequest"/> class.
        /// </summary>
        public EventSpawnRequest(string eventName, SubscribableEvent subscribableEvent)
        {
            EventName = eventName;
            Event = subscribableEvent;
        }

        /// <summary>
        /// Initialzes a new instance of the <see cref="EventSpawnRequest"/> class.
        /// Needed for deserialization.
        /// /// </summary>
        public EventSpawnRequest()
        { }
    }
}