using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PexCard.Api.Client.Core.Tests
{
    public class PexApiClientTransactionsTests
    {
        private const string Token = "ext-token";

        [Fact]
        public async Task GetAllCardholderTransactions_DefaultsIncludeVendorBillPayTrue_InQuery()
        {
            string capturedQuery = null;
            var handler = new StubHandler(req =>
            {
                capturedQuery = req.RequestUri?.Query;
                return Ok();
            });
            var client = CreateClient(handler);

            await client.GetAllCardholderTransactions(Token, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

            Assert.NotNull(capturedQuery);
            Assert.Contains("IncludeVendorBillPay=True", capturedQuery);
        }

        [Fact]
        public async Task GetAllCardholderTransactions_HonorsIncludeVendorBillPayFalse_InQuery()
        {
            string capturedQuery = null;
            var handler = new StubHandler(req =>
            {
                capturedQuery = req.RequestUri?.Query;
                return Ok();
            });
            var client = CreateClient(handler);

            await client.GetAllCardholderTransactions(Token, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31), includeVendorBillPay: false);

            Assert.NotNull(capturedQuery);
            Assert.Contains("IncludeVendorBillPay=False", capturedQuery);
        }

        private static PexApiClient CreateClient(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://coreapi.example/") };
            return new PexApiClient(httpClient);
        }

        private static HttpResponseMessage Ok()
            => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"TransactionList\":[]}") };

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(_responder(request));
        }
    }
}
