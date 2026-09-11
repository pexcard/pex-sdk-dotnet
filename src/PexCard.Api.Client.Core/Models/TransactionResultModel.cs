using System;
using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TransactionResultModel
    {
        public int BusinessId { get; set; }

        public int? CardholderId { get; set; }

        public long TransactionId { get; set; }

        public long? NetworkTransactionId { get; set; }

        public long? AuthTransactionId { get; set; }

        public long? RelationshipId { get; set; }

        public DateTime TransactionTime { get; set; }

        public DateTime? TransactionAuthTime { get; set; }

        public DateTime? TransactionHoldTime { get; set; }

        public bool IsPending { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyDesc { get; set; }

        public string CurrencyCode { get; set; }

        public decimal? SourceAmount { get; set; }

        public string SourceCurrencyDesc { get; set; }

        public string SourceCurrencyCode { get; set; }

        public TransactionResultCategoryModel Category { get; set; }

        public TransactionResultMerchantModel Merchant { get; set; }

        public TransactionResultAccountModel Account { get; set; }

        public string Approval { get; set; }

        public bool HasAttachments { get; set; }

        public IList<TransactionResultNoteModel> Notes { get; set; } = new List<TransactionResultNoteModel>();

        public IList<TransactionResultTagModel> Tags { get; set; } = new List<TransactionResultTagModel>();
    }
}
