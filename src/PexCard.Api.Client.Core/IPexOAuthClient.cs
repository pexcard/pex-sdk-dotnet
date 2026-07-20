using System;
using System.Threading;
using System.Threading.Tasks;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Core
{
    /// <summary>
    /// A protocol client for the PEX OAuth 2.1 Server (the public authorization-server front door):
    /// discovery, the authorize-URL build (authorization-code + PKCE), and the token/refresh
    /// exchanges. It authenticates as a public client using PKCE (S256) and does not send a client
    /// secret.
    /// </summary>
    public interface IPexOAuthClient
    {
        /// <summary>
        /// Fetches (and caches for the client's lifetime) the authorization-server metadata from
        /// <c>{Authority}/.well-known/openid-configuration</c>.
        /// </summary>
        Task<OAuthDiscoveryModel> GetDiscovery(CancellationToken cancelToken = default);

        /// <summary>
        /// Builds the browser-facing <c>authorize</c> URL for the authorization-code + PKCE flow.
        /// The caller generates and stores <paramref name="state"/> and the PKCE verifier whose
        /// S256 <paramref name="codeChallenge"/> is passed here.
        /// </summary>
        Task<Uri> GetAuthorizeUri(string clientId, Uri redirectUri, string scopes, string state, string codeChallenge, CancellationToken cancelToken = default);

        /// <summary>
        /// Exchanges an authorization <paramref name="code"/> (with its PKCE
        /// <paramref name="codeVerifier"/>) for tokens at the token endpoint.
        /// </summary>
        Task<OAuthTokenModel> ExchangeCode(string clientId, Uri redirectUri, string code, string codeVerifier, CancellationToken cancelToken = default);

        /// <summary>
        /// Exchanges a <paramref name="refreshToken"/> for a fresh access token (and a rotated
        /// refresh token — persist the returned value). Reusing an already-rotated refresh token is
        /// treated by the server as replay and revokes the whole token family.
        /// </summary>
        Task<OAuthTokenModel> RefreshToken(string clientId, string refreshToken, CancellationToken cancelToken = default);
    }
}
