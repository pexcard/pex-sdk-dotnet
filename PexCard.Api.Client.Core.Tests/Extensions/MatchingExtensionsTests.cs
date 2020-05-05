using PexCard.Api.Client.Core.Extensions;
using PexCard.Api.Client.Core.Interfaces;
using System.Collections.Generic;
using Xunit;

namespace PexCard.Api.Client.Core.Tests.Extensions
{
    public class MatchingExtensionsTests
    {
        [Fact]
        public void MatchEntityByName_ReturnsEntity_WhenMatchingByNameEquals()
        {
            //Arrange
            var testEntity = new TestEntity { Id = "500", Name = "OFFICE ADMINISTRATION:Administrator Meetings", };
            IEnumerable<IMatchableEntity> entities = new[] 
            {
                testEntity,
            };

            //Act
            IMatchableEntity matchedEntity = entities.MatchEntityByName(
                "OFFICE ADMINISTRATION:Administrator Meetings", 
                ':');

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
