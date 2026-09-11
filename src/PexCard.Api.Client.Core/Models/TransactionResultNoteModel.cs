namespace PexCard.Api.Client.Core.Models
{
    public class TransactionResultNoteModel
    {
        public long Id { get; set; }

        public string Content { get; set; }

        public bool SystemGenerated { get; set; }

        public TransactionResultNoteUserModel Created { get; set; }

        public TransactionResultNoteUserModel Updated { get; set; }
    }
}
