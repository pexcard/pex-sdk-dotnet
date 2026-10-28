using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
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

        [Fact]
        public async Task GetBillPayments_IncludesPayoutDateFilters_InQuery()
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
                PayoutDateFrom = new DateTime(2026, 8, 1),
                PayoutDateTo = new DateTime(2026, 9, 30),
            };

            await client.GetBillPayments(Token, model);

            Assert.NotNull(capturedQuery);
            Assert.Contains("PayoutDateFrom=", capturedQuery);
            Assert.Contains("PayoutDateTo=", capturedQuery);
            Assert.DoesNotContain("BillDate", capturedQuery);
        }

        [Fact]
        public async Task GetBillPayments_OmitsPayoutDateFilters_WhenNotSet()
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
            Assert.DoesNotContain("PayoutDate", capturedQuery);
        }

        [Fact]
        public async Task GetBillPayments_ReadsExternalApiPageInfo()
        {
            const string body = @"{
                ""Items"": [{ ""BillId"": 1 }],
                ""PageInfo"": { ""TotalCount"": 120, ""PageNumber"": 1, ""PageSize"": 50, ""TotalPages"": 3 }
            }";
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) });
            var client = CreateClient(handler);

            var response = await client.GetBillPayments(Token, new BillPaymentListRequestModel(), 1, 50);

            Assert.Equal(1, response.PageInfo.Page);
            Assert.Equal(50, response.PageInfo.PageSize);
            Assert.Equal(120, response.PageInfo.TotalItems);
        }

        [Fact]
        public async Task GetBillPayments_ReadsThePageWhenABillHasAnUnknownPaymentTransferType()
        {
            const string body = @"{
                ""Items"": [
                    { ""BillId"": 1, ""PaymentTransferType"": ""BillPayAch"" },
                    { ""BillId"": 2, ""PaymentTransferType"": ""BillPayCheck"" },
                    { ""BillId"": 3, ""PaymentTransferType"": null }
                ],
                ""PageInfo"": { ""TotalCount"": 3, ""PageNumber"": 1, ""PageSize"": 10, ""TotalPages"": 1 }
            }";
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) });
            var client = CreateClient(handler);

            var response = await client.GetBillPayments(Token, new BillPaymentListRequestModel(), 1, 10);

            Assert.Equal(3, response.Items.Count);
            Assert.Equal(PaymentTransferType.BillPayAch, response.Items[0].PaymentTransferType);
            Assert.Null(response.Items[1].PaymentTransferType);
            Assert.Null(response.Items[2].PaymentTransferType);
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
