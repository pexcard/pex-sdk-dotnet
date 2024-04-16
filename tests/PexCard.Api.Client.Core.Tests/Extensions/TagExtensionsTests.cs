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
        public void UpdateTagOptions_WithDuplicateNamesSameCaseOptions_AddsAsterisks()
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
            tag.UpdateTagOptions(tagOptions, out var countUpdated);

            //Assert
            Assert.True(tag.Options.Find(x => x.Value == "One").Name == "Foo*");
        }

        [Fact]
        public void UpdateTagOptions_WithDuplicateNamesSameCaseOptions_ThrowDataException()
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
            var ex = Assert.Throws<DataException>(() => tag.UpdateTagOptions(tagOptions, out var countUpdated, handleDuplicates: false));

            //Assert
            Assert.Contains("has duplicate tag option names and/or values", ex.Message);
        }

        [Fact]
        public void UpdateTagOptions_WithDuplicateNamesDiffCaseOptions_AddsAsterisks()
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
            tag.UpdateTagOptions(tagOptions, out var countUpdated);

            //Assert
            Assert.True(tag.Options.Find(x => x.Value == "One").Name == "Foo*");
        }

        [Fact]
        public void UpdateTagOptions_WithDuplicateNamesDiffCaseOptions_ThrowDataException()
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
            var ex = Assert.Throws<DataException>(() => tag.UpdateTagOptions(tagOptions, out var countUpdated, handleDuplicates: false));

            //Assert
            Assert.Contains("has duplicate tag option names and/or values", ex.Message);
        }

        [Fact]
        public void UpdateTagOptions_WithDuplicateIdsDiffCaseOptions_ThrowDataException()
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
                    Name = "foo"
                },
                new TagOptionEntity
                {
                    Id = "One",
                    Name = "Bar"
                },
            };

            //Act
            var ex = Assert.Throws<DataException>(() => tag.UpdateTagOptions(tagOptions, out var countUpdated));

            //Assert
            Assert.Contains("Duplicate input entity names and/or ids", ex.Message);
        }

        [Fact]
        public void UpdateTagOptions_WithDuplicateIdsDiffCaseInUpdateOptions_ThrowDataException()
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
            var ex = Assert.Throws<DataException>(() => tag.UpdateTagOptions(tagOptions, out var countUpdated));

            //Assert
            Assert.Contains("Duplicate input entity names and/or ids", ex.Message);
        }

        [Fact]
        public void UpdateTagOptions_WithDuplicateNamesDiffCaseInUpdateOptions_ThrowDataException()
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
            var ex = Assert.Throws<DataException>(() => tag.UpdateTagOptions(tagOptions, out var countUpdated));

            //Assert
            Assert.Contains("Duplicate input entity names and/or ids", ex.Message);
        }

        [Fact]
        public void UpdateTagOptions_WithNullOptions_ThrowDataException()
        {
            //Arrange
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>()
            };

            //Act
            var ex = Assert.Throws<DataException>(() => tag.UpdateTagOptions(null, out var countUpdated));

            //Assert
            Assert.Contains("At least one option must be enabled.", ex.Message);
        }

        [Fact]
        public void UpdateTagOptions_WithAllDisabled_EnablesFirstOption()
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
            tag.UpdateTagOptions(null, out var countUpdated);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(1, countUpdated);
            Assert.True(onlyOption.IsEnabled);
        }

        [Fact]
        public void UpdateTagOptions_WithAllDisabled_DoesNotUpdateOptions()
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
            tag.UpdateTagOptions(null, out var countUpdated);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(tagName, onlyOption.Name);
        }

        [Fact]
        public void UpdateTagOptions_InsertsNewOptions()
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
            tag.UpdateTagOptions(new[] { newTagEntity }, out var countUpdated);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(1, countUpdated);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(tagName, onlyOption.Name);
        }

        [Fact]
        public void UpdateTagOptions_WithUpdateNamesFalse_DoesNotUpdateExistingOptionNames()
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
            var tagUpdate = new TagOptionEntity
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
            tag.UpdateTagOptions(new[] { tagUpdate }, out var countUpdated, updateNames: false);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(0, countUpdated);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(tagName, onlyOption.Name);
        }

        [Fact]
        public void UpdateTagOptions_WithUpdateNamesTrue_UpdatesExistingOptionNames()
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
            var tagUpdate = new TagOptionEntity
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
            tag.UpdateTagOptions(new[] { tagUpdate }, out var countUpdated, updateNames: true);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(1, countUpdated);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(newTagName, onlyOption.Name);
        }

        [Fact]
        public void UpdateTagOptions_WithUpdateNamesDefault_DoesNotUpdateExistingOptionNames()
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
            var tagUpdate = new TagOptionEntity
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
            tag.UpdateTagOptions(new[] { tagUpdate }, out var countUpdated);

            //Assert
            var onlyOption = tag.Options.SingleOrDefault();
            Assert.NotNull(onlyOption);
            Assert.Equal(0, countUpdated);
            Assert.Equal(tagId, onlyOption.Value);
            Assert.Equal(tagName, onlyOption.Name);
        }

        [Fact]
        public void UpdateTagOptions_WithDisableDeletedFalse_DoesNotDisableDeletedOptions()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var existingTagOption1 = new TagOptionModel
            {
                Value = tagId,
                Name = tagName,
                IsEnabled = true
            };
            var existingTagOption2 = new TagOptionModel
            {
                Value = tagId + " (2)",
                Name = tagName + " (2)",
                IsEnabled = true
            };
            var tagUpdate = new TagOptionEntity
            {
                Id = tagId,
                Name = tagName
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    existingTagOption1,
                    existingTagOption2
                }
            };

            //Act
            tag.UpdateTagOptions(new[] { tagUpdate }, out var countUpdated, disableDeleted: false);

            //Assert
            Assert.Equal(0, countUpdated);
            Assert.Equal(2, tag.Options.Count);
            Assert.True(existingTagOption1.IsEnabled);
            Assert.True(existingTagOption2.IsEnabled);
        }

        [Fact]
        public void UpdateTagOptions_WithDisableDeletedTrue_DisablesDeletedOptions()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var existingTagOption1 = new TagOptionModel
            {
                Value = tagId,
                Name = tagName,
                IsEnabled = true
            };
            var existingTagOption2 = new TagOptionModel
            {
                Value = tagId + " (2)",
                Name = tagName + " (2)",
                IsEnabled = true
            };
            var tagUpdate = new TagOptionEntity
            {
                Id = tagId,
                Name = tagName
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    existingTagOption1,
                    existingTagOption2
                }
            };

            //Act
            tag.UpdateTagOptions(new[] { tagUpdate }, out var countUpdated, disableDeleted: true);

            //Assert
            Assert.Equal(1, countUpdated);
            Assert.Equal(2, tag.Options.Count);
            Assert.True(existingTagOption1.IsEnabled);
            Assert.False(existingTagOption2.IsEnabled);
        }

        [Fact]
        public void UpdateTagOptions_WithDisableDeletedDefault_DisablesDeletedOptions()
        {
            //Arrange
            var tagId = Guid.NewGuid().ToString();
            var tagName = "Test Option";
            var existingTagOption1 = new TagOptionModel
            {
                Value = tagId,
                Name = tagName,
                IsEnabled = true
            };
            var existingTagOption2 = new TagOptionModel
            {
                Value = tagId + " (2)",
                Name = tagName + " (2)",
                IsEnabled = true
            };
            var tagUpdate = new TagOptionEntity
            {
                Id = tagId,
                Name = tagName
            };
            var tag = new TagDropdownDataModel
            {
                Name = "Test Tag",
                Options = new List<TagOptionModel>
                {
                    existingTagOption1,
                    existingTagOption2
                }
            };

            //Act
            tag.UpdateTagOptions(new[] { tagUpdate }, out var countUpdated);

            //Assert
            Assert.Equal(1, countUpdated);
            Assert.Equal(2, tag.Options.Count);
            Assert.True(existingTagOption1.IsEnabled);
            Assert.False(existingTagOption2.IsEnabled);
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
