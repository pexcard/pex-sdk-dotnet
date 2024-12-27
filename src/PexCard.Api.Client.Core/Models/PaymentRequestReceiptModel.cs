using System;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentRequestReceiptModel
    {
        public int ReceiptDocumentId { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
