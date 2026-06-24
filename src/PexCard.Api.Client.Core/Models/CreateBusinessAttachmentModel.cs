using PexCard.Api.Client.Core.Enums;

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
        /// Stored file name of the attachment.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Number of pages (for multi-page documents such as PDFs).
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// When the upload was detected as a duplicate, the id of the existing attachment it duplicates; otherwise null.
        /// </summary>
        public string DuplicateOfId { get; set; }

        /// <summary>
        /// Channel the attachment originated from (Email or Sms).
        /// </summary>
        public AttachmentUploadChannel UploadChannel { get; set; }

        /// <summary>
        /// Sender identifier the attachment was uploaded for (email address or phone number).
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Link to the analysis resource for this attachment.
        /// </summary>
        public LinkSelfModel Link { get; set; }
    }
}
