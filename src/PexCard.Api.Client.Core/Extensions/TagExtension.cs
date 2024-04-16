using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PexCard.Api.Client.Core.Interfaces;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Core.Extensions
{
    public static class TagExtension
    {
        public static string GetTagOptionName(this TransactionTagModel transactionTag, IEnumerable<TagOptionModel> tagOptions)
        {
            if (transactionTag == null) return null;

            var result = tagOptions
                ?.FirstOrDefault(o => o.Value.Equals(transactionTag.Value))
                ?.Name;
            return result;
        }

        public static string GetTagOptionName(this TagValueItem transactionTag, IEnumerable<TagOptionModel> tagOptions)
        {
            if (transactionTag == null) return null;

            var result = tagOptions
                ?.FirstOrDefault(o => o.Value.Equals(transactionTag.Value))
                ?.Name;
            return result;
        }

        public static void UpdateTagOptions(this TagDropdownDataModel tag, IEnumerable<IMatchableEntity> entities, out int countUpdated, bool updateNames = false, bool disableDeleted = true, bool handleDuplicates = true)
        {
            var updateCounts = new Dictionary<string, bool>();

            countUpdated = 0;

            if (entities != null)
            {
                var entitiesList = entities.ToList();
                entitiesList.ValidateTagEntities();

                // upsert tag options
                foreach (var entity in entitiesList)
                {
                    var existingOption = tag.Options.Find(item => string.Equals(item.Value, entity.EntityId, StringComparison.InvariantCultureIgnoreCase));
                    if (existingOption != null)
                    {
                        // existing entity

                        // update names
                        if (updateNames && !string.Equals(existingOption.Name, entity.EntityName))
                        {
                            existingOption.Name = entity.EntityName;

                            updateCounts[existingOption.Value] = true;
                        }

                        // append asterisks to duplicates to prevent not syncing
                        if (handleDuplicates)
                        {
                            AppendAsterisksToDuplicates(tag, existingOption);
                        }
                    }
                    else
                    {
                        // new entity

                        // add new option
                        var newOption = new TagOptionModel
                        {
                            IsEnabled = true,
                            Name = entity.EntityName,
                            Value = entity.EntityId
                        };
                        tag.Options.Add(newOption);

                        updateCounts[newOption.Value] = true;

                        // append asterisks to duplicates to prevent not syncing
                        if (handleDuplicates)
                        {
                            AppendAsterisksToDuplicates(tag, newOption);
                        }
                    }
                }

                // disable deleted tag options
                if (disableDeleted)
                {
                    foreach (var option in tag.Options)
                    {
                        var entity = entitiesList.Find(item => string.Equals(item.EntityId, option.Value, StringComparison.InvariantCultureIgnoreCase));
                        if (entity == null)
                        {
                            option.IsEnabled = false;

                            updateCounts[option.Value] = true;
                        }
                    }
                }
            }

            // at least one option must be enabled.
            if (tag.Options.All(o => !o.IsEnabled))
            {
                var firstOption = tag.Options.FirstOrDefault();
                if (firstOption != null)
                {
                    firstOption.IsEnabled = true;
                    updateCounts[firstOption.Value] = true;
                }
            }

            ValidateDropdownTag(tag);

            countUpdated = updateCounts.Count;
        }

        public static void ValidateTagEntities(this IList<IMatchableEntity> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var duplicateTagOptionNamesOrValues = options.Where(x => options.Count(y => string.Equals(y.EntityName, x.EntityName, StringComparison.InvariantCultureIgnoreCase)) > 1 || options.Count(y => string.Equals(y.EntityId, x.EntityId, StringComparison.InvariantCultureIgnoreCase)) > 1).ToList();
            if (duplicateTagOptionNamesOrValues.Any())
            {
                throw new DataException($"Duplicate input entity names and/or ids: {string.Join(", ", duplicateTagOptionNamesOrValues.Select(x => $"[EntityName: '{x.EntityName}', EntityId: '{x.EntityId}']"))}.");
            }
        }

        public static void ValidateDropdownTag(this TagDropdownDataModel tag)
        {
            if (tag is null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var duplicateTagOptionNamesOrValues = tag.Options.Where(x => tag.Options.Count(y => string.Equals(y.Name, x.Name, StringComparison.InvariantCultureIgnoreCase)) > 1 || tag.Options.Count(y => string.Equals(y.Value, x.Value, StringComparison.InvariantCultureIgnoreCase)) > 1).ToList();
            if (duplicateTagOptionNamesOrValues.Any())
            {
                throw new DataException($"Tag '{tag.Name}' has duplicate tag option names and/or values: {string.Join(", ", duplicateTagOptionNamesOrValues.Select(x => $"[Name: '{x.Name}', Value: '{x.Value}']"))}.");
            }

            if (!tag.Options.Any(x => x.IsEnabled))
            {
                throw new DataException($"Tag '{tag.Name}' has no options enabled. At least one option must be enabled.");
            }
        }

        private static void AppendAsterisksToDuplicates(TagDropdownDataModel tag, TagOptionModel newOption)
        {
            var duplicate = tag.Options.FirstOrDefault(existingOption => existingOption.Value != newOption.Value && existingOption.Name.Equals(newOption.Name, StringComparison.InvariantCultureIgnoreCase));
            if (duplicate != null)
            {
                duplicate.Name = $"{duplicate.Name}*";
                AppendAsterisksToDuplicates(tag, newOption);
            }
        }
    }
}
