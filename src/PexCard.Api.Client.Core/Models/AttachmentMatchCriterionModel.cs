using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// A single extracted/matched field from attachment analysis.
    /// </summary>
    public class AttachmentMatchCriterionModel
    {
        /// <summary>Criterion name.</summary>
        public string Name { get; set; }

        /// <summary>Extracted value.</summary>
        public string Value { get; set; }

        /// <summary>Confidence score (0-1).</summary>
        public float Confidence { get; set; }

        /// <summary>Criterion type (e.g. MerchantName, Total, Date).</summary>
        public MatchCriterionType Type { get; set; }
    }
}
