using PexCard.Api.Client.Core.Enums;
using System;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentModel
    {
        public int PaymentId { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public PaymentStatusTrigger PaymentStatusTrigger { get; set; }

        public int PaymentTransferId { get; set; }

        public decimal Amount { get; set; }

        public int PlatformBusinessAccountId { get; set; }

        public long PayeeBankAccountId { get; set; }

        public long PayeeUserId { get; set; }

        public int? ProcessorRequestId { get; set; }

        public DateTimeOffset? PayoutDate { get; set; }
    }
}
