using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests
{
    public class PexApiClientTokenSchemeTests
    {
        [Fact]
        public async Task BusinessCall_UsesBearerScheme_WhenConfigured()
        {
            AuthenticationHeaderValue auth = null;
            var client = CreateClient(req => auth = req.Headers.Authorization, PexApiTokenScheme.Bearer);

            await client.GetBusinessDetails("jwt-token");

            Assert.NotNull(auth);
            Assert.Equal("Bearer", auth.Scheme);
            Assert.Equal("jwt-token", auth.Parameter);
        }

        [Fact]
        public async Task BusinessCall_UsesLegacyTokenScheme_ByDefault()
        {
            AuthenticationHeaderValue auth = null;
            var client = CreateClient(req => auth = req.Headers.Authorization, scheme: null);

            await client.GetBusinessDetails("ext-token");

            Assert.NotNull(auth);
            Assert.Equal("token", auth.Scheme);
            Assert.Equal("ext-token", auth.Parameter);
        }

        private static PexApiClient CreateClient(Action<HttpRequestMessage> capture, PexApiTokenScheme? scheme)
        {
            var handler = new StubHandler(req =>
            {
                capture(req);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new BusinessDetailsModel())),
                };
            });
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://coreapi.example/") };
            var options = scheme.HasValue ? Options.Create(new PexApiClientOptions { TokenScheme = scheme.Value }) : null;
            return new PexApiClient(httpClient, options: options);
        }

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(_responder(request));
        }
    }
}
