using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class UpsertTransactionTagsModel
    {
        public string ConcurrencyKey { get; set; }

        public List<UpsertTagValueModel> Values { get; set; } = new List<UpsertTagValueModel>();
    }
}
