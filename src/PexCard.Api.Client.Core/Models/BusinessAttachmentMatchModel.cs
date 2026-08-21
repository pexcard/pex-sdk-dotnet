using System;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Committed match state of a business source attachment (its relationship to a transaction).
    /// </summary>
    public class BusinessAttachmentMatchModel
    {
        /// <summary>Committed match status (e.g. AutoMatch, ManualMatch, SuggestedMatch, NoMatch, NoMatchDuplicate, NoMatchNoData, NoMatchTechnicalError, Retry, NoMatchAutoMatchRemoved, Unknown).</summary>
        public string Status { get; set; }

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

        /// <summary>Auth transaction id of the matched transaction; null when unmatched.</summary>
        public long? AuthTranId { get; set; }

        /// <summary>Network transaction id of the matched transaction; null when unmatched.</summary>
        public long? NetworkTranId { get; set; }

        /// <summary>Settled transaction id (the same id /Details uses); null while the transaction is pending (auth-only), set once settled.</summary>
        public long? TransactionId { get; set; }
    }
}
