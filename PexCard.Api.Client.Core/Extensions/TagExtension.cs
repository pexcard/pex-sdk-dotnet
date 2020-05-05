using System;
using System.Collections.Generic;
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
            var entitiesList = entities.ToList();
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

        private static void ProcessTagOption(TagDropdownDataModel tag, string tagOptionValue, string tagOptionName)
        {
            var option = tag.Options.Find(item =>
                item.Value.Equals(tagOptionValue, StringComparison.InvariantCultureIgnoreCase));
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
