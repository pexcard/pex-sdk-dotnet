using Newtonsoft.Json;
using PexCard.Api.Client.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Serialization
{
    public class TagDeserializationTests
    {
        /// <summary>
        /// Only when we deserialize into "TagDetailsModel" do we get dynamic tag sub type models
        /// </summary>
        [Fact]
        public void DeserializeTagDetailsJsonIntoListOfTagDetailsModel()
        {
            //Arrange
            var textTag = new TagTextDetailsModel
            {
                Type = Enums.CustomFieldType.Text,
                Id = "textTag",
                Name = "Text Tag",
                Description = "Text Tag Desc",
                IsEnabled = true,
                IsDeleted = false,
                IsRequired = true,
                Length = 10,
                ValidationType = TagTextValidationType.Numeric
            };
            var dropdownTag = new TagDropdownDetailsModel
            {
                Type = Enums.CustomFieldType.Dropdown,
                Id = "dropdownTag",
                Name = "Dropdown Tag",
                Description = "Dropdown Tag Desc",
                IsEnabled = true,
                IsDeleted = false,
                IsRequired = true,
                Options = new List<TagOptionModel>
                {
                    new TagOptionModel
                    {
                        Name = "Option 1",
                        Value = "Value 1",
                        IsEnabled = true,
                    }
                }
            };
            var decimalTag = new TagDetailsModel
            {
                Type = Enums.CustomFieldType.Decimal,
                Id = "decimalTag",
                Name = "Decimal Tag",
                Description = "Decimal Tag Desc",
                IsEnabled = true,
                IsDeleted = false,
                IsRequired = true
            };
            var yesNoTag = new TagDetailsModel
            {
                Type = Enums.CustomFieldType.YesNo,
                Id = "yesNoTag",
                Name = "YesNo Tag",
                Description = "YesNo Tag Desc",
                IsEnabled = true,
                IsDeleted = false,
                IsRequired = true
            };
            var tags = new List<TagDataModel> { textTag, dropdownTag, decimalTag, yesNoTag };
            var tagsJson = JsonConvert.SerializeObject(tags);

            //Act
            var tagsFromJson = JsonConvert.DeserializeObject<List<TagDetailsModel>>(tagsJson);

            //Assert
            Assert.True(tagsFromJson.Count == tags.Count);
            Assert.Contains(tagsFromJson, x => x.Type == Enums.CustomFieldType.Text && x is TagTextDetailsModel t && t.Id == textTag.Id && t.Length == textTag.Length && t.ValidationType == textTag.ValidationType);
            Assert.Contains(tagsFromJson, x => x.Type == Enums.CustomFieldType.Dropdown && x is TagDropdownDetailsModel t && t.Id == dropdownTag.Id && dropdownTag.Options.All(y => t.Options.Any(z => z.Value == y.Value)));
            Assert.Contains(tagsFromJson, x => x.Type == Enums.CustomFieldType.Decimal && x.Id == decimalTag.Id);
            Assert.Contains(tagsFromJson, x => x.Type == Enums.CustomFieldType.YesNo && x.Id == yesNoTag.Id);
        }

        /// <summary>
        /// Only when we deserialize into "TagDetailsModel" do we get dynamic tag sub type models
        /// </summary>
        [Fact]
        public void DeserializeTagDetailsJsonIntoListOfTagDataModels()
        {
            //Arrange
            var textTag = new TagTextDetailsModel
            {
                Type = Enums.CustomFieldType.Text,
                Id = "textTag",
                Name = "Text Tag",
                Description = "Text Tag Desc",
                IsEnabled = true,
                IsDeleted = false,
                IsRequired = true,
                Length = 10,
                ValidationType = TagTextValidationType.Numeric
            };
            var dropdownTag = new TagDropdownDetailsModel
            {
                Type = Enums.CustomFieldType.Dropdown,
                Id = "dropdownTag",
                Name = "Dropdown Tag",
                Description = "Dropdown Tag Desc",
                IsEnabled = true,
                IsDeleted = false,
                IsRequired = true,
                Options = new List<TagOptionModel>
                {
                    new TagOptionModel
                    {
                        Name = "Option 1",
                        Value = "Value 1",
                        IsEnabled = true,
                    }
                }
            };
            var decimalTag = new TagDetailsModel
            {
                Type = Enums.CustomFieldType.Decimal,
                Id = "decimalTag",
                Name = "Decimal Tag",
                Description = "Decimal Tag Desc",
                IsEnabled = true,
                IsDeleted = false,
                IsRequired = true
            };
            var yesNoTag = new TagDetailsModel
            {
                Type = Enums.CustomFieldType.YesNo,
                Id = "yesNoTag",
                Name = "YesNo Tag",
                Description = "YesNo Tag Desc",
                IsEnabled = true,
                IsDeleted = false,
                IsRequired = true
            };
            var tags = new List<TagDataModel> { textTag, dropdownTag, decimalTag, yesNoTag };
            var tagsJson = JsonConvert.SerializeObject(tags);

            //Act
            var tagsFromJson = JsonConvert.DeserializeObject<List<TagDataModel>>(tagsJson);

            //Assert
            Assert.True(tagsFromJson.Count == tags.Count);
            Assert.Contains(tagsFromJson, x => x.Type == Enums.CustomFieldType.Text && x is not TagDetailsModel && x is not TagTextDetailsModel);
            Assert.Contains(tagsFromJson, x => x.Type == Enums.CustomFieldType.Dropdown && x is not TagDetailsModel && x is not TagDropdownDetailsModel);
            Assert.Contains(tagsFromJson, x => x.Type == Enums.CustomFieldType.Decimal && x is not TagDetailsModel);
            Assert.Contains(tagsFromJson, x => x.Type == Enums.CustomFieldType.YesNo && x is not TagDetailsModel);
        }
    }
}
