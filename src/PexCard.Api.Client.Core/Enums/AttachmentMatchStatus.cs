using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Committed match state of a business source attachment relative to a transaction
    /// (the match the Dashboard reflects).
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttachmentMatchStatus
    {
        /// <summary>No match activity has been recorded yet.</summary>
        Unknown,

        /// <summary>The attachment was evaluated and determined to have no matching transaction.</summary>
        NoMatch,

        /// <summary>The attachment was matched to a transaction manually by a user.</summary>
        ManualMatch,

        /// <summary>A match was suggested but not yet committed.</summary>
        SuggestedMatch,

        /// <summary>The attachment was automatically matched to a transaction and committed.</summary>
        AutoMatch,

        /// <summary>No match: the attachment is a duplicate of another.</summary>
        NoMatchDuplicate,

        /// <summary>No match: insufficient data could be extracted to match.</summary>
        NoMatchNoData,

        /// <summary>No match: a technical error prevented matching.</summary>
        NoMatchTechnicalError,

        /// <summary>Matching will be retried.</summary>
        Retry,

        /// <summary>No match: a previously committed auto-match was removed.</summary>
        NoMatchAutoMatchRemoved
    }
}
