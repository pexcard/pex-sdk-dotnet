using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class InvoicePaymentModel
    {
        public int PaymentId { get; set; }
        public PaymentType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
