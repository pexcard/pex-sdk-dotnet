using PexCard.Api.Client.Core.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentRequestModel : PaymentRequestModel
    {
        public PayeeFundsDestinationType PayeeFundsDestinationType { get; set; }

        public DateTimeOffset? BillDate { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public string BillRefNo { get; set; }

        public DateTimeOffset Created { get; set; }

        public long? PayeePexId { get; set; }
    }
}
