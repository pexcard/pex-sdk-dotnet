using System;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Who committed the attachment match and when.
    /// </summary>
    public class BusinessAttachmentMatchedByModel
    {
        /// <summary>When the match was finalized (UTC).</summary>
        public DateTime? DateUtc { get; set; }

        /// <summary>The user/admin that finalized the match.</summary>
        public MetadataUserModel By { get; set; }
    }
}
