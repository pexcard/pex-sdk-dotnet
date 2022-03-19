using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class UpsertTransactionTagsModel
    {
        public List<UpsertTagValueModel> Values { get; set; }
    }
}
