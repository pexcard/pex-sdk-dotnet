using PexCard.Api.Client.Core.Extensions;
using PexCard.Api.Client.Core.Interfaces;
using PexCard.Api.Client.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Extensions
{
    public class TagExtensionsTests
    {
        [Fact]
        public void UpsertTagOptions_WithDuplicateNamesSameCaseOptions_ThrowDataException()
        {
            //Arrange
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    new TagOptionModel
                    {
                        Value = "One",
                        Name = "Foo"
                    }
                }
            };
            var tagOptions = new List<TagOptionEntity>
            {
                new TagOptionEntity
                {
                    Id = "Two",
                    Name = "Foo"
                },
            };

            //Act
            var ex = Assert.Throws<DataException>(() => tag.UpsertTagOptions(tagOptions, out var syncCount));

            //Assert
            Assert.Contains("has duplicate tag option names and/or values", ex.Message);
        }

        [Fact]
        public void UpsertTagOptions_WithDuplicateNamesDiffCaseOptions_ThrowDataException()
        {
            //Arrange
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    new TagOptionModel
                    {
                        Value = "One",
                        Name = "Foo"
                    }
                }
            };
            var tagOptions = new List<TagOptionEntity>
            {
                new TagOptionEntity
                {
                    Id = "Two",
                    Name = "foo"
                },
            };

            //Act
            var ex = Assert.Throws<DataException>(() => tag.UpsertTagOptions(tagOptions, out var syncCount));

            //Assert
            Assert.Contains("has duplicate tag option names and/or values", ex.Message);
        }

        [Fact]
        public void UpsertTagOptions_WithDuplicateIdsDiffCaseOptions_ThrowDataException()
        {
            //Arrange
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    new TagOptionModel
                    {
                        Value = "One",
                        Name = "Foo"
                    }
                }
            };
            var tagOptions = new List<TagOptionEntity>
            {
                new TagOptionEntity
                {
                    Id = "one",
                    Name = "Bar"
                },
            };

            //Act
            var ex = Assert.Throws<DataException>(() => tag.UpsertTagOptions(tagOptions, out var syncCount));

            //Assert
            Assert.Contains("has duplicate tag option names and/or values", ex.Message);
        }

        [Fact]
        public void UpsertTagOptions_WithDuplicateIdsDiffCaseInUpdateOptions_ThrowDataException()
        {
            //Arrange
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    new TagOptionModel
                    {
                        Value = "One",
                        Name = "Foo"
                    }
                }
            };
            var tagOptions = new List<TagOptionEntity>
            {
                new TagOptionEntity
                {
                    Id = "One",
                    Name = "Bar1"
                },
                new TagOptionEntity
                {
                    Id = "one",
                    Name = "Bar2"
                },
            };

            //Act
            var ex = Assert.Throws<DataException>(() => tag.UpsertTagOptions(tagOptions, out var syncCount));

            //Assert
            Assert.Contains("Duplicate tag option entity names ids and/or ids", ex.Message);
        }

        [Fact]
        public void UpsertTagOptions_WithDuplicateNamesDiffCaseInUpdateOptions_ThrowDataException()
        {
            //Arrange
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    new TagOptionModel
                    {
                        Value = "One",
                        Name = "Foo"
                    }
                }
            };
            var tagOptions = new List<TagOptionEntity>
            {
                new TagOptionEntity
                {
                    Id = "Two",
                    Name = "Bar1"
                },
                new TagOptionEntity
                {
                    Id = "Three",
                    Name = "bar1"
                },
            };

            //Act
            var ex = Assert.Throws<DataException>(() => tag.UpsertTagOptions(tagOptions, out var syncCount));

            //Assert
            Assert.Contains("Duplicate tag option entity names ids and/or ids", ex.Message);
        }

        [Fact]
        public void UpsertTagOptions_WithNullOptions_ThrowDataException()
        {
            //Arrange
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>()
            };

            //Act
            var ex = Assert.Throws<DataException>(() => tag.UpsertTagOptions(null, out var syncCount));

            //Assert
            Assert.Contains("At least one option must be enabled.", ex.Message);
        }

        [Fact]
        public void UpsertTagOptions_WithAllDisabled_EnablesFirstOption()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var onlyTagOption = new TagOptionModel
            {
                Value = tagId,
                Name = tagName,
                IsEnabled = false
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    onlyTagOption
                }
            };

            //Act
            tag.UpsertTagOptions(null, out var syncCount);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(1, syncCount);
            Assert.True(onlyOption.IsEnabled);
        }

        [Fact]
        public void UpsertTagOptions_WithAllDisabled_DoesNotUpdateOptions()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var onlyTagOption = new TagOptionModel
            {
                Value = tagId,
                Name = tagName,
                IsEnabled = false
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    onlyTagOption
                }
            };

            //Act
            tag.UpsertTagOptions(null, out var syncCount);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(tagName, onlyOption.Name);
        }

        [Fact]
        public void UpsertTagOptions_InsertsNewOptions()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var newTagEntity = new TagOptionEntity
            {
                Id = tagId,
                Name = tagName
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>()
            };

            //Act
            tag.UpsertTagOptions(new[] { newTagEntity }, out var syncCount);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(1, syncCount);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(tagName, onlyOption.Name);
        }

        [Fact]
        public void UpsertTagOptions_WithUpdateNamesFalse_DoesNotUpdateExistingOptionNames()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var existingTagOption = new TagOptionModel
            {
                Value = tagId,
                Name = tagName,
                IsEnabled = true
            };
            var exisintTagEntity = new TagOptionEntity
            {
                Id = tagId,
                Name = tagName
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    existingTagOption
                }
            };

            //Act
            tag.UpsertTagOptions(new[] { exisintTagEntity }, out var syncCount);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(0, syncCount);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(tagName, onlyOption.Name);
        }

        [Fact]
        public void UpsertTagOptions_WithUpdateNamesTrue_UpdatesExistingOptionNames()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var newTagName = "New Test Option";
            var existingTagOption = new TagOptionModel
            {
                Value = tagId,
                Name = tagName,
                IsEnabled = true
            };
            var exisintTagEntity = new TagOptionEntity
            {
                Id = tagId,
                Name = newTagName
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    existingTagOption
                }
            };

            //Act
            tag.UpsertTagOptions(new[] { exisintTagEntity }, out var syncCount, true);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(1, syncCount);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(newTagName, onlyOption.Name);
        }

        [Fact]
        public void UpsertTagOptions_WithUpdateNamesDefault_UpdatesExistingOptionNames()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var newTagName = "New Test Option";
            var existingTagOption = new TagOptionModel
            {
                Value = tagId,
                Name = tagName,
                IsEnabled = true
            };
            var exisintTagEntity = new TagOptionEntity
            {
                Id = tagId,
                Name = newTagName
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    existingTagOption
                }
            };

            //Act
            tag.UpsertTagOptions(new[] { exisintTagEntity }, out var syncCount);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(1, syncCount);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(newTagName, onlyOption.Name);
        }

        private class TagOptionEntity : IMatchableEntity
        {
            public string Id { get; set; }

            public string Name { get; set; }

            string IMatchableEntity.EntityId => Id;

            string IMatchableEntity.EntityName => Name;
        }
    }
}
