namespace PexCard.Api.Client.Models
{
    internal class NoteRequestModel
    {
        public long TransactionId { get; set; }
        public string NoteText { get; set; }
        public bool Pending { get; set; }
        public bool VisibleToCardholder { get; set; }
    }
}
