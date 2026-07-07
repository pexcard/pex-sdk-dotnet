using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class BillPaymentSerializationTests
    {
        [Fact]
        public void BillPaymentModel_DeserializesBillDateFromServer()
        {
            const string json = @"{
                ""BillId"": 501,
                ""BillRefNo"": ""INV-501"",
                ""Amount"": 250.00,
                ""Created"": ""2026-01-20T00:00:00+00:00"",
                ""DueDate"": ""2026-02-01T00:00:00+00:00"",
                ""BillDate"": ""2026-01-15T00:00:00+00:00""
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Equal(501, model.BillId);
            Assert.Equal(new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero), model.BillDate);
            Assert.Equal(new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero), model.DueDate);
        }

        [Fact]
        public void BillPaymentModel_OmitsBillDate_WhenNull()
        {
            var model = new BillPaymentModel { BillId = 1 };

            var json = JsonConvert.SerializeObject(
                model,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            Assert.DoesNotContain("BillDate", json);
        }

        [Fact]
        public void BillPaymentListResponse_RoundTripsBillDate()
        {
            var original = new BillPaymentListResponseModel
            {
                Items = new List<BillPaymentModel>
                {
                    new BillPaymentModel
                    {
                        BillId = 7,
                        BillDate = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero),
                        DueDate = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
                    }
                },
                PageInfo = new PageInfoModel { Page = 1, PageSize = 15, TotalItems = 1 }
            };

            var json = JsonConvert.SerializeObject(original);
            var roundTripped = JsonConvert.DeserializeObject<BillPaymentListResponseModel>(json);

            Assert.Single(roundTripped.Items);
            Assert.Equal(new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero), roundTripped.Items[0].BillDate);
        }
    }
}
