using System;

namespace PexCard.Api.Client.Core.Models
{
    public class CreateBillRequestModel
    {
        public int VendorId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = "AutoSelect";

        public CreateBillPaymentDetailsModel BillPayment { get; set; }
    }

    public class CreateBillPaymentDetailsModel
    {
        public DateTimeOffset? BillDate { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public string BillRefNo { get; set; }
    }
}
