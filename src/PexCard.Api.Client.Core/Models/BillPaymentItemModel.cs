using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentItemModel
    {
        public int PaymentId { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public PaymentStatusTrigger PaymentStatusTrigger { get; set; }

        public int? PaymentTransferId { get; set; }

        public decimal Amount { get; set; }

        public long? PayeeBankAccountId { get; set; }

        public long? PayeeUserId { get; set; }

        public DateTimeOffset? PayoutDate { get; set; }

        public DateTimeOffset? OutboundAchCreationDate { get; set; }

        public DateTimeOffset? ExpectedPaymentDate { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }
    }
}
