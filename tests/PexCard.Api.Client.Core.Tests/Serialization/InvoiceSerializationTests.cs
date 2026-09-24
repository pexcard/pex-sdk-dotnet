using System;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class InvoiceSerializationTests
    {
        [Fact]
        public void InvoiceModel_DeserializesIsSettled()
        {
            const string json = @"{
                ""InvoiceId"": 98761,
                ""InvoiceAmount"": 49.90,
                ""Status"": 2,
                ""DueDate"": ""2026-06-10T00:00:00"",
                ""IsSettled"": true
            }";

            var model = JsonConvert.DeserializeObject<InvoiceModel>(json);

            Assert.Equal(98761, model.InvoiceId);
            Assert.Equal(49.90m, model.InvoiceAmount);
            Assert.Equal(InvoiceStatus.Closed, model.Status);
            Assert.Equal(new DateTime(2026, 6, 10), model.DueDate);
            Assert.True(model.IsSettled);
        }

        [Fact]
        public void InvoiceModel_IsSettledDefaultsToFalseWhenAbsent()
        {
            const string json = @"{ ""InvoiceId"": 1, ""InvoiceAmount"": 10, ""Status"": 2, ""DueDate"": ""2026-06-10T00:00:00"" }";

            var model = JsonConvert.DeserializeObject<InvoiceModel>(json);

            Assert.False(model.IsSettled);
        }

        [Fact]
        public void InvoiceModel_SerializesIsSettled()
        {
            var json = JsonConvert.SerializeObject(new InvoiceModel { InvoiceId = 1, IsSettled = true });

            Assert.Contains("\"IsSettled\":true", json);
        }

        [Theory]
        [InlineData("5", InvoiceStatus.ClosedUnpaid)]
        [InlineData("\"ClosedUnpaid\"", InvoiceStatus.ClosedUnpaid)]
        public void InvoiceStatus_DeserializesClosedUnpaid(string statusJson, InvoiceStatus expected)
        {
            var model = JsonConvert.DeserializeObject<InvoiceModel>($@"{{ ""InvoiceId"": 1, ""Status"": {statusJson} }}");

            Assert.Equal(expected, model.Status);
            Assert.Equal(5, (int)model.Status);
        }

        [Theory]
        [InlineData("9", PaymentType.WriteOffReversal)]
        [InlineData("\"WriteOffReversal\"", PaymentType.WriteOffReversal)]
        public void PaymentType_DeserializesWriteOffReversal(string typeJson, PaymentType expected)
        {
            var type = JsonConvert.DeserializeObject<PaymentType>(typeJson);

            Assert.Equal(expected, type);
            Assert.Equal(9, (int)type);
        }
    }
}
