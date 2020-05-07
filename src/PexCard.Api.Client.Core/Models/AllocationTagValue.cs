using System;
using System.Collections.Generic;
using System.Linq;

namespace PexCard.Api.Client.Core.Models
{
    public class AllocationTagValue
    {
        public string TagId { get; set; }
        public decimal Amount { get; set; }
        public List<TagValueItem> Allocation { get; set; }

        public TagValueItem GetTagValue(string tagId)
        {
            var tagValue = Allocation?.FirstOrDefault(t =>
                t.TagId.Equals(tagId, StringComparison.InvariantCultureIgnoreCase));
            return tagValue;
        }
    }
}