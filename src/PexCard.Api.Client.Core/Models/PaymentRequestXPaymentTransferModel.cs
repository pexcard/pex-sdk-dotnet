using System;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentRequestXPaymentTransferModel
    {
        public int PaymentTransferPaymentRequestId { get; set; }

        public int PaymentRequestId { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
