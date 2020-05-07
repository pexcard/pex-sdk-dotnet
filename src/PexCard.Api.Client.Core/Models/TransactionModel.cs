using System;
using System.Collections.Generic;
using System.Linq;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class TransactionModel
    {
        public long TransactionId { get; set; }
        public int AcctId { get; set; }
        public DateTime TransactionTime { get; set; }
        public DateTime? MerchantLocalTime { get; set; }
        public DateTime? HoldTime { get; set; }
        public DateTime? SettlementTime { get; set; }
        public long AuthTransactionId { get; set; }
        public decimal TransactionAmount { get; set; }  // settlement amount or pending amount 
        public decimal PaddingAmount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public List<TransactionNoteModel> TransactionNotes { get; set; }
        public bool IsPending { get; set; }
        public bool IsDecline { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCity { get; set; }
        public string MerchantState { get; set; }
        public string MerchantZip { get; set; }
        public string MerchantCountry { get; set; }
        public string MCCCode { get; set; }
        public int TransferToOrFromAccountId { get; set; }
        public string AuthIdentityResponseCode { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public long? NetworkTransactionId { get; set; }
        public string SourceCurrencyCodeDescription { get; set; }
        public string SourceCurrencyNumericCode { get; set; }
        public decimal? SourceAmount { get; set; }
        public TransactionTagsModel TransactionTags { get; set; }
        public string MetadataApprovalStatus { get; set; }
        public string TransactionTypeCategory { get; set; }

        public TransactionTagModel GetTagValue(string tagId)
        {
            var tagValue = TransactionTags?.Tags?.FirstOrDefault(t =>
                t.FieldId.Equals(tagId, StringComparison.InvariantCultureIgnoreCase));
            return tagValue;
        }
    }
}