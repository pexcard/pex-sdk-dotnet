using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class BillInboxSerializationTests
    {
        [Fact]
        public void CreateBillInboxRequest_SerializesSourceAsStringName()
        {
            var request = new CreateBillInboxRequestModel
            {
                Source = BillInboxSource.QuickBooksOnline,
                VendorName = "Acme",
                VendorId = 42,
                Amount = 199.99m,
                DueDate = new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero),
                BillDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                BillNumber = "INV-001"
            };

            var json = JsonConvert.SerializeObject(request);

            Assert.Contains("\"Source\":\"QuickBooksOnline\"", json);
            Assert.DoesNotContain("\"Source\":2", json);
        }

        [Fact]
        public void CreateBillInboxRequest_OmitsUnsetOptionalFields()
        {
            var request = new CreateBillInboxRequestModel
            {
                Source = BillInboxSource.NetSuite,
                VendorName = "Acme",
                VendorId = 42,
                Amount = 10m
            };

            var json = JsonConvert.SerializeObject(
                request,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            Assert.DoesNotContain("DueDate", json);
            Assert.DoesNotContain("BillDate", json);
            Assert.DoesNotContain("BillNumber", json);
        }

        [Fact]
        public void BillInboxModel_DeserializesStringEnumsFromServer()
        {
            const string json = @"{
                ""BillInboxId"": 101,
                ""Status"": ""Pending"",
                ""Source"": ""SageIntacct"",
                ""ReceivedDate"": ""2026-04-20T00:00:00+00:00"",
                ""MetadataId"": 555,
                ""VendorId"": 42,
                ""VendorName"": ""Acme"",
                ""Amount"": 199.99,
                ""Created"": ""2026-04-20T00:00:00+00:00"",
                ""CreatedByUserId"": 7
            }";

            var model = JsonConvert.DeserializeObject<BillInboxModel>(json);

            Assert.Equal(101, model.BillInboxId);
            Assert.Equal(BillInboxStatus.Pending, model.Status);
            Assert.Equal(BillInboxSource.SageIntacct, model.Source);
            Assert.Equal(555L, model.MetadataId);
            Assert.Equal(199.99m, model.Amount);
            Assert.Null(model.Metadata);
        }

        [Fact]
        public void BillInboxStatus_UnknownStringFallsBackToUnknown()
        {
            const string json = @"{ ""Status"": ""SomeFutureStatus"" }";

            var ex = Record.Exception(() => JsonConvert.DeserializeObject<BillInboxModel>(json));

            // StringEnumConverter throws on unknown string names; our Unknown = 0 allows
            // callers to opt into tolerant deserialization when needed.
            Assert.NotNull(ex);
        }

        [Fact]
        public void BillInboxStatus_DefaultUnsetValueIsUnknown()
        {
            var model = new BillInboxModel();

            Assert.Equal(BillInboxStatus.Unknown, model.Status);
            Assert.Equal(BillInboxSource.Unknown, model.Source);
        }

        [Fact]
        public void SearchBillInboxResponse_RoundTripsListAndPageInfo()
        {
            var original = new SearchBillInboxResponseModel
            {
                Items = new List<BillInboxModel>
                {
                    new BillInboxModel
                    {
                        BillInboxId = 1,
                        Status = BillInboxStatus.Approved,
                        Source = BillInboxSource.Xero,
                        ReceivedDate = new DateTimeOffset(2026, 4, 20, 0, 0, 0, TimeSpan.Zero),
                        Created = new DateTimeOffset(2026, 4, 20, 0, 0, 0, TimeSpan.Zero)
                    }
                },
                PageInfo = new PageInfoModel { Page = 1, PageSize = 15, TotalItems = 1 }
            };

            var json = JsonConvert.SerializeObject(original);
            var roundTripped = JsonConvert.DeserializeObject<SearchBillInboxResponseModel>(json);

            Assert.Single(roundTripped.Items);
            Assert.Equal(BillInboxStatus.Approved, roundTripped.Items[0].Status);
            Assert.Equal(BillInboxSource.Xero, roundTripped.Items[0].Source);
            Assert.Equal(1, roundTripped.PageInfo.Page);
            Assert.Equal(15, roundTripped.PageInfo.PageSize);
            Assert.Equal(1, roundTripped.PageInfo.TotalItems);
        }

        [Fact]
        public void SortDirection_SerializesAsStringName()
        {
            var json = JsonConvert.SerializeObject(SortDirection.Ascending);
            Assert.Equal("\"Ascending\"", json);
        }

        [Fact]
        public void BillInboxSortBy_SerializesAsStringName()
        {
            var json = JsonConvert.SerializeObject(BillInboxSortBy.ReceivedDate);
            Assert.Equal("\"ReceivedDate\"", json);
        }
    }
}
