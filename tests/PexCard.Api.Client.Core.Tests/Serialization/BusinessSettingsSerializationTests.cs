using Newtonsoft.Json;
using PexCard.Api.Client.Core.Models;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class BusinessSettingsSerializationTests
    {
        [Fact]
        public void BusinessSettings_DeserializesReviewSettings()
        {
            const string json = "{\"UseBillPay\":true,\"FullMetadataRequired\":true,\"FullMetadataThresholdAmount\":75.0,\"RequiredNotesEnabled\":true,\"RequiredNotesThresholdAmount\":25.5,\"ReceiptReminderEnabled\":false,\"ReceiptReminderThresholdAmount\":0}";

            var settings = JsonConvert.DeserializeObject<BusinessSettingsModel>(json);

            Assert.True(settings.UseBillPay);
            Assert.True(settings.FullMetadataRequired);
            Assert.Equal(75m, settings.FullMetadataThresholdAmount);
            Assert.True(settings.RequiredNotesEnabled);
            Assert.Equal(25.5m, settings.RequiredNotesThresholdAmount);
            Assert.False(settings.ReceiptReminderEnabled);
            Assert.Equal(0m, settings.ReceiptReminderThresholdAmount);
        }

        [Fact]
        public void BusinessSettings_LeavesReviewSettingsNull_WhenMissingOrNull()
        {
            const string json = "{\"UseBillPay\":true,\"FullMetadataRequired\":null,\"FullMetadataThresholdAmount\":null}";

            var settings = JsonConvert.DeserializeObject<BusinessSettingsModel>(json);

            Assert.True(settings.UseBillPay);
            Assert.Null(settings.FullMetadataRequired);
            Assert.Null(settings.FullMetadataThresholdAmount);
            Assert.Null(settings.RequiredNotesEnabled);
            Assert.Null(settings.RequiredNotesThresholdAmount);
            Assert.Null(settings.ReceiptReminderEnabled);
            Assert.Null(settings.ReceiptReminderThresholdAmount);
        }
    }
}
