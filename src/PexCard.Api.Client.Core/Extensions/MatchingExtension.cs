using Microsoft.Extensions.Logging;
using PexCard.Api.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PexCard.Api.Client.Core.Extensions
{
    public static class MatchingExtension
    {
        public static T MatchEntityByName<T>(this IEnumerable<T> entities, string name, char delimiter = default(char)) where T : IMatchableEntity
        {
            if (string.IsNullOrEmpty(name)) return default(T);

            List<T> matchedEntities;
            if (delimiter == default(char))
            {
                matchedEntities = entities.Where(x => string.Equals(x.EntityName, name, StringComparison.InvariantCultureIgnoreCase)).ToList();
                return matchedEntities.FirstOrDefault();
            }

            matchedEntities = entities
                .Where(x => string.Equals(x.EntityName, name, StringComparison.InvariantCultureIgnoreCase) ||
                            x.EntityName?.EndsWith($"{delimiter}{name}", StringComparison.InvariantCultureIgnoreCase) == true)
                .ToList();

            if (!matchedEntities.Any()) return default(T);

            if (matchedEntities.Count == 1)
            {
                return matchedEntities.First();
            }

            var exactlyMatchedEntities = matchedEntities
                .Where(x => string.Equals(x.EntityName, name, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (exactlyMatchedEntities.Count == 1)
            {
                return exactlyMatchedEntities.First();
            }

            // Try to match entity name on the deepest chunk with PEX tag option name
            // e.g.
            // PEX tag option name is "One".
            // Entities are following: "1 One", "2 Expense:One" and "3 Expense:Category:One"
            // We should match "3 Expense:Category:One" as entity
            var groupedEntities = matchedEntities
                .GroupBy(c => c.EntityName?.Count(x => x == delimiter))
                .ToList();
            var maxKey = groupedEntities.Max(c => c.Key);

            return groupedEntities.First(e => e.Key == maxKey).First();
        }

        public static T FindMatchingEntity<T>(this IEnumerable<T> entities, string value, string name, char entityNameHierarchyDelimiter = default, ILogger logger = default) where T : class, IMatchableEntity
        {
            if (entities == null)
            {
                return null;
            }

            logger?.LogInformation($"Searching {entities.Count()} entities for a match using using value arg '{value}' and name arg '{name}'.");

            T entityIdMatchedByValue = default;
            T entityNameMatchedByName = default;
            T entityNameMatchedByValue = default;
            T entityNameMatchedByNameHeirarchy = default;
            T entityNameMatchedByValueHeirarchy = default;

            int entityNameMatchedByNameHierarchyLevel = default;
            int entityNameMatchedByValueHierarchyLevel = default;

            foreach (T entity in entities)
            {
                logger?.LogDebug($"Attemping to match entity '{entity.EntityName}' ({entity.EntityId}) using value arg '{value}' and name arg '{name}'.");

                if (string.Equals(entity.EntityId, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    // if there is an exact match on entity id from value arg, stop searching. we have an exact match.
                    logger?.LogDebug($"Matched entity '{entity.EntityName}' by entity-id and value arg '{value}'. Done searching.");
                    entityIdMatchedByValue = entity;
                    break;
                }

                if (string.Equals(entity.EntityName, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    // if there is an exact match on entity name from name arg, use it unless we find an better match later
                    logger?.LogDebug($"Matched entity '{entity.EntityName}' by entity-name and name arg '{name}'.");
                    entityNameMatchedByName = entity;
                    continue;
                }

                if (string.Equals(entity.EntityName, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    // if there is an exact match on entity name from value arg, use it unless we find an better match later
                    logger?.LogDebug($"Matched entity '{entity.EntityName}' by entity-name and value arg '{value}'.");
                    entityNameMatchedByValue = entity;
                    continue;
                }

                if (entityNameHierarchyDelimiter != default && entity.EntityName?.EndsWith($"{entityNameHierarchyDelimiter}{name}", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    // if there is a match on a child entity name from name arg, use the most specific one unless we find an better match later
                    var currentHierarchyLevel = entity.EntityName.Count(x => x == entityNameHierarchyDelimiter);
                    logger?.LogDebug($"Matched entity '{entity.EntityName}' by entity-name and name arg '{name}' using delimiter matching ('{entityNameHierarchyDelimiter}') on {currentHierarchyLevel} levels.");

                    if (currentHierarchyLevel > entityNameMatchedByNameHierarchyLevel)
                    {
                        entityNameMatchedByNameHeirarchy = entity;
                        entityNameMatchedByNameHierarchyLevel = currentHierarchyLevel;
                    }
                }

                if (entityNameHierarchyDelimiter != default && entity.EntityName?.EndsWith($"{entityNameHierarchyDelimiter}{value}", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    // if there is a match on a child entity name from value arg, use the most specific one unless we find an better match later
                    var currentHierarchyLevel = entity.EntityName.Count(x => x == entityNameHierarchyDelimiter);
                    logger?.LogDebug($"Matched entity '{entity.EntityName}' by entity-name and value arg '{value}' using delimiter matching ('{entityNameHierarchyDelimiter}') on {currentHierarchyLevel} levels.");

                    if (currentHierarchyLevel > entityNameMatchedByValueHierarchyLevel)
                    {
                        entityNameMatchedByValueHeirarchy = entity;
                        entityNameMatchedByValueHierarchyLevel = currentHierarchyLevel;
                    }
                }
            }

            // order of matching fallbacks:
            // - if we matched entity id by value arg
            // - if we matched by entity name from name arg
            // - if we matched by entity name from value arg
            // - if we matched by entity name from name arg using heirarchy+delimiter
            // - if we matched by entity name from value arg using heirarchy+delimiter

            return entityIdMatchedByValue ?? entityNameMatchedByName ?? entityNameMatchedByValue ?? entityNameMatchedByNameHeirarchy ?? entityNameMatchedByValueHeirarchy;
        }
    }
}
