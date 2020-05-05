using System.Collections.Generic;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Models
{
    internal class TransactionListModel
    {
        public List<TransactionModel> TransactionList { get; set; }
        public PageInfoModel Pages { get; set; }
    }
}