using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PexCard.Api.Client.Core.Enums;
using Xunit;

namespace PexCard.Api.Client.Core.Tests
{
    public class PexApiClientInvoiceTests
    {
        private const string Token = "ext-token";

        private const string InvoiceJson = "{\"InvoiceId\":98761,\"BusinessAccountId\":5331803,\"Type\":\"ChargeBilling\",\"InvoiceTypeName\":\"Charge Billing\",\"InvoiceDescription\":\"desc\",\"InvoiceAmount\":49.90,\"AmountTaken\":49.40,\"PendingAmount\":1.0,\"WaivedAmount\":2.0,\"RebateCreditAmount\":0.50,\"RebateCreditReversalAmount\":3.0,\"CarryOverCreditAmount\":4.0,\"WriteOffAmount\":5.0,\"WriteOffReversalAmount\":6.0,\"CCStatus\":\"Paid\",\"HasReversal\":true,\"Status\":\"ClosedUnpaid\",\"DueDate\":\"2026-07-01T00:00:00\",\"DateAssessed\":\"2026-06-01T00:00:00\",\"DateUpdated\":\"2026-07-02T00:00:00\",\"IsPastReturnWindow\":true";

        [Fact]
        public async Task GetInvoices_WithPaging_SendsPagingAndSortInQuery()
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return Ok("[]");
            }));

            await client.GetInvoices(Token, new DateTime(2026, 1, 1), 100, 2, SortDirection.Ascending);

            Assert.Equal("/V4/Invoices", capturedUri.AbsolutePath);
            Assert.Contains("startDate=", capturedUri.Query);
            Assert.Contains("pageSize=100", capturedUri.Query);
            Assert.Contains("pageNumber=2", capturedUri.Query);
            Assert.Contains("sortOrder=ASC", capturedUri.Query);
        }

        [Fact]
        public async Task GetInvoices_Descending_SendsDesc()
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return Ok("[]");
            }));

            await client.GetInvoices(Token, new DateTime(2026, 1, 1), 10, 1, SortDirection.Descending);

            Assert.Contains("sortOrder=DESC", capturedUri.Query);
        }

        [Fact]
        public async Task GetInvoices_WithoutPaging_SendsNoPagingParams()
        {
            Uri capturedUri = null;
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return Ok("[]");
            }));

            await client.GetInvoices(Token, new DateTime(2026, 1, 1));

            Assert.DoesNotContain("pageSize", capturedUri.Query);
            Assert.DoesNotContain("pageNumber", capturedUri.Query);
            Assert.DoesNotContain("sortOrder", capturedUri.Query);
        }

        [Fact]
        public async Task GetInvoices_DeserializesEverySummaryField()
        {
            var client = CreateClient(new StubHandler(_ => Ok("[" + InvoiceJson + "}]")));

            var invoice = Assert.Single(await client.GetInvoices(Token, new DateTime(2026, 1, 1), 10, 1, SortDirection.Descending));

            Assert.Equal(98761, invoice.InvoiceId);
            Assert.Equal(5331803, invoice.BusinessAccountId);
            Assert.Equal("ChargeBilling", invoice.Type);
            Assert.Equal("Charge Billing", invoice.InvoiceTypeName);
            Assert.Equal("desc", invoice.InvoiceDescription);
            Assert.Equal(49.90m, invoice.InvoiceAmount);
            Assert.Equal(49.40m, invoice.AmountTaken);
            Assert.Equal(1m, invoice.PendingAmount);
            Assert.Equal(2m, invoice.WaivedAmount);
            Assert.Equal(0.50m, invoice.RebateCreditAmount);
            Assert.Equal(3m, invoice.RebateCreditReversalAmount);
            Assert.Equal(4m, invoice.CarryOverCreditAmount);
            Assert.Equal(5m, invoice.WriteOffAmount);
            Assert.Equal(6m, invoice.WriteOffReversalAmount);
            Assert.Equal("Paid", invoice.CCStatus);
            Assert.True(invoice.HasReversal);
            Assert.Equal(InvoiceStatus.ClosedUnpaid, invoice.Status);
            Assert.Equal(new DateTime(2026, 6, 1), invoice.DateAssessed);
            Assert.Equal(new DateTime(2026, 7, 2), invoice.DateUpdated);
            Assert.True(invoice.IsPastReturnWindow);
        }

        [Fact]
        public async Task GetInvoices_MissingIsPastReturnWindow_ReadsFalse()
        {
            var client = CreateClient(new StubHandler(_ => Ok("[{\"InvoiceId\":1,\"InvoiceAmount\":1.0,\"Status\":\"Open\",\"DueDate\":\"2026-07-01T00:00:00\"}]")));

            var invoice = Assert.Single(await client.GetInvoices(Token, new DateTime(2026, 1, 1)));

            Assert.False(invoice.IsPastReturnWindow);
        }

        [Fact]
        public async Task GetInvoice_UsesPathAndDeserializesPartiesAndLines()
        {
            Uri capturedUri = null;
            var json = InvoiceJson + ",\"Payer\":{\"Id\":5331803,\"PartyType\":\"PEXBusiness\"},\"Payee\":{\"Id\":1,\"PartyType\":\"PEX\"},\"Lines\":[{\"Amount\":49.90,\"Description\":\"Spend\",\"DateUpdated\":\"2026-06-01T00:00:00\",\"TransactionId\":77,\"LineType\":\"Spend\",\"SourceInvoiceId\":null},{\"Amount\":10.0,\"Description\":\"Returned payment\",\"DateUpdated\":\"2026-06-02T00:00:00\",\"TransactionId\":null,\"LineType\":\"UnpaidInvoice\",\"SourceInvoiceId\":98761}]}";
            var client = CreateClient(new StubHandler(req =>
            {
                capturedUri = req.RequestUri;
                return Ok(json);
            }));

            var invoice = await client.GetInvoice(Token, 98761);

            Assert.Equal("/V4/Invoice/98761", capturedUri.AbsolutePath);
            Assert.Equal(49.40m, invoice.AmountTaken);
            Assert.Equal(5331803, invoice.Payer.Id);
            Assert.Equal("PEXBusiness", invoice.Payer.PartyType);
            Assert.Equal("PEX", invoice.Payee.PartyType);
            Assert.Equal(2, invoice.Lines.Count);
            Assert.Equal(77, invoice.Lines[0].TransactionId);
            Assert.Null(invoice.Lines[0].SourceInvoiceId);
            Assert.Equal("UnpaidInvoice", invoice.Lines[1].LineType);
            Assert.Equal(98761, invoice.Lines[1].SourceInvoiceId);
        }

        [Fact]
        public async Task GetInvoicePayments_DeserializesNewFieldsAndLegs()
        {
            var json = "[{\"PaymentId\":501,\"InvoiceId\":98761,\"PaidTransactionId\":77,\"ReplenishmentDelayEndDate\":\"2026-07-15T00:00:00\",\"PaymentTrigger\":\"Manual\",\"WriteOffReasonCode\":\"Other\",\"RejectionReason\":\"BankReturn\",\"Type\":\"WriteOffReversal\",\"DatePaid\":\"2026-07-10T00:00:00\",\"Amount\":100.0,\"RejectedByBank\":false,\"PaymentGroupId\":\"6f1c2a9e-0000-4000-8000-000000000001\",\"ReleasedAmount\":40.0,\"FloatRole\":\"Held\",\"FloatPercentage\":60.0,\"FloatGroupAmount\":1000.0,\"Legs\":[{\"PaymentId\":502,\"FloatRole\":\"Released\",\"IsPastReturnWindow\":true,\"Legs\":[]}],\"IsPastReturnWindow\":true}]";
            var client = CreateClient(new StubHandler(_ => Ok(json)));

            var payment = Assert.Single(await client.GetInvoicePayments(Token, 98761));

            Assert.Equal(98761, payment.InvoiceId);
            Assert.Equal(77, payment.PaidTransactionId);
            Assert.Equal(new DateTime(2026, 7, 15), payment.ReplenishmentDelayEndDate);
            Assert.Equal("Manual", payment.PaymentTrigger);
            Assert.Equal("Other", payment.WriteOffReasonCode);
            Assert.Equal("BankReturn", payment.RejectionReason);
            Assert.Equal(PaymentType.WriteOffReversal, payment.Type);
            Assert.Equal(Guid.Parse("6f1c2a9e-0000-4000-8000-000000000001"), payment.PaymentGroupId);
            Assert.Equal(40m, payment.ReleasedAmount);
            Assert.Equal("Held", payment.FloatRole);
            Assert.Equal(60m, payment.FloatPercentage);
            Assert.Equal(1000m, payment.FloatGroupAmount);
            Assert.True(payment.IsPastReturnWindow);
            var leg = Assert.Single(payment.Legs);
            Assert.Equal(502, leg.PaymentId);
            Assert.Equal("Released", leg.FloatRole);
            Assert.True(leg.IsPastReturnWindow);
        }

        [Fact]
        public async Task GetInvoiceAllocations_DeserializesSourceInvoiceId()
        {
            var client = CreateClient(new StubHandler(_ => Ok("[{\"InvoiceId\":98761,\"TagName\":\"Funds\",\"TagValue\":\"General\",\"TotalAmount\":10.0,\"TransactionTypeCategory\":\"Spend\",\"SourceInvoiceId\":98700}]")));

            var allocation = Assert.Single(await client.GetInvoiceAllocations(Token, 98761));

            Assert.Equal(98700, allocation.SourceInvoiceId);
        }

        private static PexApiClient CreateClient(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://coreapi.example/") };
            return new PexApiClient(httpClient);
        }

        private static HttpResponseMessage Ok(string json)
            => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(_responder(request));
        }
    }
}
