using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentRequestModel
    {
        public int PaymentRequestId { get; set; }

        public PaymentRequestType PaymentRequestType { get; set; }

        public int PlatformBusinessAccountId { get; set; }

        public string MerchantName { get; set; }

        public decimal Amount { get; set; }

        public DateTimeOffset PurchaseDate { get; set; }

        public string Note { get; set; }

        public long PayeeBankAccountId { get; set; }

        public string MetadataId { get; set; }

        public long? MetadataRelationId { get; set; }

        public PaymentRequestMetadataModel Metadata { get; set; } = new PaymentRequestMetadataModel();

        public long? ApprovalId { get; set; }

        public PaymentRequestStatus PaymentRequestStatus { get; set; }

        public PaymentRequestStatusTrigger PaymentRequestStatusTrigger { get; set; }

        public int? PaymentId { get; set; }

        public DateTimeOffset? PayoutDate { get; set; }

        public long CreatedByUserId { get; set; }

        public long? UpdatedByUserId { get; set; }
    }
}
