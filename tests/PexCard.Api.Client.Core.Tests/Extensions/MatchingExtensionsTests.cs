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
            var testEntity = new TestEntity
            {
                Id = "500",
                Name = entityName
            };
            var entities = new[]
            {
                testEntity
            };

            //Act
            var matchedEntity = entities.MatchEntityByName(entityName, ':');

            //Assert
            Assert.Equal(testEntity, matchedEntity);
        }

        [Fact]
        public void MatchEntityByName_ReturnsEntity_WhenThereIsMatchingChild()
        {
            const string entityName = "Administrator Meetings";
            //Arrange
            var testEntity = new TestEntity
            {
                Id = "500",
                Name = entityName
            };
            var entities = new[]
            {
                testEntity,
                new TestEntity
                {
                    Id = "510",
                    Name = $"OFFICE ADMINISTRATION:{entityName}"
                }
            };

            //Act
            var matchedEntity = entities.MatchEntityByName(entityName, ':');

            //Assert
            Assert.Equal(testEntity, matchedEntity);
        }

        [Fact]
        public void MatchEntityByName_ReturnsEntity_WhenThereAreMultipleMatchingChildren()
        {
            const string entityName = "Administrator Meetings";
            //Arrange
            var testEntity = new TestEntity
            {
                Id = "500",
                Name = $"MANAGEMENT:OFFICE ADMINISTRATION:{entityName}"
            };
            var entities = new[]
            {
                new TestEntity
                {
                    Id = "510",
                    Name = $"OFFICE ADMINISTRATION:{entityName}"
                },
                testEntity
            };

            //Act
            var matchedEntity = entities.MatchEntityByName(entityName, ':');

            //Assert
            Assert.Equal(testEntity, matchedEntity);
        }

        private class TestEntity : IMatchableEntity
        {
            public string Id { get; set; }
            public string Name { get; set; }

            string IMatchableEntity.EntityId => Id;

            string IMatchableEntity.EntityName => Name;
        }
    }
}
