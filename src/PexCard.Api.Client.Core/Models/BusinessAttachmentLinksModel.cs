using System;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Presigned links to a business source attachment's file content. Short-lived: re-fetch the attachment
    /// when the links are needed rather than storing them, since they stop working at <see cref="Expiration"/>.
    /// </summary>
    public class BusinessAttachmentLinksModel
    {
        /// <summary>Link to the full-size file.</summary>
        public string Full { get; set; }

        /// <summary>Link to the thumbnail rendition, when one is available.</summary>
        public string Thumbnail { get; set; }

        /// <summary>UTC instant at which the links stop working.</summary>
        public DateTime Expiration { get; set; }
    }
}
