using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Models
{
    internal class AttachmentsModel
    {
        public List<AttachmentLinkModel> Attachments { get; set; }
        public MetadataStateType ApprovalStatus { get; set; }
    }
}
