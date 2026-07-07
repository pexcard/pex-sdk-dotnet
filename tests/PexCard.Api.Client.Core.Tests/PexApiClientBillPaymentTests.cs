using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests
{
    public class PexApiClientBillPaymentTests
    {
        private const string Token = "ext-token";

        [Fact]
        public async Task GetBillPayments_IncludesBillDateFilters_InQuery()
        {
            string capturedQuery = null;
            var handler = new StubHandler(req =>
            {
                capturedQuery = req.RequestUri?.Query;
                return Ok();
            });
            var client = CreateClient(handler);

            var model = new BillPaymentListRequestModel
            {
                BillDateFrom = new DateTime(2026, 1, 1),
                BillDateTo = new DateTime(2026, 1, 31),
            };

            await client.GetBillPayments(Token, model);

            Assert.NotNull(capturedQuery);
            Assert.Contains("BillDateFrom=", capturedQuery);
            Assert.Contains("BillDateTo=", capturedQuery);
        }

        [Fact]
        public async Task GetBillPayments_OmitsBillDateFilters_WhenNotSet()
        {
            string capturedQuery = null;
            var handler = new StubHandler(req =>
            {
                capturedQuery = req.RequestUri?.Query;
                return Ok();
            });
            var client = CreateClient(handler);

            await client.GetBillPayments(Token, new BillPaymentListRequestModel());

            Assert.NotNull(capturedQuery);
            Assert.DoesNotContain("BillDate", capturedQuery);
        }

        private static PexApiClient CreateClient(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://coreapi.example/") };
            return new PexApiClient(httpClient);
        }

        private static HttpResponseMessage Ok()
        {
            var body = JsonConvert.SerializeObject(new BillPaymentListResponseModel
            {
                Items = new List<BillPaymentModel>(),
                PageInfo = new PageInfoModel { Page = 1, PageSize = 15, TotalItems = 0 },
            });
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) };
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
