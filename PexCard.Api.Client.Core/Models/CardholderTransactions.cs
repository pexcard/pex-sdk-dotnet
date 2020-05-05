using System.Collections.Generic;
using System.Linq;

namespace PexCard.Api.Client.Core.Models
{
    public class CardholderTransactions : List<TransactionModel>
    {
        public CardholderTransactions(List<TransactionModel> transactions)
        {
            if (transactions?.Any() ?? false)
            {
                AddRange(transactions);
            }
        }
    }
}
