using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class CreditPaymentModel
    {
        public int PaymentId { get; set; }

        public int InvoiceId { get; set; }

        public decimal Amount { get; set; }

        public DateTime DatePaid { get; set; }

        public PaymentTrigger PaymentTrigger { get; set; }
    }
}
