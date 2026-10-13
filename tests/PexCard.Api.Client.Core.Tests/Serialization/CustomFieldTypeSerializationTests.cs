using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;
using System.Collections.Generic;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class CustomFieldTypeSerializationTests
    {
        private const CustomFieldType Unknown = (CustomFieldType)(-1);

        [Theory]
        [InlineData("7", CustomFieldType.Odometer)]
        [InlineData("\"7\"", CustomFieldType.Odometer)]
        [InlineData("\"Odometer\"", CustomFieldType.Odometer)]
        [InlineData("\"odometer\"", CustomFieldType.Odometer)]
        [InlineData("6", CustomFieldType.MerchantAddress)]
        [InlineData("\"MerchantAddress\"", CustomFieldType.MerchantAddress)]
        [InlineData("3", CustomFieldType.Decimal)]
        [InlineData("\"Decimal\"", CustomFieldType.Decimal)]
        [InlineData("10", CustomFieldType.Allocation)]
        [InlineData("\"Foo\"", Unknown)]
        [InlineData("\"-1\"", Unknown)]
        [InlineData("-1", Unknown)]
        [InlineData("8", Unknown)]
        [InlineData("\"\"", Unknown)]
        [InlineData("\"Text, YesNo\"", Unknown)]
        [InlineData("null", Unknown)]
        [InlineData("99999999999999999999", Unknown)]
        public void DeserializeTagDetails_ReadsTypeTolerantly(string typeJson, CustomFieldType expected)
        {
            var json = $"{{\"Id\":\"tag\",\"Name\":\"Tag\",\"Type\":{typeJson}}}";

            var tag = JsonConvert.DeserializeObject<TagDetailsModel>(json);

            Assert.Equal(expected, tag.Type);
            Assert.Equal("tag", tag.Id);
            Assert.Equal(typeof(TagDetailsModel), tag.GetType());
        }

        [Theory]
        [InlineData("\"Odometer\"", CustomFieldType.Odometer)]
        [InlineData("7", CustomFieldType.Odometer)]
        [InlineData("\"Foo\"", Unknown)]
        public void DeserializeTagData_ReadsTypeTolerantly(string typeJson, CustomFieldType expected)
        {
            var tag = JsonConvert.DeserializeObject<TagDataModel>($"{{\"Name\":\"Tag\",\"Type\":{typeJson}}}");

            Assert.Equal(expected, tag.Type);
        }

        [Fact]
        public void DeserializeTagDetailsList_WithUnknownTypes_KeepsKnownSubtypes()
        {
            var json = @"[
                { ""Id"": ""text"", ""Type"": ""Text"", ""Length"": 10 },
                { ""Id"": ""foo"", ""Type"": ""Foo"" },
                { ""Id"": ""dropdown"", ""Type"": 2, ""Options"": [ { ""Name"": ""Option 1"", ""Value"": ""Value 1"" } ] },
                { ""Id"": ""minusOne"", ""Type"": ""-1"" },
                { ""Id"": ""odometer"", ""Type"": ""Odometer"" },
                { ""Id"": ""odometerNumber"", ""Type"": ""7"" }
            ]";

            var tags = JsonConvert.DeserializeObject<List<TagDetailsModel>>(json);

            Assert.Equal(6, tags.Count);
            Assert.True(tags[0] is TagTextDetailsModel { Type: CustomFieldType.Text, Length: 10 });
            Assert.True(tags[1].GetType() == typeof(TagDetailsModel) && tags[1].Type == Unknown && tags[1].Id == "foo");
            Assert.True(tags[2] is TagDropdownDetailsModel { Type: CustomFieldType.Dropdown } dropdown && dropdown.Options.Count == 1);
            Assert.True(tags[3].GetType() == typeof(TagDetailsModel) && tags[3].Type == Unknown);
            Assert.True(tags[4].GetType() == typeof(TagDetailsModel) && tags[4].Type == CustomFieldType.Odometer);
            Assert.True(tags[5].GetType() == typeof(TagDetailsModel) && tags[5].Type == CustomFieldType.Odometer);
        }

        [Theory]
        [InlineData(CustomFieldType.Dropdown, "\"Type\":2")]
        [InlineData(CustomFieldType.Odometer, "\"Type\":7")]
        [InlineData(CustomFieldType.MerchantAddress, "\"Type\":6")]
        public void SerializeTagData_WritesTypeAsNumber(CustomFieldType type, string expected)
        {
            var json = JsonConvert.SerializeObject(new TagDropdownDataModel { Name = "Tag", Type = type });

            Assert.Contains(expected, json);
        }

        [Fact]
        public void NullableCustomFieldType_ReadsNullAsNull()
        {
            Assert.Null(JsonConvert.DeserializeObject<CustomFieldType?>("null"));
            Assert.Equal(CustomFieldType.Odometer, JsonConvert.DeserializeObject<CustomFieldType?>("\"Odometer\""));
        }
    }
}
