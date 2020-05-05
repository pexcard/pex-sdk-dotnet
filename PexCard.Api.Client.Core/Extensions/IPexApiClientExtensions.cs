using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace PexCard.Api.Client.Core.Extensions
{
    public static class IPexApiClientExtensions
    {
        public static async Task<Dictionary<long, List<AllocationTagValue>>> GetTagAllocations(
            this IPexApiClient pexApiClient, 
            string externalToken, 
            List<TransactionModel> transactions, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pexApiClient is null)
            {
                throw new ArgumentNullException(nameof(pexApiClient));
            }

            if (externalToken is null)
            {
                throw new ArgumentNullException(nameof(externalToken));
            }

            if (transactions is null)
            {
                throw new ArgumentNullException(nameof(transactions));
            }

            var result = new Dictionary<long, List<AllocationTagValue>>();

            var tagDefinitions = new List<TagDetailsModel>();
            if (transactions.Any(t => t.TransactionTags?.Tags != null))
            {
                //Only get the tag definitions if we see tags on the transactions.
                //If a businesses uses tags for some time then disables them, this will throw an exception.
                tagDefinitions.AddRange(await pexApiClient.GetTags(externalToken, cancellationToken));
            }

            Dictionary<string, TagDetailsModel> hashedTagDefinitions = tagDefinitions.ToDictionary(key => key.Id, value => value);

            foreach (TransactionModel transaction in transactions)
            {
                var allocations = new List<AllocationTagValue>();

                var defaultAllocation = new AllocationTagValue { TagId = null, Allocation = new List<TagValueItem>(), Amount = Math.Abs(transaction.TransactionAmount), };
                if (transaction.TransactionTags?.Tags != null)
                {
                    foreach (TransactionTagModel tag in transaction.TransactionTags.Tags)
                    {
                        if (!hashedTagDefinitions.TryGetValue(tag.FieldId, out TagDetailsModel tagDefinition))
                        {
                            throw new KeyNotFoundException($"Unable to find tag defintion for {nameof(TagDetailsModel.Id)} '{tag.FieldId}'.");
                        }

                        if (tagDefinition.Type == CustomFieldType.Allocation)
                        {
                            //Split tags
                            var allocation = JsonConvert.DeserializeObject<AllocationTagValue>(tag.Value.ToString());
                            allocations.Add(allocation);
                        }
                        else
                        {
                            //Non-split tags
                            defaultAllocation.Allocation.Add(new TagValueItem { TagId = tag.FieldId, Value = tag.Value, });
                        }
                    }
                }

                if (!allocations.Any())
                {
                    //Ensure each transaction has at least one allocation
                    allocations.Add(defaultAllocation);
                }

                result.Add(transaction.TransactionId, allocations);
            }

            return result;
        }
    }
}
