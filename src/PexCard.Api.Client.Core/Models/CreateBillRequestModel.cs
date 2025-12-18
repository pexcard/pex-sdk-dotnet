using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class CreateBillRequestModel
    {
        public int VendorId { get; set; }
        public decimal Amount { get; set; }
        public BillPaymentMethodType? PaymentMethod { get; set; }
        public BillPaymentDetailsModel BillPayment { get; set; }
    }
}
