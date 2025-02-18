using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentRequestMetadataModel
    {
        public IEnumerable<AttachmentModel> Attachments { get; set; }

        public IEnumerable<TagAnswerModel> TagAnswers { get; set; }

        public IEnumerable<TransactionNoteModel> Notes { get; set; }
    }
}
