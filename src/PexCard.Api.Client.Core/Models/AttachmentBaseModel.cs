using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public abstract class AttachmentBaseModel
    {
        public string AttachmentId { get; set; }
        public AttachmentType Type { get; set; }
        public long Size { get; set; }
        public MetadataFileSaveState UploadStatus { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public MetadataUserModel CreatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
        public MetadataUserModel UpdatedBy { get; set; }
    }
}
