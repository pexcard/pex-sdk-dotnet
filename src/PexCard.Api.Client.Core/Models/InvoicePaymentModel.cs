using System;
using System.Collections.Generic;
using System.Text;

namespace PexCard.Api.Client.Core.Models
{
    public class InvoicePaymentModel
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
    }
}
