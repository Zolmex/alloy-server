#region

using Common.API.Requests;
using Common.Resources.Config;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Common.API.Helpers
{
    /// <summary>
    /// Class for helping with API functionality.
    /// </summary>
    public static class APIHelper
    {
        private static HttpClient _apiClient = new() { BaseAddress = baseAddress };

        private static Uri baseAddress = new($"https://{APIConfig.Config.Address}:{APIConfig.Config.Port}");

        /// <summary>
        /// Create a standard <see cref="HttpRequestMessage"/> from an <see cref="IAPIRequest"/> model without authorisation.
        /// </summary>
        /// <param name="request">Request to send.</param>
        /// <returns>Valid message to send.</returns>
        public static HttpRequestMessage CreateNonAuthMessage(IAPIRequest request)
        {
            var msg = new HttpRequestMessage { Method = request.Method, RequestUri = new Uri(baseAddress, request.Uri), Content = new StringContent(request.Json, Encoding.UTF8, "application/json") };
            return msg;
        }

        /// <summary>
        /// Create a callback <see cref="HttpRequestMessage"/> from an <see cref="IAPIRequest"/> model.
        /// </summary>
        /// <param name="callbackUri">Callback uri to use.</param>
        /// <param name="request">Request to send.</param>
        /// <returns>Valid message to send.</returns>
        public static HttpRequestMessage CreateCallbackMessage(string callbackUri, IAPIRequest request)
        {
            var msg = new HttpRequestMessage { Method = request.Method, RequestUri = new Uri(callbackUri), Content = new StringContent(request.Json, Encoding.UTF8, "application/json") };
            return msg;
        }

        /// <summary>
        /// Create a standard <see cref="HttpRequestMessage"/> from an <see cref="IAPIRequest"/> model with authorisation.
        /// </summary>
        /// <param name="request">Request to send.</param>
        /// <returns>Valid message to send.</returns>
        public static HttpRequestMessage CreateAuthMessage(IAPIRequest request)
        {
            var msg = CreateNonAuthMessage(request);
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken.Get());
            return msg;
        }

        /// <summary>
        /// Send a request with no authentication.
        /// </summary>
        /// <param name="request">Request to send.</param>
        public static async Task<HttpResponseMessage> SendRequestNoAuth(IAPIRequest request)
        {
            var msg = CreateNonAuthMessage(request);
            var task = await _apiClient.SendAsync(msg);
            return task;
        }

        /// <summary>
        /// Send a request with authentication.
        /// </summary>
        /// <param name="request">Request to send.</param>
        public static async Task<HttpResponseMessage> SendRequestAuth(IAPIRequest request)
        {
            var msg = CreateAuthMessage(request);
            var task = await _apiClient.SendAsync(msg);
            return task;
        }

        /// <summary>
        /// Send a callback request.
        /// </summary>
        /// <param name="callbackUri">Callback uri.</param>
        /// <param name="request">Request to send.</param>
        /// <returns>Response.</returns>
        public static async Task<HttpResponseMessage> SendCallbackRequest(string callbackUri, IAPIRequest request)
        {
            try
            {
                var msg = CreateCallbackMessage(callbackUri, request);
                var task = await _apiClient.SendAsync(msg);
                return task;
            }
            catch
            {
                return null;
            }
        }
    }
}