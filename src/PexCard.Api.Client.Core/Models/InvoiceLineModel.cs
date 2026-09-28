using System;

namespace PexCard.Api.Client.Core.Models
{
    public class InvoiceLineModel
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime DateUpdated { get; set; }
        public long? TransactionId { get; set; }
        public string LineType { get; set; }
        public int? SourceInvoiceId { get; set; }
    }
}
