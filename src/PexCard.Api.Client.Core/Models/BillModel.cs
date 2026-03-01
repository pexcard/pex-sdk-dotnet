using System;
using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class BillModel
    {
        public int BillId { get; set; }
        public string MerchantName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentRequestStatus { get; set; }
        public string PaymentRequestStatusTrigger { get; set; }
        public string PaymentRequestType { get; set; }
        public string PayeeFundsDestinationType { get; set; }
        public DateTimeOffset PurchaseDate { get; set; }
        public DateTimeOffset? PayoutDate { get; set; }
        public long? MetadataId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public bool HasPaymentError { get; set; }
        public long? PayeePexId { get; set; }
        public BillPaymentDetailsModel BillPayment { get; set; }
        public BillApprovalDetailsModel ApprovalDetails { get; set; }
        public bool IsUserApprovalRestricted { get; set; }
        public bool IsUserInApprovalRoute { get; set; }
        public List<BillAttachmentModel> Attachments { get; set; }
        public BillTagsModel Tags { get; set; }
        public List<BillNoteModel> Notes { get; set; }
    }
}
