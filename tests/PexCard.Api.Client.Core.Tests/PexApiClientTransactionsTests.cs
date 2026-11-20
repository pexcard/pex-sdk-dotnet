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

        [Fact]
        public async Task GetCardholderPurchases_WithIncludeDisputes_SendsIncludeDisputesTrue()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkPaginated();
            });
            var client = CreateClient(handler);

            await client.GetCardholderPurchases(Token, new CardholderPurchasesQueryModel { IncludeDisputes = true });

            Assert.Equal("/V4/Transactions/Cardholder/Purchases", capturedUri.AbsolutePath);
            Assert.Contains("IncludeDisputes=True", capturedUri.Query);
        }

        [Fact]
        public async Task GetCardholderPurchases_ByDefault_DoesNotSendIncludeDisputes()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkPaginated();
            });
            var client = CreateClient(handler);

            await client.GetCardholderPurchases(Token, 42, new CardholderPurchasesQueryModel());

            Assert.DoesNotContain("IncludeDisputes", capturedUri.Query);
        }

        [Fact]
        public async Task GetCardholderPurchase_WithIncludeDisputes_SendsIncludeDisputesTrue()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkTransaction();
            });
            var client = CreateClient(handler);

            await client.GetCardholderPurchase(Token, 42, 987654321L, includeDisputes: true);

            Assert.Equal("/V4/Transactions/Cardholder/42/Purchases/987654321", capturedUri.AbsolutePath);
            Assert.Equal("?IncludeDisputes=True", capturedUri.Query);
        }

        [Fact]
        public async Task GetCardholderPurchase_ByDefault_SendsNoQueryParams()
        {
            Uri capturedUri = null;
            var handler = new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return OkTransaction();
            });
            var client = CreateClient(handler);

            await client.GetCardholderPurchase(Token, 987654321L);

            Assert.Equal("https://coreapi.example/V4/Transactions/Cardholder/Purchases/987654321", capturedUri.ToString());
        }

        [Fact]
        public async Task DetailsTransactionMethods_WithIncludeDisputes_SendIncludeDisputesTrue()
        {
            var capturedQueries = new List<string>();
            var handler = new StubHandler(req =>
            {
                capturedQueries.Add(req.RequestUri?.Query);
                return Ok();
            });
            var client = CreateClient(handler);
            var start = new DateTime(2026, 1, 1);
            var end = new DateTime(2026, 1, 31);

            await client.GetAllCardholderTransactions(Token, start, end, includeDisputes: true);
            await client.GetBusinessAccountTransactions(Token, start, end, includeDisputes: true);
            await client.GetCardholderTransactions(Token, 42, start, end, includeDisputes: true);

            Assert.Equal(3, capturedQueries.Count);
            Assert.All(capturedQueries, query => Assert.Contains("IncludeDisputes=True", query));
        }

        [Fact]
        public async Task DetailsTransactionMethods_ByDefault_DoNotSendIncludeDisputes()
        {
            var capturedQueries = new List<string>();
            var handler = new StubHandler(req =>
            {
                capturedQueries.Add(req.RequestUri?.Query);
                return Ok();
            });
            var client = CreateClient(handler);
            var start = new DateTime(2026, 1, 1);
            var end = new DateTime(2026, 1, 31);

            await client.GetAllCardholderTransactions(Token, start, end);
            await client.GetBusinessAccountTransactions(Token, start, end);
            await client.GetCardholderTransactions(Token, 42, start, end);

            Assert.Equal(3, capturedQueries.Count);
            Assert.All(capturedQueries, query => Assert.DoesNotContain("IncludeDisputes", query));
        }

        [Fact]
        public async Task GetCardholderPurchase_DeserializesDisputeWithCredit()
        {
            var handler = new StubHandler(_ => OkJson(TransactionJsonWith(DisputeJson)));
            var client = CreateClient(handler);

            var result = await client.GetCardholderPurchase(Token, 5551234L, includeDisputes: true);

            AssertFullDispute(result.Dispute);
        }

        [Fact]
        public async Task GetAllCardholderTransactions_DeserializesDisputeWithCredit()
        {
            var handler = new StubHandler(_ => OkJson("{\"TransactionList\":[{\"TransactionId\":5551234,\"Dispute\":" + DisputeJson + "}]}"));
            var client = CreateClient(handler);

            var result = await client.GetAllCardholderTransactions(Token, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31), includeDisputes: true);

            AssertFullDispute(Assert.Single(result).Dispute);
        }

        [Theory]
        [InlineData("")]
        [InlineData(",\"Dispute\":null")]
        public async Task GetCardholderPurchase_WithoutDispute_LeavesDisputeNull(string disputeJson)
        {
            var handler = new StubHandler(_ => OkJson(TransactionJsonWith(disputeJson, raw: true)));
            var client = CreateClient(handler);

            var result = await client.GetCardholderPurchase(Token, 5551234L);

            Assert.NotNull(result);
            Assert.Null(result.Dispute);
        }

        [Fact]
        public async Task GetAllCardholderTransactions_WithNullDispute_LeavesDisputeNull()
        {
            var handler = new StubHandler(_ => OkJson("{\"TransactionList\":[{\"TransactionId\":5551234,\"Dispute\":null},{\"TransactionId\":5551235}]}"));
            var client = CreateClient(handler);

            var result = await client.GetAllCardholderTransactions(Token, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

            Assert.All(result, transaction => Assert.Null(transaction.Dispute));
        }

        [Fact]
        public async Task GetCardholderPurchase_WithUnknownDisputeValues_KeepsThemAsStrings()
        {
            const string disputeJson = "{\"Status\":\"Escalated\",\"Decision\":\"Arbitration\",\"OpenedDate\":\"2026-02-01T10:00:00Z\",\"Credit\":{\"Type\":\"GoodwillCredit\",\"Amount\":5.00,\"Status\":\"Reversed\"}}";
            var handler = new StubHandler(_ => OkJson(TransactionJsonWith(disputeJson)));
            var client = CreateClient(handler);

            var result = await client.GetCardholderPurchase(Token, 5551234L, includeDisputes: true);

            Assert.Equal("Escalated", result.Dispute.Status);
            Assert.Equal("Arbitration", result.Dispute.Decision);
            Assert.Equal("GoodwillCredit", result.Dispute.Credit.Type);
            Assert.Equal("Reversed", result.Dispute.Credit.Status);
        }

        private const string DisputeJson = "{\"Status\":\"Closed\",\"Decision\":\"Recovered\",\"DecisionDate\":\"2026-02-10T15:30:00Z\",\"OpenedDate\":\"2026-02-01T10:00:00Z\",\"Credit\":{\"Type\":\"IsaFeeReversal\",\"Amount\":25.75,\"Status\":\"Done\",\"Date\":\"2026-02-11T09:00:00Z\"}}";

        private static string TransactionJsonWith(string dispute, bool raw = false)
            => TransactionJson.Substring(0, TransactionJson.Length - 1) + (raw ? dispute : ",\"Dispute\":" + dispute) + "}";

        private static void AssertFullDispute(DisputeModel dispute)
        {
            Assert.NotNull(dispute);
            Assert.Equal("Closed", dispute.Status);
            Assert.Equal("Recovered", dispute.Decision);
            Assert.Equal(new DateTime(2026, 2, 10, 15, 30, 0, DateTimeKind.Utc), dispute.DecisionDate?.ToUniversalTime());
            Assert.Equal(new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc), dispute.OpenedDate.ToUniversalTime());
            Assert.NotNull(dispute.Credit);
            Assert.Equal("IsaFeeReversal", dispute.Credit.Type);
            Assert.Equal(25.75m, dispute.Credit.Amount);
            Assert.Equal("Done", dispute.Credit.Status);
            Assert.Equal(new DateTime(2026, 2, 11, 9, 0, 0, DateTimeKind.Utc), dispute.Credit.Date?.ToUniversalTime());
        }

        private static HttpResponseMessage OkJson(string json)
            => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };

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
