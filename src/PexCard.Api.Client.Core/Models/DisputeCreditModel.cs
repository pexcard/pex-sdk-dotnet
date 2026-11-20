using System;

namespace PexCard.Api.Client.Core.Models
{
    public class DisputeCreditModel
    {
        /// <summary>DisputeCredit, ChargeBackCredit or IsaFeeReversal.</summary>
        public string Type { get; set; }

        public decimal Amount { get; set; }

        /// <summary>Done, Error, Skipped or Pending.</summary>
        public string Status { get; set; }

        public DateTime? Date { get; set; }
    }
}
