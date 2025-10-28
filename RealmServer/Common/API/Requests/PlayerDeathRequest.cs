#region

using System.Net.Http;
using System.Text.Json.Serialization;

#endregion

namespace Common.API.Requests
{
    /// <summary>
    /// Request sent to the API to trigger a player death event.
    /// </summary>
    public class PlayerDeathRequest : IAPIRequest
    {
        /// <inheritdoc/>
        public string Uri => "/api/Players/Death";

        /// <inheritdoc/>
        [JsonIgnore]
        public HttpMethod Method => HttpMethod.Put;

        /// <summary>
        /// Gets or sets the player name for the death.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the killer name for the death.
        /// </summary>
        public string Killer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDeathRequest"/> class.
        /// </summary>
        /// <param name="playerName">Player name.</param>
        /// <param name="killer">Killer name.</param>
        public PlayerDeathRequest(string playerName, string killer)
        {
            PlayerName = playerName;
            Killer = killer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDeathRequest"/> class.
        /// Needed for deserialization.
        /// </summary>
        public PlayerDeathRequest()
        { }
    }
}