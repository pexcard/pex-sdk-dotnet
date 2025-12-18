namespace PexCard.Api.Client.Core.Models
{
    public class AddBillNoteRequestModel
    {
        public string Note { get; set; }
        public bool VisibleToCardholder { get; set; }
        public bool SystemGenerated { get; set; }
    }
}
