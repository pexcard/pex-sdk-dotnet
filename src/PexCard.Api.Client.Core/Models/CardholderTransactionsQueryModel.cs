using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class CardholderTransactionsQueryModel : TransactionsQueryModel
    {
        public IList<int> OnlyCategoryIds { get; set; }

        public IList<int> NotCategoryIds { get; set; }
    }
}
