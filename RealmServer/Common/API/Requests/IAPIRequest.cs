#region

using Newtonsoft.Json;
using System.Net.Http;

#endregion

namespace Common.API.Requests;

/// <summary>
///     Interface for an API Request.
/// </summary>
public interface IAPIRequest
{
    /// <summary>
    ///     Gets the uri for this request.
    /// </summary>
    [JsonIgnore]
    string Uri { get; }

    /// <summary>
    ///     Gets the method for this request.
    /// </summary>
    [JsonIgnore]
    HttpMethod Method { get; }

    /// <summary>
    ///     Gets the json for this request.
    /// </summary>
    string Json => JsonConvert.SerializeObject(this);
}