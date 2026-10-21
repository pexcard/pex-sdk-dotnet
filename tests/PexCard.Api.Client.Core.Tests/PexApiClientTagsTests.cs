using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PexCard.Api.Client.Core.Enums;
using Xunit;

namespace PexCard.Api.Client.Core.Tests
{
    public class PexApiClientTagsTests
    {
        private const string Token = "ext-token";

        [Fact]
        public async Task GetTags_WithoutFeatureSource_SendsNoQuery()
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkTags();
            }));

            var tags = await client.GetTags(Token);

            Assert.Equal("/V4/Business/Configuration/Tags", capturedUri.AbsolutePath);
            Assert.Equal(string.Empty, capturedUri.Query);
            Assert.Single(tags);
        }

        [Theory]
        [InlineData(TagFeatureSource.Transaction, "featureSource=Transaction")]
        [InlineData(TagFeatureSource.Reimbursement, "featureSource=Reimbursement")]
        [InlineData(TagFeatureSource.BillPay, "featureSource=BillPay")]
        public async Task GetTags_WithFeatureSource_SendsItByName(TagFeatureSource featureSource, string expectedQuery)
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkTags();
            }));

            var tags = await client.GetTags(Token, featureSource);

            Assert.Equal("/V4/Business/Configuration/Tags", capturedUri.AbsolutePath);
            Assert.Equal("?" + expectedQuery, capturedUri.Query);
            Assert.Equal(new[] { TagFeatureSource.Transaction, TagFeatureSource.BillPay }, Assert.Single(tags).FeatureSources);
        }

        [Theory]
        [InlineData(TagFeatureSource.Unknown)]
        [InlineData((TagFeatureSource)42)]
        public async Task GetTags_WithUndefinedFeatureSource_ThrowsBeforeSending(TagFeatureSource featureSource)
        {
            var sent = false;
            var client = CreateClient(new StubHandler(_ =>
            {
                sent = true;
                return OkTags();
            }));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.GetTags(Token, featureSource));
            Assert.False(sent);
        }

        private static PexApiClient CreateClient(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://coreapi.example/") };
            return new PexApiClient(httpClient);
        }

        private static HttpResponseMessage OkTags()
            => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[{\"Id\":\"tag\",\"Name\":\"Tag\",\"Type\":\"Decimal\",\"FeatureSources\":[\"Transaction\",\"BillPay\"]}]")
            };

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(_responder(request));
        }
    }
}
