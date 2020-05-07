using System;

namespace PexCard.Api.Client.Core.Models
{
    public class TransactionNoteModel
    {
        public long NoteId { get; set; }
        public string UserName { get; set; }
        public string NoteText { get; set; }
        public DateTime NoteDate { get; set; }
    }
}