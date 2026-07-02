using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Request to upload a business source attachment (an emailed / SMS'd receipt or invoice) for AI analysis
    /// and auto-matching to a cardholder purchase. The <see cref="Source"/> must resolve to a cardholder in the
    /// authenticated business.
    /// </summary>
    public class CreateBusinessAttachmentRequestModel
    {
        /// <summary>
        /// Base64-encoded file contents (raw base64, not a data URI). Required.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Attachment type (Image or Pdf). Required.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AttachmentType Type { get; set; }

        /// <summary>
        /// Channel the attachment originated from (Email or Sms). Required.
        /// </summary>
        public AttachmentUploadChannel UploadChannel { get; set; }

        /// <summary>
        /// Sender identifier used to resolve the cardholder: email address (Email) or phone number (Sms). Required.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Optional original file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Optional note applied to the transaction the attachment is matched to. Omit to add no note.
        /// </summary>
        public CreateBusinessAttachmentNoteModel Note { get; set; }
    }
}
