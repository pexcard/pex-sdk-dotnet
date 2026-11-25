using System;

namespace PexCard.Api.Client.Core.Models
{
    public class DisputeModel
    {
        /// <summary>Dispute status: Open, Pending or Closed.</summary>
        public string Status { get; set; }

        /// <summary>Dispute decision: WriteOff, Recovered, MerchantIssuedCredit, Expired, Withdrawn, Denied or Pending. Null until a decision is made.</summary>
        public string Decision { get; set; }

        /// <summary>Time the decision was made (UTC). Null until a decision is made.</summary>
        public DateTime? DecisionDate { get; set; }

        /// <summary>Time the dispute was opened (UTC).</summary>
        public DateTime OpenedDate { get; set; }

        /// <summary>Credit issued to the card for this purchase. Null until a credit is issued.</summary>
        public DisputeCreditModel Credit { get; set; }
    }
}
