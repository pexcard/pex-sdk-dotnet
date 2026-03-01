using System;

namespace PexCard.Api.Client.Core.Models
{
    public class BillAttachmentModel
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset UploadedDate { get; set; }
    }
}
