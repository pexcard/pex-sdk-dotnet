using System;

namespace PexCard.Api.Client.Core.Models
{
    public class TransactionResultNoteUserModel
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime Time { get; set; }
    }
}
