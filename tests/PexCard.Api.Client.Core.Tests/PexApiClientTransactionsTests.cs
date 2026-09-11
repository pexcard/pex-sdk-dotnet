using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;
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

        [Fact]
        public async Task GetBusinessTransactions_UsesExpectedPathAndQuery()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkPaginated();
            });
            var client = CreateClient(handler);

            var query = new BusinessTransactionsQueryModel
            {
                MinDate = new DateTime(2026, 1, 1, 8, 30, 0),
                MinAmount = 10.5m,
                PageIndex = 2,
                PageSize = 25,
                SortBy = TransactionQuerySortBy.Amount,
                OrderBy = TransactionQuerySortOrder.Asc,
                OnlyCategoryIds = new List<int> { 3, 7 }
            };

            await client.GetBusinessTransactions(Token, query);

            Assert.Equal("/V4/Transactions/Business/Transactions", capturedUri.AbsolutePath);
            Assert.Contains("MinDate=2026-01-01T08%3a30%3a00", capturedUri.Query);
            Assert.Contains("MinAmount=10.5", capturedUri.Query);
            Assert.Contains("SortBy=Amount", capturedUri.Query);
            Assert.Contains("OrderBy=Asc", capturedUri.Query);
            Assert.Contains("PageIndex=2", capturedUri.Query);
            Assert.Contains("PageSize=25", capturedUri.Query);
            Assert.Contains("OnlyCategoryIds=3&OnlyCategoryIds=7", capturedUri.Query);
        }

        [Fact]
        public async Task GetBusinessTransactions_WithoutQuery_SendsNoQueryParams()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkPaginated();
            });
            var client = CreateClient(handler);

            await client.GetBusinessTransactions(Token);

            Assert.Equal("/V4/Transactions/Business/Transactions", capturedUri.AbsolutePath);
            Assert.Equal(string.Empty, capturedUri.Query);
        }

        [Fact]
        public async Task GetCardholderPurchases_ForCardholder_UsesExpectedPathAndQuery()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkPaginated();
            });
            var client = CreateClient(handler);

            var query = new CardholderPurchasesQueryModel
            {
                Pending = true,
                Search = "coffee",
                Approval = new List<TransactionApprovalStatus> { TransactionApprovalStatus.Approved, TransactionApprovalStatus.NoReceipt }
            };

            await client.GetCardholderPurchases(Token, 42, query);

            Assert.Equal("/V4/Transactions/Cardholder/42/Purchases", capturedUri.AbsolutePath);
            Assert.Contains("Pending=True", capturedUri.Query);
            Assert.Contains("Search=coffee", capturedUri.Query);
            Assert.Contains("Approval=Approved&Approval=NoReceipt", capturedUri.Query);
        }

        [Fact]
        public async Task GetCardholderDeclines_UsesExpectedPath()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkPaginated();
            });
            var client = CreateClient(handler);

            await client.GetCardholderDeclines(Token, new CardholderDeclinesQueryModel { Search = "fuel" });

            Assert.Equal("/V4/Transactions/Cardholder/Declines", capturedUri.AbsolutePath);
            Assert.Contains("Search=fuel", capturedUri.Query);
        }

        [Fact]
        public async Task GetCardholderTransaction_ForCardholder_UsesExpectedPath()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkTransaction();
            });
            var client = CreateClient(handler);

            await client.GetCardholderTransaction(Token, 42, 987654321L);

            Assert.Equal("/V4/Transactions/Cardholder/42/Transactions/987654321", capturedUri.AbsolutePath);
        }

        [Fact]
        public async Task GetBusinessTransactions_DeserializesResultsAndPagination()
        {
            var handler = new StubHandler(_ => OkPaginated());
            var client = CreateClient(handler);

            var result = await client.GetBusinessTransactions(Token);

            var transaction = Assert.Single(result.Results);
            Assert.Equal(5551234L, transaction.TransactionId);
            Assert.Equal(-25.75m, transaction.Amount);
            Assert.Equal(TransactionAccountType.Cardholder, transaction.Account.Type);
            Assert.Equal(2, result.Pagination.TotalCount);
            Assert.Equal(1, result.Pagination.ResultsCount);
        }

        [Fact]
        public async Task GetBusinessTransaction_ReturnsNull_WhenNotFound()
        {
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(string.Empty) });
            var client = CreateClient(handler);

            var result = await client.GetBusinessTransaction(Token, 123L);

            Assert.Null(result);
        }

        private static PexApiClient CreateClient(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://coreapi.example/") };
            return new PexApiClient(httpClient);
        }

        private static HttpResponseMessage Ok()
            => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"TransactionList\":[]}") };

        private const string TransactionJson = "{\"BusinessId\":100,\"CardholderId\":42,\"TransactionId\":5551234,\"TransactionTime\":\"2026-01-15T12:00:00\",\"IsPending\":false,\"Description\":\"Coffee Shop\",\"Amount\":-25.75,\"Account\":{\"Type\":\"Cardholder\",\"Id\":42}}";

        private static HttpResponseMessage OkPaginated()
            => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Results\":[" + TransactionJson + "],\"Pagination\":{\"SortBy\":\"TransactionTime\",\"OrderBy\":\"Desc\",\"PageIndex\":1,\"PageSize\":100,\"ResultsCount\":1,\"TotalCount\":2}}")
            };

        private static HttpResponseMessage OkTransaction()
            => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(TransactionJson) };

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(_responder(request));
        }
    }
}
