using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class CreateBillInboxRequestModel
    {
        /// <summary>
        /// Source app the bill came from. Required.
        /// <see cref="BillInboxSource.Email"/> is reserved for the PEX inbound-email channel and is not allowed here.
        /// </summary>
        public BillInboxSource Source { get; set; }

        public string VendorName { get; set; }

        public int VendorId { get; set; }

        public decimal Amount { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public DateTimeOffset? BillDate { get; set; }

        public string BillNumber { get; set; }
    }
}
