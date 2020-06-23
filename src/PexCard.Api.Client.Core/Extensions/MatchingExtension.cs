using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
