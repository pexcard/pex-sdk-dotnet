using System;
using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentDetailsResponseModel
    {
        public int BillId { get; set; }

        public string MerchantName { get; set; }

        public decimal Amount { get; set; }

        public PaymentRequestStatus PaymentRequestStatus { get; set; }

        public PaymentRequestStatusTrigger PaymentRequestStatusTrigger { get; set; }

        public PaymentRequestType PaymentRequestType { get; set; }

        public string PayeeFundsDestinationType { get; set; }

        public DateTimeOffset? PurchaseDate { get; set; }

        public DateTimeOffset? PayoutDate { get; set; }

        public long? MetadataId { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public bool HasPaymentError { get; set; }

        public long? PayeePexId { get; set; }

        public bool IsUserApprovalRestricted { get; set; }

        public bool IsUserInApprovalRoute { get; set; }

        public List<AttachmentLinkModel> Attachments { get; set; }

        public TagsModel Tags { get; set; }

        public List<TransactionNoteModel> Notes { get; set; }
    }
}
