using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
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
            Assert.Equal(1, roundTripped.PageInfo.Page);
            Assert.Equal(15, roundTripped.PageInfo.PageSize);
            Assert.Equal(1, roundTripped.PageInfo.TotalItems);
        }

        [Fact]
        public void BillPaymentListResponse_MapsExternalApiSearchPageInfo()
        {
            const string json = @"{
                ""Items"": [],
                ""PageInfo"": {
                    ""TotalCount"": 137,
                    ""PageNumber"": 2,
                    ""PageSize"": 50,
                    ""TotalPages"": 3
                }
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentListResponseModel>(json);

            Assert.Equal(2, model.PageInfo.Page);
            Assert.Equal(50, model.PageInfo.PageSize);
            Assert.Equal(137, model.PageInfo.TotalItems);
        }

        [Fact]
        public void BillPaymentListResponse_PageInfo_IsNullWhenServerReturnsNull()
        {
            const string json = @"{ ""Items"": [], ""PageInfo"": null }";

            var model = JsonConvert.DeserializeObject<BillPaymentListResponseModel>(json);

            Assert.Null(model.PageInfo);
        }

        [Fact]
        public void BillPaymentModel_DeserializesSettlementTransactionIdFromServer()
        {
            const string json = @"{
                ""BillId"": 501,
                ""BillRefNo"": ""INV-501"",
                ""SettlementTransactionId"": ""1234567890""
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Equal("1234567890", model.SettlementTransactionId);
        }

        [Fact]
        public void BillPaymentModel_SettlementTransactionId_IsNullWhenServerOmitsIt()
        {
            const string json = @"{
                ""BillId"": 501,
                ""BillRefNo"": ""INV-501""
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Null(model.SettlementTransactionId);
        }

        [Fact]
        public void BillPaymentModel_PreservesNonNumericSettlementTransactionId()
        {
            const string json = @"{
                ""BillId"": 501,
                ""SettlementTransactionId"": ""SETTLE-1""
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Equal("SETTLE-1", model.SettlementTransactionId);
        }

        [Fact]
        public void BillPaymentRequestModel_DeserializesSettlementTransactionIdFromServer()
        {
            const string json = @"{
                ""PaymentRequestId"": 42,
                ""BillRefNo"": ""INV-42"",
                ""SettlementTransactionId"": ""9876543210""
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentRequestModel>(json);

            Assert.Equal(42, model.PaymentRequestId);
            Assert.Equal("9876543210", model.SettlementTransactionId);
        }

        [Fact]
        public void BillPaymentRequestModel_SettlementTransactionId_IsNullWhenServerOmitsIt()
        {
            const string json = @"{
                ""PaymentRequestId"": 42,
                ""BillRefNo"": ""INV-42""
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentRequestModel>(json);

            Assert.Null(model.SettlementTransactionId);
        }

        [Theory]
        [InlineData("ReimbursementAch", PaymentTransferType.ReimbursementAch)]
        [InlineData("BillPayAch", PaymentTransferType.BillPayAch)]
        [InlineData("BillPayVirtualCards", PaymentTransferType.BillPayVirtualCards)]
        [InlineData("BillPayNonPlatform", PaymentTransferType.BillPayNonPlatform)]
        public void BillPaymentModel_DeserializesPaymentTransferTypeFromServer(string serverValue, PaymentTransferType expected)
        {
            var json = $@"{{
                ""BillId"": 501,
                ""PaymentTransferType"": ""{serverValue}""
            }}";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Equal(expected, model.PaymentTransferType);
        }

        [Fact]
        public void BillPaymentModel_PaymentTransferType_IsNullWhenServerSendsNull()
        {
            const string json = @"{
                ""BillId"": 501,
                ""PaymentTransferType"": null
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Null(model.PaymentTransferType);
        }

        [Fact]
        public void BillPaymentModel_PaymentTransferType_IsNullWhenServerOmitsIt()
        {
            const string json = @"{
                ""BillId"": 501
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Null(model.PaymentTransferType);
        }

        [Fact]
        public void BillPaymentModel_PaymentTransferType_IsNullWhenServerSendsAnUnknownName()
        {
            const string json = @"{
                ""BillId"": 501,
                ""PaymentTransferType"": ""BillPayCheck"",
                ""BillRefNo"": ""INV-501""
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Null(model.PaymentTransferType);
            Assert.Equal(501, model.BillId);
            Assert.Equal("INV-501", model.BillRefNo);
        }

        [Fact]
        public void BillPaymentListResponse_ReadsEveryBillWhenOneHasAnUnknownPaymentTransferType()
        {
            const string json = @"{
                ""Items"": [
                    { ""BillId"": 1, ""PaymentTransferType"": ""BillPayAch"" },
                    { ""BillId"": 2, ""PaymentTransferType"": ""BillPayCheck"" },
                    { ""BillId"": 3, ""PaymentTransferType"": ""BillPayVirtualCards"" }
                ]
            }";

            var response = JsonConvert.DeserializeObject<BillPaymentListResponseModel>(json);

            Assert.Equal(
                new PaymentTransferType?[] { PaymentTransferType.BillPayAch, null, PaymentTransferType.BillPayVirtualCards },
                response.Items.Select(i => i.PaymentTransferType));
        }

        [Fact]
        public void BillPaymentModel_ReadsANumericPaymentTransferType()
        {
            const string json = @"{
                ""BillId"": 501,
                ""PaymentTransferType"": 20
            }";

            var model = JsonConvert.DeserializeObject<BillPaymentModel>(json);

            Assert.Equal(PaymentTransferType.BillPayAch, model.PaymentTransferType);
        }

        [Fact]
        public void BillPaymentModel_WritesPaymentTransferTypeAsANumber()
        {
            var json = JsonConvert.SerializeObject(new BillPaymentModel { PaymentTransferType = PaymentTransferType.BillPayVirtualCards });

            Assert.Contains(@"""PaymentTransferType"":30", json);
        }

        [Fact]
        public void PaymentTransferType_ValuesMatchTheServerEnum()
        {
            Assert.Equal(10, (int)PaymentTransferType.ReimbursementAch);
            Assert.Equal(20, (int)PaymentTransferType.BillPayAch);
            Assert.Equal(30, (int)PaymentTransferType.BillPayVirtualCards);
            Assert.Equal(40, (int)PaymentTransferType.BillPayNonPlatform);
        }
    }
}
