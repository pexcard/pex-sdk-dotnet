using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Committed match state of a business source attachment (its relationship to a transaction).
    /// </summary>
    public class BusinessAttachmentMatchModel
    {
        /// <summary>Committed match status (e.g. AutoMatch, ManualMatch, NoMatch, Retry).</summary>
        public AttachmentMatchStatus Status { get; set; }

        /// <summary>Id of the suggested match this attachment resolved to, if any.</summary>
        public string SuggestedMatchId { get; set; }

        /// <summary>When the attachment was determined to have no match, if applicable.</summary>
        public DateTime? NoMatchDateUtc { get; set; }

        /// <summary>When the match was committed (the receipt was attached to the transaction).</summary>
        public DateTime? CommitDateUtc { get; set; }

        /// <summary>Number of match attempts made.</summary>
        public int? MatchRetryCount { get; set; }

        /// <summary>Who/when finalized the match; null until matched.</summary>
        public BusinessAttachmentMatchedByModel Matched { get; set; }
    }
}
