using System;

namespace PexCard.Api.Client.Core.Models
{
    public class DisputeCreditModel
    {
        /// <summary>Credit type: DisputeCredit, ChargeBackCredit or IsaFeeReversal.</summary>
        public string Type { get; set; }

        /// <summary>Credit amount.</summary>
        public decimal Amount { get; set; }

        /// <summary>Credit status: Done, Error, Skipped or Pending.</summary>
        public string Status { get; set; }

        /// <summary>Time the credit was issued (UTC).</summary>
        public DateTime? Date { get; set; }
    }
}
