using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentModel
    {
        public int BillId { get; set; }

        public string BillRefNo { get; set; }

        public decimal Amount { get; set; }

        public DateTimeOffset Created { get; set; }

        public long CreatedByUserId { get; set; }

        public string CreatedByUser { get; set; }

        public long? PayeePexId { get; set; }

        public string PayeeName { get; set; }

        /// <summary>
        /// Normalized vendor name (payee) from merchant normalization system
        /// </summary>
        public string PayeeNameNormalized { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public PaymentRequestStatus PaymentRequestStatus { get; set; }

        public PaymentRequestStatusTrigger PaymentRequestStatusTrigger { get; set; }
    }
}
