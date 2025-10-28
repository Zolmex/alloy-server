#region

using Newtonsoft.Json;
using System.Net.Http;

#endregion

namespace Common.API.Requests
{
    /// <summary>
    /// Interface for an API Request.
    /// </summary>
    public interface IAPIRequest
    {
        /// <summary>
        /// Gets the uri for this request.
        /// </summary>
        [JsonIgnore]
        public abstract string Uri { get; }

        /// <summary>
        /// Gets the method for this request.
        /// </summary>
        [JsonIgnore]
        public abstract HttpMethod Method { get; }

        /// <summary>
        /// Gets the json for this request.
        /// </summary>
        public string Json => JsonConvert.SerializeObject(this);
    }
}