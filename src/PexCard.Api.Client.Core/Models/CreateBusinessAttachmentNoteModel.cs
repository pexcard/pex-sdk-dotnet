namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Optional note to apply to the transaction a business source attachment is matched to.
    /// All fields are optional; when omitted the service defaults visibility to the cardholder
    /// (<see cref="VisibleToCardholder"/> = true) and treats the note as not system-generated
    /// (<see cref="IsSystemGenerated"/> = false).
    /// </summary>
    public class CreateBusinessAttachmentNoteModel
    {
        /// <summary>
        /// Note text. Max 255 characters. No note is created when null/empty.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Whether the note is visible to the cardholder. Defaults to true when omitted.
        /// </summary>
        public bool? VisibleToCardholder { get; set; }

        /// <summary>
        /// Whether the note is system-generated (vs. authored by a person). Defaults to false when omitted.
        /// </summary>
        public bool? IsSystemGenerated { get; set; }
    }
}
