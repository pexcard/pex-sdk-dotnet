using System;

namespace PexCard.Api.Client.Core.Models
{
    public class BillNoteModel
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public bool VisibleToCardholder { get; set; }
        public bool SystemGenerated { get; set; }
        public long? CreatedByUserId { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
