using System;

namespace PexCard.Api.Client.Core.Models
{
    public class BillSearchItemModel
    {
        public int BillId { get; set; }
        public string BillRefNo { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset Created { get; set; }
        public long CreatedByUserId { get; set; }
        public string CreatedByUser { get; set; }
        public long PayeePexId { get; set; }
        public string PayeeName { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string PaymentRequestStatus { get; set; }
        public string PaymentRequestStatusTrigger { get; set; }
    }
}
