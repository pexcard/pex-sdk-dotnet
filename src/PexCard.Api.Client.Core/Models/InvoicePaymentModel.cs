using PexCard.Api.Client.Core.Enums;
using System;

namespace PexCard.Api.Client.Core.Models
{
    public class InvoicePaymentModel
    {
        public int PaymentId { get; set; }
        public int? RequestId { get; set; }
        public PaymentType Type { get; set; }
        public DateTime DatePaid { get; set; }
        public string Description { get; internal set; }
        public long? PaymentInitiatedByUserId { get; set; }
        public int? CreditCarriedOverFromInvoiceId { get; set; }
        public decimal Amount { get; set; }
        public bool RejectedByBank { get; set; }
    }
}
