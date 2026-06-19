namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// AI analysis result for a business source attachment.
    /// </summary>
    public class BusinessAttachmentAnalysisModel
    {
        /// <summary>Unique identifier of the attachment.</summary>
        public string AttachmentId { get; set; }

        /// <summary>Metadata container id the attachment belongs to.</summary>
        public long MetadataId { get; set; }

        /// <summary>Analysis details: operation, platform, timestamps, match criteria, and intelligent match.</summary>
        public AttachmentAnalysisDetailsModel Analysis { get; set; }
    }
}
