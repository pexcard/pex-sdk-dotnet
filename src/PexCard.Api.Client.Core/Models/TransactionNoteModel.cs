using System;

namespace PexCard.Api.Client.Core.Models
{
    public class TransactionNoteModel
    {
        public long NoteId { get; set; }
        public string NoteText { get; set; }

        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserMiddleName { get; set; }
        public string UserLastName { get; set; }
        public DateTime NoteDate { get; set; }

        public string UpdatedUserName { get; set; }
        public string UpdatedUserFirstName { get; set; }
        public string UpdatedUserMiddleName { get; set; }
        public string UpdatedUserLastName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}