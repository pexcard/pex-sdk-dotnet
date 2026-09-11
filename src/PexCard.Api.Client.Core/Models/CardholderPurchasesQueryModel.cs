using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class CardholderPurchasesQueryModel : TransactionsQueryModel
    {
        public bool? Pending { get; set; }

        public string Search { get; set; }

        public IList<TransactionApprovalStatus> Approval { get; set; }
    }
}
