using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// A business source attachment (emailed / SMS'd receipt or invoice) and its committed match state.
    /// </summary>
    public class BusinessAttachmentModel : AttachmentBaseModel
    {
        /// <summary>Metadata container id the attachment belongs to.</summary>
        public long MetadataId { get; set; }

        /// <summary>Stored file name of the attachment.</summary>
        public string FileName { get; set; }

        /// <summary>Channel the attachment originated from (Email or Sms).</summary>
        public AttachmentUploadChannel UploadChannel { get; set; }

        /// <summary>Committed match state; null when no match activity has occurred yet.</summary>
        public BusinessAttachmentMatchModel Match { get; set; }
    }
}
