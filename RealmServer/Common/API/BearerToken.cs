#region

using Common.Resources.Config;

#endregion

namespace Common.API
{
    /// <summary>
    /// Class for handling bearer token logic.
    /// </summary>
    public static class BearerToken
    {
        /// <summary>
        /// Get the valid bearer token for restricted API calls.
        /// </summary>
        /// <returns>Valid bearer token.</returns>
        public static string Get()
        {
            return APIConfig.Config.BearerToken;
        }

        /// <summary>
        /// Validates an authorization header with our bearer token.
        /// </summary>
        /// <param name="authorization">Authorization header.</param>
        /// <returns>Whether or not the authorization header permits restricted API calls.</returns>
        public static bool ValidateAuthorizationHeader(string authorization)
        {
            var token = authorization["Bearer ".Length..].Trim();
            return Get() == token;
        }
    }
}