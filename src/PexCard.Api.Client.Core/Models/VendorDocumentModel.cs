using System;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorDocumentModel
    {
        public int DocumentId { get; set; }
        public long UploadedByUserId { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
