using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PexCard.Api.Client.Core.Interfaces;

namespace PexCard.Api.Client.Core.Extensions
{
    public static class MatchingExtension
    {
        public static T MatchEntityByName<T>(
            this IEnumerable<T> entities,
            string entityName,
            char delimiter = default(char)) where T : IMatchableEntity
        {
            if (string.IsNullOrEmpty(entityName)) return default(T);

            List<T> matchedEntities;
            if (delimiter == default(char))
            {
                matchedEntities = entities
                    .Where(x => x.EntityName.Equals(entityName,
                        StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
                return matchedEntities.FirstOrDefault();
            }

            matchedEntities = entities
                .Where(x => x.EntityName.Equals(entityName,
                                StringComparison.InvariantCultureIgnoreCase) ||
                            x.EntityName.EndsWith($"{delimiter}{entityName}",
                                StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (!matchedEntities.Any()) return default(T);

            if (matchedEntities.Count == 1)
            {
                return matchedEntities.First();
            }

            var exactlyMatchedEntities = matchedEntities
                .Where(x => x.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            if (exactlyMatchedEntities.Count == 1)
            {
                return exactlyMatchedEntities.First();
            }

            //Try to match entity name on the deepest chunk with PEX tag option name
            // e.g.
            // PEX tag option name is "One".
            // Entities are following: "1 One", "2 Expense:One" and "3 Expense:Category:One"
            // We should match "3 Expense:Category:One" as entity
            var groupedEntities = matchedEntities
                .GroupBy(c => c.EntityName.Count(x => x == delimiter))
                .ToList();
            var maxKey = groupedEntities.Max(c => c.Key);
            return groupedEntities.First(e => e.Key == maxKey).First();
        }

        public static T FindMatchingEntity<T>(
            this IEnumerable<T> entities,
            string entityValue,
            string entityName,
            char entityNameHierarchyDelimiter = default,
            ILogger logger = default)
                where T : class, IMatchableEntity
        {
            if (entities == null)
            {
                return null;
            }

            logger?.LogInformation($"Searching for a match for value '{entityValue}' / name '{entityName}' in {entities.Count()} entities");

            T entityMatchedByValue = default;
            T entityMatchedByName = default;
            T entityMatchedByNameHierarchy = default;
            int entityMatchedByNameHierarchyLevel = default;
            foreach (T entity in entities)
            {
                logger?.LogDebug($"Searching against entity: '{entity.EntityId}' / '{entity.EntityName}'");

                if (entity.EntityId.Equals(entityValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    //If there is an exact match on Id, use the first match.
                    logger?.LogDebug($"Matched on value");
                    entityMatchedByValue = entity;
                    break;
                }

                if (entity.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //If there is an exact match on Name, use it unless we find an Id match later.
                    logger?.LogDebug($"Matched on name");
                    entityMatchedByName = entity;
                    continue;
                }

                if (entityNameHierarchyDelimiter != default
                    && entity.EntityName.EndsWith($"{entityNameHierarchyDelimiter}{entityName}", StringComparison.InvariantCultureIgnoreCase))
                {
                    //If there is a match on a child entity name, use the most specific one unless there is an exact match on Id or Name later.
                    var currentHierarchyLevel = entity.EntityName.Count(x => x == entityNameHierarchyDelimiter);
                    logger?.LogDebug($"Matched on name (delimiter) ({currentHierarchyLevel} levels)");

                    if (currentHierarchyLevel > entityMatchedByNameHierarchyLevel)
                    {
                        entityMatchedByNameHierarchy = entity;
                        entityMatchedByNameHierarchyLevel = currentHierarchyLevel;
                    }
                }
            }

            return entityMatchedByValue ?? entityMatchedByName ?? entityMatchedByNameHierarchy;
        }
    }
}
