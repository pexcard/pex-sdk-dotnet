using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using PexCard.Api.Client.Core;
using PexCard.Api.Client.Core.Exceptions;
using Xunit;

namespace PexCard.Api.Client.Core.Tests
{
    public class PexOAuthClientTests
    {
        private const string Authority = "https://oauth.example/";
        private const string ClientId = "client-123";
        private static readonly Uri RedirectUri = new Uri("https://connector.example/api/app/auth/oauth/callback");

        [Fact]
        public async Task GetAuthorizeUri_BuildsAuthorizationCodePkceUrl()
        {
            var handler = new RoutingHandler();
            var client = CreateClient(handler);

            var uri = await client.GetAuthorizeUri(ClientId, RedirectUri, "openid offline_access", "state-xyz", "challenge-abc");

            Assert.Equal("https://oauth.example/oauth21/authorize", uri.GetLeftPart(UriPartial.Path));
            var q = HttpUtility.ParseQueryString(uri.Query);
            Assert.Equal("code", q["response_type"]);
            Assert.Equal(ClientId, q["client_id"]);
            Assert.Equal(RedirectUri.ToString(), q["redirect_uri"]);
            Assert.Equal("openid offline_access", q["scope"]);
            Assert.Equal("state-xyz", q["state"]);
            Assert.Equal("challenge-abc", q["code_challenge"]);
            Assert.Equal("S256", q["code_challenge_method"]);
        }

        [Fact]
        public async Task ExchangeCode_PostsAuthorizationCodeGrant_AndParsesTokens()
        {
            var handler = new RoutingHandler();
            var client = CreateClient(handler);

            var tokens = await client.ExchangeCode(ClientId, RedirectUri, "code-abc", "verifier-xyz");

            Assert.Equal(HttpMethod.Post, handler.TokenMethod);
            Assert.Equal("https://oauth.example/oauth21/token", handler.TokenUri);
            var form = HttpUtility.ParseQueryString(handler.TokenBody);
            Assert.Equal("authorization_code", form["grant_type"]);
            Assert.Equal("code-abc", form["code"]);
            Assert.Equal("verifier-xyz", form["code_verifier"]);
            Assert.Equal(ClientId, form["client_id"]);
            Assert.Equal(RedirectUri.ToString(), form["redirect_uri"]);

            Assert.Equal("at-abc", tokens.AccessToken);
            Assert.Equal("rt-new", tokens.RefreshToken);
            Assert.Equal(600, tokens.ExpiresIn);
        }

        [Fact]
        public async Task RefreshToken_PostsRefreshTokenGrant()
        {
            var handler = new RoutingHandler();
            var client = CreateClient(handler);

            await client.RefreshToken(ClientId, "rt-old");

            var form = HttpUtility.ParseQueryString(handler.TokenBody);
            Assert.Equal("refresh_token", form["grant_type"]);
            Assert.Equal("rt-old", form["refresh_token"]);
            Assert.Equal(ClientId, form["client_id"]);
        }

        // Regression guard: token/refresh POSTs are non-idempotent (single-use code, rotating refresh
        // token). They MUST opt out of the retry policy, or a 5xx after the server rotates the refresh
        // token would replay a consumed token and revoke the whole token family.
        [Fact]
        public async Task RefreshToken_OptsOutOfRetry()
        {
            var handler = new RoutingHandler();
            var client = CreateClient(handler);

            await client.RefreshToken(ClientId, "rt-old");

            Assert.True(handler.TokenHadDontRetry, "refresh_token POST must be marked DontRetryRequest.");
        }

        [Fact]
        public async Task ExchangeCode_OptsOutOfRetry()
        {
            var handler = new RoutingHandler();
            var client = CreateClient(handler);

            await client.ExchangeCode(ClientId, RedirectUri, "code-abc", "verifier-xyz");

            Assert.True(handler.TokenHadDontRetry, "authorization_code POST must be marked DontRetryRequest.");
        }

        [Fact]
        public async Task Discovery_IsIdempotentGet_AndStaysRetryable()
        {
            var handler = new RoutingHandler();
            var client = CreateClient(handler);

            await client.GetAuthorizeUri(ClientId, RedirectUri, "openid", "s", "c");

            Assert.Equal(HttpMethod.Get, handler.DiscoveryMethod);
            Assert.False(handler.DiscoveryHadDontRetry, "discovery is idempotent and should remain retryable.");
        }

        [Fact]
        public async Task Discovery_IsCached_AcrossCalls()
        {
            var handler = new RoutingHandler();
            var client = CreateClient(handler);

            await client.GetAuthorizeUri(ClientId, RedirectUri, "openid", "s", "c");
            await client.RefreshToken(ClientId, "rt-old");

            Assert.Equal(1, handler.DiscoveryCount);
        }

        [Fact]
        public async Task PostToTokenEndpoint_Throws_OnErrorResponse()
        {
            var handler = new RoutingHandler { TokenStatus = HttpStatusCode.BadRequest, TokenResponseBody = "{\"error\":\"invalid_grant\"}" };
            var client = CreateClient(handler);

            var ex = await Assert.ThrowsAsync<PexOAuthClientException>(() => client.RefreshToken(ClientId, "rt-old"));
            Assert.Equal(HttpStatusCode.BadRequest, ex.Code);
        }

        private static IPexOAuthClient CreateClient(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(Authority) };
            return new PexOAuthClient(httpClient);
        }

        private static string DiscoveryJson => JsonConvert.SerializeObject(new
        {
            issuer = "https://oauth.example",
            authorization_endpoint = "https://oauth.example/oauth21/authorize",
            token_endpoint = "https://oauth.example/oauth21/token",
            jwks_uri = "https://oauth.example/.well-known/jwks",
        });

        private static string DefaultTokenJson => JsonConvert.SerializeObject(new
        {
            access_token = "at-abc",
            token_type = "Bearer",
            expires_in = 600,
            refresh_token = "rt-new",
            refresh_token_expires_in = 1209600,
            scope = "openid",
        });

        private sealed class RoutingHandler : HttpMessageHandler
        {
            public int DiscoveryCount;
            public HttpMethod DiscoveryMethod;
            public bool DiscoveryHadDontRetry;

            public HttpMethod TokenMethod;
            public string TokenUri;
            public string TokenBody;
            public bool TokenHadDontRetry;
            public HttpStatusCode TokenStatus = HttpStatusCode.OK;
            public string TokenResponseBody;

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri.AbsolutePath.Contains(".well-known/openid-configuration"))
                {
                    DiscoveryCount++;
                    DiscoveryMethod = request.Method;
                    DiscoveryHadDontRetry = HasDontRetry(request);
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(DiscoveryJson), RequestMessage = request };
                }

                TokenMethod = request.Method;
                TokenUri = request.RequestUri.ToString();
                TokenBody = request.Content is null ? null : await request.Content.ReadAsStringAsync();
                TokenHadDontRetry = HasDontRetry(request);
                return new HttpResponseMessage(TokenStatus) { Content = new StringContent(TokenResponseBody ?? DefaultTokenJson), RequestMessage = request };
            }

            private static bool HasDontRetry(HttpRequestMessage request)
            {
#pragma warning disable CS0618 // HttpRequestMessage.Properties is where the SDK stores the flag
                return request.Properties.TryGetValue("DontRetryRequest", out var value) && value is bool flag && flag;
#pragma warning restore CS0618
            }
        }
    }
}
