using System;

namespace PexCard.Api.Client.Core.Models
{
    public class DisputeModel
    {
        /// <summary>Open, Pending or Closed.</summary>
        public string Status { get; set; }

        /// <summary>WriteOff, Recovered, MerchantIssuedCredit, Expired, Withdrawn, Denied or Pending. Null until a decision is made.</summary>
        public string Decision { get; set; }

        public DateTime? DecisionDate { get; set; }

        public DateTime OpenedDate { get; set; }

        /// <summary>Credit issued for the purchase. Null until a credit is issued.</summary>
        public DisputeCreditModel Credit { get; set; }
    }
}
