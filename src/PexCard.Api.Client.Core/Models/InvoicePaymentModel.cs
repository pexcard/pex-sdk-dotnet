using PexCard.Api.Client.Core.Enums;
using System;
using System.Collections.Generic;

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
        public int InvoiceId { get; set; }
        public long? PaidTransactionId { get; set; }
        public DateTime? ReplenishmentDelayEndDate { get; set; }
        public string PaymentTrigger { get; set; }
        public string WriteOffReasonCode { get; set; }
        public string RejectionReason { get; set; }
        public Guid? PaymentGroupId { get; set; }
        public decimal? ReleasedAmount { get; set; }
        public string FloatRole { get; set; }
        public decimal? FloatPercentage { get; set; }
        public decimal? FloatGroupAmount { get; set; }
        public List<InvoicePaymentModel> Legs { get; set; } = new List<InvoicePaymentModel>();
        public bool IsPastReturnWindow { get; set; }
    }
}
