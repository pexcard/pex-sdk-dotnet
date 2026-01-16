using System;
using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentListRequestModel
    {
        public DateTime? CreatedDateFrom { get; set; }

        public DateTime? CreatedDateTo { get; set; }

        public DateTime? DueDateFrom { get; set; }

        public DateTime? DueDateTo { get; set; }

        public long? CreatedByUserId { get; set; }

        public int? VendorId { get; set; }

        public decimal? AmountFrom { get; set; }

        public decimal? AmountTo { get; set; }

        public List<PaymentRequestStatus> PaymentRequestStatuses { get; set; }
    }
}
