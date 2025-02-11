using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentRequestMetadataModel
    {
        public IEnumerable<TagAnswerModel> TagAnswers { get; set; }
    }
}
