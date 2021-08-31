using PexCard.Api.Client.Core.Extensions;
using PexCard.Api.Client.Core.Interfaces;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Extensions
{
    public class MatchingExtensionsTests
    {
        [Fact]
        public void MatchEntityByName_ReturnsEntity_WhenMatchingByNameEquals()
        {
            const string entityName = "OFFICE ADMINISTRATION:Administrator Meetings";
            //Arrange
            var testEntity = new TestTagOption
            {
                OptionValue = "500",
                OptionName = entityName
            };
            var entities = new[]
            {
                testEntity
            };

            //Act
            var matchedEntity = entities.MatchEntityByName(entityName, ':');
            var newMatchedEntity = entities.FindMatchingEntity("123", entityName, ':');

            //Assert
            Assert.Equal(testEntity, matchedEntity);
            Assert.Equal(testEntity, newMatchedEntity);
        }

        [Fact]
        public void MatchEntityByName_ReturnsEntity_WhenThereIsMatchingChild()
        {
            const string entityName = "Administrator Meetings";
            //Arrange
            var testEntity = new TestTagOption
            {
                OptionValue = "500",
                OptionName = entityName
            };
            var entities = new[]
            {
                testEntity,
                new TestTagOption
                {
                    OptionValue = "510",
                    OptionName = $"OFFICE ADMINISTRATION:{entityName}"
                }
            };

            //Act
            var matchedEntity = entities.MatchEntityByName(entityName, ':');
            var newMatchedEntity = entities.FindMatchingEntity("123", entityName, ':');

            //Assert
            Assert.Equal(testEntity, matchedEntity);
            Assert.Equal(testEntity, newMatchedEntity);
        }

        [Fact]
        public void MatchEntityByName_ReturnsEntity_WhenThereAreMultipleMatchingChildren()
        {
            const string entityName = "Administrator Meetings";
            //Arrange
            var testEntity = new TestTagOption
            {
                OptionValue = "500",
                OptionName = $"MANAGEMENT:OFFICE ADMINISTRATION:{entityName}"
            };
            var entities = new[]
            {
                new TestTagOption
                {
                    OptionValue = "510",
                    OptionName = $"OFFICE ADMINISTRATION:{entityName}"
                },
                testEntity
            };

            //Act
            var matchedEntity = entities.MatchEntityByName(entityName, ':');
            var newMatchedEntity = entities.FindMatchingEntity("123", entityName, ':');

            //Assert
            Assert.Equal(testEntity, matchedEntity);
            Assert.Equal(testEntity, newMatchedEntity);
        }

        [Fact]
        public void FindMatchingEntity_MatchesOnId_WhenNameAlsoMatches()
        {
            const string entityName = "Administrator Meetings";
            //Arrange
            var testEntity = new TestTagOption
            {
                OptionValue = "500",
                OptionName = $"MANAGEMENT:OFFICE ADMINISTRATION:{entityName}"
            };
            var entities = new[]
            {
                new TestTagOption
                {
                    OptionValue = "510",
                    OptionName = $"OFFICE ADMINISTRATION:{entityName}"
                },
                testEntity
            };

            //Act
            var newMatchedEntity = entities.FindMatchingEntity("500", $"OFFICE ADMINISTRATION:{entityName}", ':');

            //Assert
            Assert.Equal(testEntity, newMatchedEntity);
        }

        [Fact]
        public void FindMatchingEntity_MatchesOnId_WhenNameAndHierarchyAlsoMatches()
        {
            const string entityName = "Administrator Meetings";
            //Arrange
            var testEntity = new TestTagOption
            {
                OptionValue = "500",
                OptionName = $"MANAGEMENT:OFFICE ADMINISTRATION:{entityName}"
            };
            var entities = new[]
            {
                new TestTagOption
                {
                    OptionValue = "510",
                    OptionName = $"OFFICE ADMINISTRATION:{entityName}"
                },
                                new TestTagOption
                {
                    OptionValue = "511",
                    OptionName = entityName
                },
                testEntity
            };

            //Act
            var newMatchedEntity = entities.FindMatchingEntity("500", entityName, ':');

            //Assert
            Assert.Equal(testEntity, newMatchedEntity);
        }

        private class TestTagOption : IMatchableEntity
        {
            public string OptionValue { get; set; }
            public string OptionName { get; set; }

            string IMatchableEntity.EntityId => OptionValue;

            string IMatchableEntity.EntityName => OptionName;
        }
    }
}
