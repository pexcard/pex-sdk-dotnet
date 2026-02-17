using System;

namespace PexCard.Api.Client.Core.Models
{
    public class CreditPaymentListRequestModel
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
