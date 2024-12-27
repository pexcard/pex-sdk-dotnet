using System.Collections.Generic;
using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentTransferModel
    {
        public int PaymentTransferId { get; set; }

        public long UserId { get; set; }

        public DateTimeOffset? ScheduleOnOrAfterDate { get; set; }

        public int PayerPlatformBusinessAccountId { get; set; }

        public long PayerBankAccountId { get; set; }

        public long? ProcessorRequestId { get; set; }

        public long? ApprovalId { get; set; }

        public PaymentTransferStatus PaymentTransferStatus { get; set; }

        public PaymentTransferStatusTrigger PaymentTransferStatusTrigger { get; set; }

        public List<PaymentRequestXPaymentTransferModel> PaymentRequests { get; set; }

        public long? UpdatedByUserId { get; set; }

        public int NoOfPaymentRequests { get; set; }

        public decimal Amount { get; set; }

        public bool? IsTransferToPayablesInitiated { get; set; }
    }
}
