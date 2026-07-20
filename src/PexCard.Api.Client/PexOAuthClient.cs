using Microsoft.Extensions.DependencyInjection;
using PexCard.Api.Client.Core;
using PexCard.Api.Client.Core.Exceptions;
using PexCard.Api.Client.Core.Interfaces;
using PexCard.Api.Client.Core.Models;
using PexCard.Api.Client.Extensions;
using PexCard.Api.Client.Security;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PexCard.Api.Client
{
    /// <summary>
    /// Talks to the PEX OAuth 2.1 Server as a public (PKCE) client. See <see cref="IPexOAuthClient"/>.
    /// </summary>
    public class PexOAuthClient : IPexOAuthClient
    {
        private const string DiscoveryPath = ".well-known/openid-configuration";

        private readonly HttpClient _httpClient;
        private readonly ICorrelationIdResolver _correlationIdResolver;
        private readonly SemaphoreSlim _discoveryLock = new SemaphoreSlim(1, 1);

        private OAuthDiscoveryModel _discovery;

        public PexOAuthClient(HttpClient httpClient,
                              ICorrelationIdResolver correlationIdResolver = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _correlationIdResolver = correlationIdResolver ?? new DefaultCorrelationIdResolver();
        }

        public async Task<OAuthDiscoveryModel> GetDiscovery(CancellationToken cancelToken = default)
        {
            if (_discovery != null)
            {
                return _discovery;
            }

            await _discoveryLock.WaitAsync(cancelToken);
            try
            {
                if (_discovery != null)
                {
                    return _discovery;
                }

                var requestUri = new Uri(_httpClient.BaseAddress, DiscoveryPath);

                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
                request.SetPexAcceptJsonHeader();

                var response = await _httpClient.SendAsync(request, cancelToken);

                _discovery = await HandleHttpResponseMessage<OAuthDiscoveryModel>(response);

                return _discovery;
            }
            finally
            {
                _discoveryLock.Release();
            }
        }

        public async Task<Uri> GetAuthorizeUri(string clientId, Uri redirectUri, string scopes, string state, string codeChallenge, CancellationToken cancelToken = default)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));
            }
            if (redirectUri is null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentException($"'{nameof(state)}' cannot be null or empty.", nameof(state));
            }
            if (string.IsNullOrEmpty(codeChallenge))
            {
                throw new ArgumentException($"'{nameof(codeChallenge)}' cannot be null or empty.", nameof(codeChallenge));
            }

            var discovery = await GetDiscovery(cancelToken);
            if (string.IsNullOrEmpty(discovery.AuthorizationEndpoint))
            {
                throw new PexOAuthClientException(System.Net.HttpStatusCode.BadGateway, "The OAuth discovery document did not contain an authorization_endpoint.");
            }

            var builder = new UriBuilder(discovery.AuthorizationEndpoint);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["response_type"] = "code";
            query["client_id"] = clientId;
            query["redirect_uri"] = redirectUri.ToString();
            query["scope"] = scopes ?? string.Empty;
            query["state"] = state;
            query["code_challenge"] = codeChallenge;
            query["code_challenge_method"] = PexOAuthPkce.CodeChallengeMethod;
            builder.Query = query.ToString();

            return builder.Uri;
        }

        public async Task<OAuthTokenModel> ExchangeCode(string clientId, Uri redirectUri, string code, string codeVerifier, CancellationToken cancelToken = default)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));
            }
            if (redirectUri is null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
            }
            if (string.IsNullOrEmpty(codeVerifier))
            {
                throw new ArgumentException($"'{nameof(codeVerifier)}' cannot be null or empty.", nameof(codeVerifier));
            }

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = redirectUri.ToString(),
                ["client_id"] = clientId,
                ["code_verifier"] = codeVerifier,
            };

            return await PostToTokenEndpoint(form, cancelToken);
        }

        public async Task<OAuthTokenModel> RefreshToken(string clientId, string refreshToken, CancellationToken cancelToken = default)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));
            }
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException($"'{nameof(refreshToken)}' cannot be null or empty.", nameof(refreshToken));
            }

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken,
                ["client_id"] = clientId,
            };

            return await PostToTokenEndpoint(form, cancelToken);
        }

        #region private methods

        private async Task<OAuthTokenModel> PostToTokenEndpoint(IDictionary<string, string> form, CancellationToken cancelToken)
        {
            var discovery = await GetDiscovery(cancelToken);
            if (string.IsNullOrEmpty(discovery.TokenEndpoint))
            {
                throw new PexOAuthClientException(System.Net.HttpStatusCode.BadGateway, "The OAuth discovery document did not contain a token_endpoint.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, discovery.TokenEndpoint);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.Content = new FormUrlEncodedContent(form);

            // The token endpoint (authorization_code / refresh_token grant) is NON-idempotent: the
            // code and refresh token are single-use and the server rotates the refresh token. Retrying
            // after the server has already rotated it (e.g. a transient 5xx returned post-rotation)
            // replays a consumed token, which the server treats as theft and revokes the whole token
            // family — logging the user out everywhere. Opt these POSTs out of the retry policy.
            request.DontRetryRequest();

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<OAuthTokenModel>(response);
        }

        private static async Task<TData> HandleHttpResponseMessage<TData>(HttpResponseMessage response)
        {
            var content = response.Content != null ? await response.Content.ReadAsStringAsync() : null;
            var correlationId = response.GetPexCorrelationId();

            if (!response.IsSuccessStatusCode)
            {
                throw new PexOAuthClientException(response.StatusCode, content, correlationId);
            }

            return content.FromPexJson<TData>();
        }

        #endregion
    }
}
