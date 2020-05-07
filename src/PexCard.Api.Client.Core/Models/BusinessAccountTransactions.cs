using System.Collections.Generic;
using System.Linq;

namespace PexCard.Api.Client.Core.Models
{
    public class BusinessAccountTransactions : List<TransactionModel>
    {
        public BusinessAccountTransactions(List<TransactionModel> transactions)
        {
            if (transactions?.Any() ?? false)
            {
                AddRange(transactions);
            }
        }
    }
}
