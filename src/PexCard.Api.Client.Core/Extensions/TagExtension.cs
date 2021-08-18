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
        public static string GetTagOptionName(
            this TransactionTagModel transactionTag,
            IEnumerable<TagOptionModel> tagOptions)
        {
            var result = tagOptions
                ?.FirstOrDefault(o => o.Value.Equals(transactionTag.Value))
                ?.Name;
            return result;
        }

        public static string GetTagOptionName(
            this TagValueItem transactionTag,
            IEnumerable<TagOptionModel> tagOptions)
        {
            var result = tagOptions
                ?.FirstOrDefault(o => o.Value.Equals(transactionTag.Value))
                ?.Name;
            return result;
        }

        public static void InitTagOptions(
            this TagDropdownDataModel tag,
            IEnumerable<IMatchableEntity> entities,
            out int syncCount,
            out int removalCount)
        {
            syncCount = 0;
            removalCount = 0;

            if (entities != null)
            {
                var entitiesList = entities.ToList();
                entitiesList.ValidateTagEntities();

                foreach (var entity in entitiesList)
                {
                    var tagOption = tag.Options.FirstOrDefault(o =>
                        o.Value.Equals(entity.EntityId, StringComparison.InvariantCultureIgnoreCase));
                    if (tagOption != null && tagOption.IsEnabled && tagOption.Name.Equals(entity.EntityName))
                    {
                        continue;
                    }

                    ProcessTagOption(tag, entity.EntityId, entity.EntityName);
                    syncCount++;
                }

                foreach (var option in tag.Options)
                {
                    if (entitiesList.All(item =>
                        !item.EntityId.Equals(option.Value, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (option.IsEnabled) removalCount++;
                        DisableTagOption(tag, option.Value);
                    }
                }

                // At least one option should be enabled.
                // So lets enable the first one that in fact should be default option.
                if (tag.Options.All(o => !o.IsEnabled))
                {
                    var firstOption = tag.Options.FirstOrDefault();
                    if (firstOption != null)
                    {
                        firstOption.IsEnabled = true;
                        syncCount++;
                    }
                }
            }

            ValidateDropdownTag(tag);
        }

        public static void UpsertTagOptions(this TagDropdownDataModel tag, IEnumerable<IMatchableEntity> entities, out int syncCount, bool updateNames = true)
        {
            syncCount = 0;

            if (entities != null)
            {
                var entitiesList = entities.ToList();
                entitiesList.ValidateTagEntities();

                foreach (var entity in entitiesList)
                {
                    // we only add new tag options (or update tag option NAMES if updateNames is true) and we do NOT change IsEnabled statuses.
                    var option = tag.Options.Find(item => string.Equals(item.Value, entity.EntityId));
                    if (option != null)
                    {
                        if (updateNames && !string.Equals(option.Name, entity.EntityName))
                        {
                            option.Name = entity.EntityName;
                            syncCount++;
                        }
                    }
                    else
                    {
                        tag.Options.Add(
                            new TagOptionModel
                            {
                                IsEnabled = true,
                                Name = entity.EntityName,
                                Value = entity.EntityId
                            }
                        );
                        syncCount++;
                    }
                }
            }

            // At least one option should be enabled.
            // So lets enable the first one that in fact should be default option.
            if (tag.Options.All(o => !o.IsEnabled))
            {
                var firstOption = tag.Options.FirstOrDefault();
                if (firstOption != null)
                {
                    firstOption.IsEnabled = true;
                    syncCount++;
                }
            }

            ValidateDropdownTag(tag);
        }

        private static void ValidateTagEntities(this IList<IMatchableEntity> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var duplicateTagOptionNamesOrValues = options.Where(x => options.Count(y => string.Equals(y.EntityName, x.EntityName, StringComparison.InvariantCultureIgnoreCase)) > 1 || options.Count(y => string.Equals(y.EntityId, x.EntityId, StringComparison.InvariantCultureIgnoreCase)) > 1).ToList();
            if (duplicateTagOptionNamesOrValues.Any())
            {
                throw new DataException($"Duplicate tag option entity names ids and/or ids: {string.Join(", ", duplicateTagOptionNamesOrValues.Select(x => $"[EntityName: '{x.EntityName}', EntityId: '{x.EntityId}']"))}.");
            }
        }

        private static void ValidateDropdownTag(this TagDropdownDataModel tag)
        {
            if (tag is null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var duplicateTagOptionNamesOrValues = tag.Options.Where(x => tag.Options.Count(y => string.Equals(y.Name, x.Name, StringComparison.InvariantCultureIgnoreCase)) > 1 || tag.Options.Count(y => string.Equals(y.Value, x.Value, StringComparison.InvariantCultureIgnoreCase)) > 1).ToList();
            if (duplicateTagOptionNamesOrValues.Any())
            {
                throw new DataException($"{nameof(TagDropdownDataModel)} '{tag.Name}' has duplicate tag option names and/or values: {string.Join(", ", duplicateTagOptionNamesOrValues.Select(x => $"[Name: '{x.Name}', Value: '{x.Value}']"))}.");
            }

            if (!tag.Options.Any(x => x.IsEnabled))
            {
                throw new DataException($"{nameof(TagDropdownDataModel)} '{tag.Name}' has no options enabled. At least one option must be enabled.");
            }
        }

        private static void ProcessTagOption(TagDropdownDataModel tag, string tagOptionValue, string tagOptionName)
        {
            var option = tag.Options.Find(item => item.Value.Equals(tagOptionValue, StringComparison.InvariantCultureIgnoreCase));
            if (option != null)
            {
                option.IsEnabled = true;
                option.Name = tagOptionName;
            }
            else
            {
                tag.Options.Add(
                    new TagOptionModel
                    {
                        IsEnabled = true,
                        Name = tagOptionName,
                        Value = tagOptionValue
                    }
                );
            }
        }

        private static void DisableTagOption(TagDropdownDataModel tag, string tagOptionValue)
        {
            var option = tag.Options.Find(item =>
                item.Value.Equals(tagOptionValue, StringComparison.InvariantCultureIgnoreCase));
            if (option != null)
            {
                option.IsEnabled = false;
                AppendAsteriskIfNeeded(tag, option);
            }
        }

        private static void AppendAsteriskIfNeeded(TagDropdownDataModel tag, TagOptionModel option)
        {
            if (tag.Options.Any(o =>
                o.Value != option.Value &&
                o.Name.Equals(option.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                option.Name = $"{option.Name}*";
                AppendAsteriskIfNeeded(tag, option);
            }
        }
    }
}
