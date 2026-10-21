using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;
using System.Collections.Generic;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class TagFeatureSourceSerializationTests
    {
        [Fact]
        public void DeserializeTagDetails_ReadsFeatureSourcesByName()
        {
            // the External API response shape (names written by its enum converter)
            var json = "{\"Id\":\"tag\",\"Name\":\"Tag\",\"Type\":\"Decimal\",\"FeatureSources\":[\"Transaction\",\"Reimbursement\",\"BillPay\"]}";

            var tag = JsonConvert.DeserializeObject<TagDetailsModel>(json);

            Assert.Equal(new[] { TagFeatureSource.Transaction, TagFeatureSource.Reimbursement, TagFeatureSource.BillPay }, tag.FeatureSources);
        }

        [Fact]
        public void DeserializeTagDetailsList_FillsFeatureSourcesOnEverySubtype()
        {
            var json = @"[
                { ""Id"": ""text"", ""Type"": ""Text"", ""Length"": 10, ""FeatureSources"": [ ""Transaction"" ] },
                { ""Id"": ""dropdown"", ""Type"": ""Dropdown"", ""Options"": [], ""FeatureSources"": [ ""BillPay"", ""Reimbursement"" ] },
                { ""Id"": ""yesno"", ""Type"": ""YesNo"", ""FeatureSources"": [ ""Transaction"", ""Reimbursement"", ""BillPay"" ] }
            ]";

            var tags = JsonConvert.DeserializeObject<List<TagDetailsModel>>(json);

            Assert.Equal(3, tags.Count);
            var text = Assert.IsType<TagTextDetailsModel>(tags[0]);
            Assert.Equal(new[] { TagFeatureSource.Transaction }, text.FeatureSources);
            var dropdown = Assert.IsType<TagDropdownDetailsModel>(tags[1]);
            Assert.Equal(new[] { TagFeatureSource.BillPay, TagFeatureSource.Reimbursement }, dropdown.FeatureSources);
            Assert.Equal(typeof(TagDetailsModel), tags[2].GetType());
            Assert.Equal(3, tags[2].FeatureSources.Count);
        }

        [Fact]
        public void DeserializeTagDetails_WithoutFeatureSources_LeavesNull()
        {
            // an older External API that does not send the field
            var tag = JsonConvert.DeserializeObject<TagDetailsModel>("{\"Id\":\"tag\",\"Name\":\"Tag\",\"Type\":\"Decimal\"}");

            Assert.Null(tag.FeatureSources);
        }

        [Fact]
        public void DeserializeTagDetails_WithUnknownFeature_KeepsOtherEntries()
        {
            // a feature added to the API after this SDK version
            var json = "{\"Id\":\"tag\",\"Type\":\"Decimal\",\"FeatureSources\":[\"Transaction\",\"FundingRequest\",\"BillPay\"]}";

            var tag = JsonConvert.DeserializeObject<TagDetailsModel>(json);

            Assert.Equal(new[] { TagFeatureSource.Transaction, TagFeatureSource.Unknown, TagFeatureSource.BillPay }, tag.FeatureSources);
        }

        [Theory]
        [InlineData("1", TagFeatureSource.Transaction)]
        [InlineData("\"1\"", TagFeatureSource.Transaction)]
        [InlineData("\"transaction\"", TagFeatureSource.Transaction)]
        [InlineData("\" BillPay \"", TagFeatureSource.BillPay)]
        [InlineData("\"Reimbursement\"", TagFeatureSource.Reimbursement)]
        [InlineData("\"Foo\"", TagFeatureSource.Unknown)]
        [InlineData("\"\"", TagFeatureSource.Unknown)]
        [InlineData("0", TagFeatureSource.Unknown)]
        [InlineData("null", TagFeatureSource.Unknown)]
        [InlineData("6", (TagFeatureSource)6)]
        [InlineData("99999999999999999999", TagFeatureSource.Unknown)]
        public void DeserializeFeatureSource_ReadsTolerantly(string valueJson, TagFeatureSource expected)
        {
            var tag = JsonConvert.DeserializeObject<TagDetailsModel>($"{{\"Id\":\"tag\",\"Type\":\"Decimal\",\"FeatureSources\":[{valueJson}]}}");

            Assert.Equal(new[] { expected }, tag.FeatureSources);
        }

        [Fact]
        public void DeserializeNullableFeatureSource_ReadsNullAsNull()
        {
            var holder = JsonConvert.DeserializeObject<Holder>("{\"Source\":null}");

            Assert.Null(holder.Source);
        }

        [Fact]
        public void SerializeFeatureSources_WritesNumbersByDefault()
        {
            var tag = new TagModel { Id = "tag", FeatureSources = new List<TagFeatureSource> { TagFeatureSource.Transaction, TagFeatureSource.BillPay } };

            var json = JsonConvert.SerializeObject(tag);

            Assert.Contains("\"FeatureSources\":[1,3]", json);
        }

        [Fact]
        public void SerializeFeatureSources_UsesCallerStringEnumConverter()
        {
            var tag = new TagModel { Id = "tag", FeatureSources = new List<TagFeatureSource> { TagFeatureSource.Transaction, TagFeatureSource.BillPay } };

            var json = JsonConvert.SerializeObject(tag, new JsonSerializerSettings { Converters = { new StringEnumConverter() } });

            Assert.Contains("\"FeatureSources\":[\"Transaction\",\"BillPay\"]", json);
        }

        [Fact]
        public void SerializeCreateModel_DoesNotCarryFeatureSources()
        {
            // the API does not accept the field on create; the create models derive from TagDataModel, not TagModel
            var json = JsonConvert.SerializeObject(new TagDropdownDataModel { Name = "Tag", Type = CustomFieldType.Dropdown });

            Assert.DoesNotContain("FeatureSources", json);
        }

        private class Holder
        {
            public TagFeatureSource? Source { get; set; }
        }
    }
}
