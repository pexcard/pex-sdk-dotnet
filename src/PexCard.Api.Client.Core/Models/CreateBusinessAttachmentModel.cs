namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Result of uploading a business source attachment.
    /// </summary>
    public class CreateBusinessAttachmentModel : AttachmentBaseModel
    {
        /// <summary>
        /// Metadata container id assigned to the uploaded attachment; used to fetch its analysis.
        /// </summary>
        public long MetadataId { get; set; }

        /// <summary>
        /// Link to the analysis resource for this attachment.
        /// </summary>
        public LinkSelfModel Link { get; set; }
    }
}
