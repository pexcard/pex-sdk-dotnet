using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Intelligent (AI) match outcome for an attachment.
    /// </summary>
    public class AttachmentIntelligentMatchModel
    {
        /// <summary>Analysis operation identifier.</summary>
        public string OperationId { get; set; }

        /// <summary>Match status (e.g. Match, NoMatch, Skip, Unknown).</summary>
        public IntelligentMatchStatus Status { get; set; }

        /// <summary>Reason / explanation for the match outcome.</summary>
        public string Reason { get; set; }

        /// <summary>When the match was produced (UTC).</summary>
        public DateTime CreatedDateUtc { get; set; }
    }
}
