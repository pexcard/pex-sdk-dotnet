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
        /// <summary>Invoice whose rejected payment this line re-bills; null for an ordinary line. Not part of InvoiceAmount when it is this invoice.</summary>
        public int? SourceInvoiceId { get; set; }
    }
}
