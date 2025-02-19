using System;
using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentListRequestModel
    {
        public List<PaymentStatus> PaymentStatuses { get; set; }

        public List<PaymentStatusTrigger> PaymentStatusTriggers { get; set; }

        public DateTime? OutboundAchCreationStartDate { get; set; }

        public DateTime? OutboundAchCreationEndDate { get; set; }

        public DateTime? ExpectedPaymentStartDate { get; set; }

        public DateTime? ExpectedPaymentEndDate { get; set; }
    }
}
