using System;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentDetailsModel
    {
        public DateTimeOffset BillDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string BillRefNo { get; set; }
    }
}
