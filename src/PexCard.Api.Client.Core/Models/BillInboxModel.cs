using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class BillInboxModel
    {
        public int BillInboxId { get; set; }

        /// <summary>
        /// One of: Pending, NeedsReview, Approved, Rejected, Converted
        /// </summary>
        public BillInboxStatus Status { get; set; }

        /// <summary>
        /// Source app the bill was ingested from.
        /// </summary>
        public BillInboxSource Source { get; set; }

        public DateTimeOffset ReceivedDate { get; set; }

        public long? MetadataId { get; set; }

        public PaymentRequestMetadataModel Metadata { get; set; }

        public int? VendorId { get; set; }

        public string VendorName { get; set; }

        public decimal? Amount { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public DateTimeOffset? BillDate { get; set; }

        public string BillNumber { get; set; }

        public long CreatedByUserId { get; set; }

        public DateTimeOffset Created { get; set; }

        public long? UpdatedByUserId { get; set; }

        public DateTimeOffset? Updated { get; set; }
    }
}
