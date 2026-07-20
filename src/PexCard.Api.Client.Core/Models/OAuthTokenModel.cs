using Newtonsoft.Json;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// An OAuth 2.1 token endpoint response (authorization-code or refresh-token grant).
    /// </summary>
    public class OAuthTokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>Access-token lifetime in seconds.</summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Present only when the <c>offline_access</c> scope was granted. PEX rotates this on every
        /// refresh, so the caller MUST persist the value returned by each token/refresh call.
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>Refresh-token lifetime in seconds (0 when the server does not report it).</summary>
        [JsonProperty("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }
    }
}
