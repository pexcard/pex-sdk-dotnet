using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class AttachmentModel : AttachmentBaseModel
    {
        public string Content { get; set; }
        public MetadataStateType ApprovalStatus { get; set; }
    }
}
