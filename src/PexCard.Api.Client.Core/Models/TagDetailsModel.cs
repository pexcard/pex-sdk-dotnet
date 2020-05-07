using System;

namespace PexCard.Api.Client.Core.Models
{
    public class TagDetailsModel : TagModel
    {
        public string ConcurrencyKey { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public MetadataUserModel CreatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
        public MetadataUserModel UpdatedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        public MetadataUserModel DeletedBy { get; set; }
    }
}
