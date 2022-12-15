using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class BusinessDetailsModel
    {
        public int BusinessAccountId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAccountStatus { get; set; }
        public decimal BusinessAccountBalance { get; set; }
        public decimal PendingTransferAmount { get; set; }
        public List<CardholderAccountModel> CHAccountList { get; set; }
        public string BusinessAccountNumber { get; set; }
    }
}