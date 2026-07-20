using Newtonsoft.Json;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// The subset of the OAuth 2.1 / OIDC authorization-server metadata
    /// (RFC 8414 / OpenID discovery) that the PEX OAuth client consumes.
    /// </summary>
    public class OAuthDiscoveryModel
    {
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [JsonProperty("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonProperty("userinfo_endpoint")]
        public string UserInfoEndpoint { get; set; }

        [JsonProperty("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonProperty("revocation_endpoint")]
        public string RevocationEndpoint { get; set; }
    }
}
